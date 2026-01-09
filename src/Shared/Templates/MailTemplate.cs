namespace api_infor_cell.src.Shared.Templates
{

    public static class MailTemplate
    {
        private static readonly string UiURI =  Environment.GetEnvironmentVariable("UI_URI") ?? "";
        public static string ForgotPasswordWeb(string name, string code)
        {
            return $@"
                <html>
                    <head>
                        <style>
                            .container {{
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                background-color: #ffffff;
                                padding: 40px;
                                border-radius: 10px;
                                max-width: 600px;
                                margin: 20px auto;
                                color: #333;
                                border: 1px solid #e0e0e0;
                                box-shadow: 0 4px 6px rgba(0,0,0,0.1);
                            }}
                            .header {{
                                text-align: center;
                                color: #2c3e50;
                                border-bottom: 2px solid #34495e;
                                padding-bottom: 20px;
                                margin-bottom: 20px;
                            }}
                            .instruction {{
                                font-size: 16px;
                                line-height: 1.6;
                                text-align: center;
                            }}
                            .code-display {{
                                background-color: #f4f7f6;
                                border: 1px solid #cfd8dc;
                                padding: 15px;
                                text-align: center;
                                font-size: 28px;
                                font-weight: bold;
                                color: #2c3e50;
                                margin: 25px 0;
                                border-radius: 5px;
                                letter-spacing: 4px;
                            }}
                            .alert {{
                                background-color: #fff3cd;
                                color: #856404;
                                padding: 15px;
                                border-radius: 5px;
                                font-size: 13px;
                                margin-top: 25px;
                                border-left: 5px solid #ffeeba;
                            }}
                            .footer {{
                                margin-top: 30px;
                                font-size: 12px;
                                color: #95a5a6;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <h2>Recuperação de Senha</h2>
                            </div>
                            
                            <p class=""instruction"">Olá <strong>{name}</strong>,</p>
                            <p class=""instruction"">Recebemos uma solicitação para redefinir a senha da sua conta na <strong>Telemovvi</strong>. Se foi você, utilize o link abaixo:</p>
                            
                            <div class=""flex justify-center items-center"">
                                <a class='text-center' href='{UiURI}/reset-password/{code}'>Link para alterar senha</a>
                            </div>
                                                        
                            <div class=""alert"">
                                <strong>Segurança:</strong> Se você não solicitou a alteração da sua senha, ignore este e-mail. Sua senha atual permanecerá segura e nenhuma ação será tomada.
                            </div>
                            
                            <div class=""footer"">
                                <hr style=""border: 0; border-top: 1px solid #eee;"" />
                                <p>Atenciosamente,<br><strong>Equipe Telemovvi</strong></p>
                                <p>Este é um e-mail automático, por favor não responda.</p>
                            </div>
                        </div>
                    </body>
                </html>";
        }
        public static string ForgotPasswordApp(string code)
        {
            return $@"
                <html>
                    <head>
                        <style>
                        .container {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            padding: 20px;
                            border-radius: 8px;
                            max-width: 600px;
                            margin: auto;
                            color: #333;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin-top: 20px;
                            background-color: #007bff;
                            color: #fff;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                        .footer {{
                            margin-top: 30px;
                            font-size: 12px;
                            color: #888;
                        }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                        <h2>Redefinição de Senha</h2>
                        <p>Você solicitou a alteração da sua senha.</p>
                        <p>Código de alteração da senha: {code}.</p>                        
                        <p>Se você não solicitou esta alteração, ignore este e-mail.</p>
                        <div class=""footer"">
                            <p>Este é um e-mail automático. Não responda esta mensagem.</p>
                        </div>
                        </div>
                    </body>
                </html>";
        }
        public static string FirstAccess(string name, string email, string passowrd)
        {
            return $@"               
                <html>
                    <head>
                        <style>
                            .container {{
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                background-color: #ffffff;
                                padding: 40px;
                                border-radius: 10px;
                                max-width: 600px;
                                margin: 20px auto;
                                color: #333;
                                border: 1px solid #e0e0e0;
                                box-shadow: 0 4px 6px rgba(0,0,0,0.1);
                            }}
                            .header {{
                                text-align: center;
                                border-bottom: 2px solid #007bff;
                                padding-bottom: 20px;
                                margin-bottom: 20px;
                            }}
                            .code-box {{
                                background-color: #f8f9fa;
                                border: 2px dashed #007bff;
                                padding: 20px;
                                text-align: center;
                                font-size: 32px;
                                font-weight: bold;
                                letter-spacing: 5px;
                                color: #007bff;
                                margin: 30px 0;
                                border-radius: 8px;
                            }}
                            .footer {{
                                margin-top: 30px;
                                font-size: 13px;
                                color: #777;
                                text-align: center;
                                line-height: 1.6;
                            }}
                            .welcome-text {{
                                font-size: 18px;
                                margin-bottom: 10px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <h1>Bem-vindo à Telemovvi!</h1>
                            </div>
                            
                            <p class=""welcome-text"">Olá, <strong>{name}</strong>,</p>
                            <p>Dados do primeiro acesso ao sistema:</p>

                            <p>E-mail: {email}</p>                        
                            <p>Senha: {passowrd}</p>      
                            <a href=""{UiURI}"">Fazer Login</a>                            
                            <p>Este código expira em 5 minutos. Se você não solicitou a criação desta conta, por favor, ignore este e-mail.</p>
                            
                            <div class=""footer"">
                                <hr style=""border: 0; border-top: 1px solid #eee;"" />
                                <p>Atenciosamente,<br><strong>Equipe Telemovvi</strong></p>
                                <p>Este é um e-mail automático, por favor não responda.</p>
                            </div>
                        </div>
                    </body>
                </html>
            ";
        }
        public static string ConfirmAccount(string name, string code)
        {
            return $@"
                <html>
                    <head>
                        <style>
                            .container {{
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                background-color: #ffffff;
                                padding: 40px;
                                border-radius: 10px;
                                max-width: 600px;
                                margin: 20px auto;
                                color: #333;
                                border: 1px solid #e0e0e0;
                                box-shadow: 0 4px 6px rgba(0,0,0,0.1);
                            }}
                            .header {{
                                text-align: center;
                                border-bottom: 2px solid #007bff;
                                padding-bottom: 20px;
                                margin-bottom: 20px;
                            }}
                            .code-box {{
                                background-color: #f8f9fa;
                                border: 2px dashed #007bff;
                                padding: 20px;
                                text-align: center;
                                font-size: 32px;
                                font-weight: bold;
                                letter-spacing: 5px;
                                color: #007bff;
                                margin: 30px 0;
                                border-radius: 8px;
                            }}
                            .footer {{
                                margin-top: 30px;
                                font-size: 13px;
                                color: #777;
                                text-align: center;
                                line-height: 1.6;
                            }}
                            .welcome-text {{
                                font-size: 18px;
                                margin-bottom: 10px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <h1>Bem-vindo à Telemovvi!</h1>
                            </div>
                            
                            <p class=""welcome-text"">Olá, <strong>{name}</strong>,</p>
                            <p>Ficamos felizes em ter você conosco. Para concluir a criação da sua conta e garantir a segurança dos seus dados, utilize o código de verificação abaixo:</p>
                            
                            <div class=""code-box"">
                                {code}
                            </div>
                            
                            <p>Este código expira em 5 minutos. Se você não solicitou a criação desta conta, por favor, ignore este e-mail.</p>
                            
                            <div class=""footer"">
                                <hr style=""border: 0; border-top: 1px solid #eee;"" />
                                <p>Atenciosamente,<br><strong>Equipe Telemovvi</strong></p>
                                <p>Este é um e-mail automático, por favor não responda.</p>
                            </div>
                        </div>
                    </body>
                </html>";
        }
        public static string NewCodeConfirmAccount(string name, string code)
        {
            return $@"
                <html>
                    <head>
                        <style>
                            .container {{
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                background-color: #ffffff;
                                padding: 40px;
                                border-radius: 10px;
                                max-width: 600px;
                                margin: 20px auto;
                                color: #333;
                                border: 1px solid #e0e0e0;
                                box-shadow: 0 4px 6px rgba(0,0,0,0.1);
                            }}
                            .header {{
                                text-align: center;
                                color: #007bff; /* Cor de alerta/atenção */
                                border-bottom: 2px solid #007bff;
                                padding-bottom: 20px;
                                margin-bottom: 20px;
                            }}
                            .code-box {{
                                background-color: #f8f9fa;
                                border: 2px dashed #007bff;
                                padding: 20px;
                                text-align: center;
                                font-size: 32px;
                                font-weight: bold;
                                letter-spacing: 5px;
                                color: #007bff;
                                margin: 30px 0;
                                border-radius: 8px;
                            }}
                            .info-text {{
                                font-size: 16px;
                                line-height: 1.5;
                                text-align: center;
                            }}
                            .footer {{
                                margin-top: 30px;
                                font-size: 12px;
                                color: #999;
                                text-align: center;
                            }}
                            .warning {{
                                background-color: #fff3cd;
                                color: #856404;
                                padding: 10px;
                                border-radius: 5px;
                                font-size: 14px;
                                margin-top: 20px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <h2>Novo Código de Verificação</h2>
                            </div>
                            
                            <p class=""info-text"">Olá <strong>{name}</strong>,</p>
                            <p class=""info-text"">Você solicitou um novo código de acesso para a sua conta na <strong>Telemovvi</strong>. Utilize o código abaixo para prosseguir:</p>
                            
                            <div class=""code-box"">
                                {code}
                            </div>
                            
                            <div class=""warning"">
                                <strong>Atenção:</strong> Este código é válido por 5 minutos. Por segurança, não compartilhe este código com ninguém.
                            </div>
                            
                            <p class=""info-text"" style=""margin-top: 20px;"">Se você não solicitou este código, ignore este e-mail ou entre em contato com nosso suporte.</p>
                            
                            <div class=""footer"">
                                <hr style=""border: 0; border-top: 1px solid #eee;"" />
                                <p>Atenciosamente,<br><strong>Equipe Telemovvi</strong></p>
                                <p>Este é um e-mail automático, por favor não responda.</p>
                            </div>
                        </div>
                    </body>
                </html>";
        }
    }
}