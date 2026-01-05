using System.Text;
using System.Text.Json;

public class CompanyQueryMiddleware
{
    private readonly RequestDelegate _next;

    public CompanyQueryMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            string path = context.Request.Path;
            string method = context.Request.Method;
            string? plan = context.User.FindFirst("plan")?.Value;

            if(path != "/api/companies") 
            {
                var companiesClaim = context.User.FindFirst("companies")?.Value;
                
                if (!string.IsNullOrEmpty(companiesClaim))
                {
                    try 
                    {
                        var companyIds = JsonSerializer.Deserialize<List<string>>(companiesClaim);

                        if (companyIds != null && companyIds.Any())
                        {
                            var queryItems = context.Request.Query.ToDictionary(x => x.Key, x => x.Value);
                            
                            queryItems["company"] = new Microsoft.Extensions.Primitives.StringValues(companyIds.ToArray());

                            context.Request.Query = new QueryCollection(queryItems);
                        }
                    }
                    catch (JsonException)
                    {

                    }
                }
            }
            else 
            {                
                if(method == HttpMethods.Post && context.Request.ContentType?.Contains("application/json") == true)
                {
                    context.Request.EnableBuffering();

                    using (var reader = new StreamReader(context.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                    {
                        var body = await reader.ReadToEndAsync();
                        
                        if (!string.IsNullOrEmpty(body))
                        {
                            var jsonDoc = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
                            if (jsonDoc != null)
                            {
                                jsonDoc["plan"] = plan!;

                                var modifiedBody = JsonSerializer.Serialize(jsonDoc);
                                var bytes = Encoding.UTF8.GetBytes(modifiedBody);

                                context.Request.Body = new MemoryStream(bytes);
                                context.Request.ContentLength = bytes.Length;
                            }
                        }
                    }
                }
            }
        }

        await _next(context);
    }
}