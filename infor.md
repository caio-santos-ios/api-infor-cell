1. Estrutura do Arquivo docker-compose.yml

services:
  # Banco de Dados
  mongodb:
    image: mongo:latest
    container_name: mongodb_erp
    restart: always
    ports:
      - "27017:27017"
    volumes:
      - mongo_data:/data/db
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: sua_senha_forte

  # Backend .NET
  api:
    build: ./api-folder
    container_name: dotnet_api
    restart: always
    depends_on:
      - mongodb
    environment:
      - ConnectionStrings__DefaultConnection=mongodb://admin:sua_senha_forte@mongodb:27017/db_name?authSource=admin
      - ASPNETCORE_ENVIRONMENT=Production
    ports:
      - "5000:8080"

  # Frontend Next.js
  frontend:
    build: ./frontend-folder
    container_name: nextjs_front
    restart: always
    ports:
      - "3000:3000"
    environment:
      - NEXT_PUBLIC_API_URL=http://seu-ip-ou-dominio:5000

volumes:
  mongo_data:


2. docker compose up -d --build


3. Instalação do Nginx
# Atualiza a lista de pacotes
sudo apt update

# Instala o Nginx
sudo apt install nginx -y

# Verifica se o serviço está rodando
sudo systemctl status nginx

4. Configuração do Firewall
sudo ufw allow 'Nginx Full'

5. Criação do Arquivo de Configuração
# Cria o arquivo de configuração
sudo nano /etc/nginx/sites-available/meu-erp

server {
    listen 80;
    server_name seu-dominio.com www.seu-dominio.com;

    # Frontend (Next.js rodando na porta 3000)
    location / {
        proxy_pass http://localhost:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    # Backend (API .NET rodando na porta 5000)
    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        # Ajuste para não perder o IP real do cliente na API
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

6. Ativando a Configuração
# Ativa o site
sudo ln -s /etc/nginx/sites-available/meu-erp /etc/nginx/sites-enabled/

# Remove o arquivo default para evitar conflitos (opcional)
sudo rm /etc/nginx/sites-enabled/default

# Testa se a sintaxe do Nginx está correta
sudo nginx -t

# Reinicia o Nginx para aplicar as mudanças
sudo systemctl restart nginx


7. Próximo Passo: SSL Grátis (HTTPS)
# Instala o Certbot para Nginx
sudo apt install certbot python3-certbot-nginx -y

# Gera e instala o certificado automaticamente
sudo certbot --nginx -d seu-dominio.com -d www.seu-dominio.com