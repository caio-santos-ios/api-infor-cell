using api_infor_cell.src.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Responses;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Handlers;
using api_infor_cell.src.Shared.Templates;
using api_infor_cell.src.Shared.Validators;
using api_infor_cell.src.Shared.Utils;
using System.Text.Json;

namespace api_infor_cell.src.Services
{
    public class AuthService(IUserRepository repository, IPlanRepository planRepository, ICompanyRepository companyRepository, MailHandler mailHandler) : IAuthService
    {
        public async Task<ResponseApi<AuthResponse>> LoginAsync(LoginDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email)) return new(null, 400, "E-mail é obrigatório");
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                
                ResponseApi<User?> res = await repository.GetByEmailAsync(request.Email);
                if(res.Data is null) return new(null, 400, "Dados incorretos");
                User user = res.Data!;

                if(user is null) return new(null, 400, "Dados incorretos");
                bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if(!isValid) return new(null, 400, "Dados incorretos");

                ResponseApi<Company?> company = await companyRepository.GetByIdAsync(user.Company);

                AuthResponse response = new ()
                {
                    Token = GenerateJwtToken(user), 
                    RefreshToken = GenerateJwtToken(user, true), 
                    Name = user.Name, 
                    Id = user.Id, 
                    Admin = user.Admin, 
                    Modules = user.Modules, 
                    Photo = user.Photo, 
                    Email = user.Email,
                    Plan = user.Plan,
                    LogoCompany = company.Data is not null ? company.Data.Photo : "",
                    NameCompany = company.Data is not null ? company.Data.CorporateName : ""
                };

                return new(response);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<dynamic>> RegisterAsync(RegisterDTO request)
        {
            try
            {
                if (!request.PrivacyPolicy) return new(null, 400, "Aceitar os Termos e Condições e nossa Política de Privacidade é obrigatório");
                if (string.IsNullOrEmpty(request.CompanyName)) return new(null, 400, "Nome da empresa é obrigatório");
                if (string.IsNullOrEmpty(request.Name)) return new(null, 400, "Nome completo é obrigatório");
                if (string.IsNullOrEmpty(request.Email)) return new(null, 400, "E-mail é obrigatório");
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                
                ResponseApi<User?> isEmail = await repository.GetByEmailAsync(request.Email);
                if(isEmail.Data is not null || !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");

                if(Validator.IsReliable(request.Password).Equals("Ruim")) return new(null, 400, $"Senha é muito fraca");

                dynamic access = Util.GenerateCodeAccess(5);

                User user = new()
                {
                    UserName = $"usuário{access.CodeAccess}",
                    Email = request.Email,
                    Phone = request.Phone,
                    Name = request.Name,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    CodeAccess = access.CodeAccess,
                    CodeAccessExpiration = access.CodeAccessExpiration,
                    ValidatedAccess = false,
                    Modules = [],
                    Admin = true,
                    Blocked = false,
                    Whatsapp = request.Whatsapp,
                    Role = Enums.User.RoleEnum.Admin
                };

                ResponseApi<User?> response = await repository.CreateAsync(user);
                if(response.Data is null) return new(null, 400, "Falha ao criar conta.");

                DateTime date = DateTime.UtcNow;

                ResponseApi<Plan?> responsePlan = await planRepository.CreateAsync(new ()
                {
                    StartDate = date,
                    ExpirationDate = date.AddDays(7),
                    Type = "free"
                });

                if(responsePlan.Data is null) return new(null, 400, "Falha ao criar conta.");

                ResponseApi<Company?> responseCompany = await companyRepository.CreateAsync(new ()
                {
                    CorporateName = request.CompanyName,
                    Phone = request.Phone,
                    Plan = responsePlan.Data.Id
                });

                if(responseCompany.Data is null) return new(null, 400, "Falha ao criar conta.");

                response.Data.Companies.Add(responseCompany.Data.Id);
                response.Data.Company = responseCompany.Data.Id;
                response.Data.Plan = responsePlan.Data.Id;
                
                await repository.UpdateAsync(response.Data);

                                
                await mailHandler.SendMailAsync(request.Email, "Código de Confirmação", MailTemplate.ConfirmAccount(request.Name, access.CodeAccess));

                return new(null, 201, "Conta criada com sucesso, foi enviado o e-mail de confirmação.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<dynamic>> ConfirmAccountAsync(ConfirmAccountDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Code)) return new(null, 400, "Código de confirmação é obrigatório");
                
                ResponseApi<User?> user = await repository.GetByCodeAccessAsync(request.Code);
                if(user.Data is null) return new(null, 400, "Código inválido.");
                // if(!user.Data.ValidatedAccess) return new(null, 400, "Código inválido.");

                if(user.Data.CodeAccessExpiration < DateTime.UtcNow) return new(null, 400, "Código expirou, solicite um novo código.");

                user.Data.CodeAccess = "";
                user.Data.CodeAccessExpiration = null;
                user.Data.ValidatedAccess = true;

                ResponseApi<User?> response = await repository.UpdateAsync(user.Data);
                if(response.Data is null) return new(null, 400, "Falha ao solicitar novo código.");
                                
                return new(null, 200, "Conta verificada com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<dynamic>> NewCodeConfirmAsync(RegisterDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email)) return new(null, 400, "E-mail é obrigatório");
                
                ResponseApi<User?> user = await repository.GetByEmailAsync(request.Email);
                if(user.Data is null || !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");

                dynamic access = Util.GenerateCodeAccess(5);

                user.Data.CodeAccess = access.CodeAccess;
                user.Data.CodeAccessExpiration = access.CodeAccessExpiration;

                ResponseApi<User?> response = await repository.UpdateAsync(user.Data);
                if(response.Data is null) return new(null, 400, "Falha ao solicitar novo código.");
                                
                await mailHandler.SendMailAsync(request.Email, "Novo Código de Verificação", MailTemplate.NewCodeConfirmAccount(request.Name, access.CodeAccess));

                return new(null, 200, "Novo código foi enviado.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<AuthResponse>> RefreshTokenAsync(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                SecurityToken? validatedToken;

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("ISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("AUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY") ?? "")
                    ),
                    ValidateLifetime = false 
                };

                var principal = handler.ValidateToken(token, validationParameters, out validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                if (jwtToken == null) return new(null, 401, "Token inválido.");

                string? tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "type")?.Value;
                if (tokenType != "refresh") return new(null, 401, "O token fornecido não é um refresh token.");

                var userId = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId)) return new(null, 401, "Usuário não encontrado no token.");

                ResponseApi<User?> user = await repository.GetByIdAsync(userId);
                if (user.Data is null) return new(null, 401, "Usuário não encontrado.");

                string accessToken = GenerateJwtToken(user.Data);
                string refreshToken = GenerateJwtToken(user.Data, true);

                return new(new AuthResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Id = user.Data.Id
                });
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<User>> ResetPasswordAsync(ResetPasswordDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                if (string.IsNullOrEmpty(request.Id)) return new(null, 400, "Falha ao alterar senha");
                
                if(Validator.IsReliable(request.Password).Equals("Ruim")) return new(null, 400, $"Senha é muito fraca");

                ResponseApi<User?> user = await repository.GetByIdAsync(request.Id);
                if(!user.IsSuccess || user.Data is null) return new(null, 400, "Falha ao alterar senha");
                
                bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Data.Password);
                if(!isValid) return new(null, 400, "Senha antiga incorreta");

                user.Data.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                ResponseApi<User?> response = await repository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao alterar senha");

                return new(null, 200, "Senha alterada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<User>> RequestForgotPasswordAsync(ForgotPasswordDTO request)
        {
            try
            {
                ResponseApi<User?> user = await repository.GetByEmailAsync(request.Email);
                if(user.Data is null || !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");

                dynamic access = Util.GenerateCodeAccess();

                user.Data.CodeAccess = access.CodeAccess;
                user.Data.CodeAccessExpiration = access.CodeAccessExpiration;
                user.Data.ValidatedAccess = false;

                string template = MailTemplate.ForgotPasswordWeb(user.Data.Name, user.Data.CodeAccess);

                await mailHandler.SendMailAsync(request.Email, "Redefinição de Senha", template);

                ResponseApi<User?> response = await repository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao redefinir senha");

                return new(null, 200, "Foi enviado um e-mail para redefinir sua senha");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        public async Task<ResponseApi<User>> ResetPassordForgotAsync(ResetPasswordDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                if (string.IsNullOrEmpty(request.NewPassword)) return new(null, 400, "Confirmação da senha é obrigatória");
                if (request.Password != request.NewPassword) return new(null, 400, "As senhas não podem ser diferentes");

                ResponseApi<User?> user = await repository.GetByCodeAccessAsync(request.CodeAccess);
                if(!user.IsSuccess || user.Data is null) return new(null, 400, "Falha ao alterar senha");

                if(user.Data.CodeAccessExpiration < DateTime.UtcNow) return new(null, 400, "Código expirou, solicite um novo e-mail.");
                
                if(Validator.IsReliable(request.Password).Equals("Ruim")) return new(null, 400, $"Senha é muito fraca");

                user.Data.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.Data.ValidatedAccess = true;
                user.Data.CodeAccess = "";
                user.Data.CodeAccessExpiration = null;

                ResponseApi<User?> response = await repository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao alterar senha");

                return new(null, 200, "Senha alterada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");            
            }
        }
        private static string GenerateJwtToken(User user, bool refresh = false)
        {
            string? SecretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? "";
            string? Issuer = Environment.GetEnvironmentVariable("ISSUER") ?? "";
            string? Audience = Environment.GetEnvironmentVariable("AUDIENCE") ?? "";

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(SecretKey));

            var companiesJson = JsonSerializer.Serialize(user.Companies);

            Claim[] claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("companies", companiesJson),
                new Claim("plan", user.Plan),
                new Claim(JwtRegisteredClaimNames.Nickname, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("type", refresh ? "refresh" : "access")
            ];

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: refresh ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}