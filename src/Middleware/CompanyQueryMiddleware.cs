using System.Text;
using System.Text.Json;
using api_infor_cell.src.Shared.Utils;

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
            string? company = context.User.FindFirst("company")?.Value;
            string? store = context.User.FindFirst("store")?.Value;
            
            if(path.Split("/")[2] != "companies") 
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
                                jsonDoc["company"] = company!;
                                jsonDoc["store"] = store!;

                                var modifiedBody = JsonSerializer.Serialize(jsonDoc);
                                var bytes = Encoding.UTF8.GetBytes(modifiedBody);

                                context.Request.Body = new MemoryStream(bytes);
                                context.Request.ContentLength = bytes.Length;
                            }
                        }
                    }
                } 
                else 
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