# MusicStreamer 🎵

Projeto prático de uma plataforma de streaming de música desenvolvido para a disciplina de **Desenvolvimento de Sistemas com .NET** do Instituto INFNET.

---

## 🚀 Como Rodar o Projeto Localmente (Guia Rápido)

Siga os passos abaixo para compilar, configurar o banco de dados e rodar a API junto com o Frontend MVC.

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior.
- [SQL Server LocalDB](https://docs.microsoft.com/pt-br/sql/database-engine/configure-windows/sql-server-express-localdb) (geralmente instalado junto ao Visual Studio).

---

### Passo 1: Clonar o repositório
```bash
git clone <url-do-repositorio>
cd amplifike_musica
```

### Passo 2: Instalar a ferramenta de migrations do Entity Framework Core
Se você ainda não tiver o `dotnet-ef` instalado globalmente:
```bash
dotnet tool install --global dotnet-ef --version 8.0.11
```

### Passo 3: Criar o banco de dados e aplicar o Seed (Dados Iniciais)
Execute o comando a partir do diretório raiz da solução para gerar o banco `MusicStreamerDb` localmente no LocalDB e popular as tabelas:
```bash
dotnet ef database update \
  --project src/MusicStreamer.Infrastructure \
  --startup-project src/MusicStreamer.Api
```

> ℹ️ **Dados populados pelo Seed automático:**
> - **Planos de Assinatura:** Gratuito (R$ 0), Premium (R$ 19,90) e Família (R$ 34,90).
> - **Usuário Administrador:** Login: `admin@musicstreamer.com` | Senha: `Admin@123`
> - **Bandas, Álbuns e Músicas:** 4 bandas de rock já cadastradas com seus respectivos álbuns e faixas.

---

### Passo 4: Executar a API
Abra um terminal na raiz e execute:
```bash
dotnet run --project src/MusicStreamer.Api/MusicStreamer.Api.csproj
```
- **Swagger / OpenAPI:** [http://localhost:5191/swagger/index.html](http://localhost:5191/swagger/index.html)

---

### Passo 5: Executar o Frontend MVC
Abra outro terminal na raiz e execute:
```bash
dotnet run --project src/MusicStreamer.Web/MusicStreamer.Web.csproj
```
- **Página Inicial do App:** [http://localhost:5110](http://localhost:5110)

---

### Passo 6: Rodar os Testes Unitários
Para rodar a suíte de testes do projeto:
```bash
dotnet test
```

---

## 🔑 Credenciais de Acesso para Testes

### 🧑‍💻 Usuário Administrador (Cadastrado via Seed)
* **E-mail:** `admin@musicstreamer.com`
* **Senha:** `Admin@123`
*(Permite testar as rotas restritas de criação de bandas, álbuns e músicas).*

### 🧑‍User Usuário Padrão
* Crie uma nova conta clicando em **"Criar conta"** no menu superior do Frontend MVC ou use o endpoint `POST /api/auth/register`.

---

## 📦 Estrutura da Solução (Clean Architecture)

A solução está dividida em 4 camadas principais estruturadas segundo as boas práticas do Clean Architecture, além dos testes:

```
MusicStreamer/
├── src/
│   ├── MusicStreamer.Api/             ← Ponto de entrada HTTP, JWT, Swagger, Middlewares
│   ├── MusicStreamer.Application/     ← Serviços (Camada de Aplicação), DTOs, Interfaces
│   ├── MusicStreamer.Domain/          ← Entidades de domínio, Enums, Regras de Negócio, Interfaces de Repositório
│   ├── MusicStreamer.Infrastructure/  ← Contexto do EF Core, Fluent API, Repositórios, Migrations, Seeds
│   └── MusicStreamer.Web/             ← App Frontend ASP.NET Core MVC 
├── tests/
│   └── MusicStreamer.Tests/           ← Testes Unitários de Serviços (xUnit + Moq)
```

---

## 🛠️ Principais Recursos e Regras Implementados

1. **Autenticação:** JWT Bearer Token customizado.
2. **Autorização:** Acesso controlado por roles (`Admin` e `User`).
3. **Padrão Repository:** Implementação assíncrona específica por entidade para evitar overload.
4. **Mapeamento:** Camada de serviço desacoplada do banco utilizando mapeamento manual robusto e DTOs específicos.
5. **Tratamento de Exceções:** Middleware global que captura erros de domínio e retorna códigos HTTP semânticos (`400 BadRequest`, `401 Unauthorized`, `403 Forbidden`, `404 NotFound`, `409 Conflict`).
6. **Regras de Transação:** Validação de 8 regras na autorização da transação financeira (duplicidade no mesmo comerciante, valor máximo, limite de intervalo de 1 minuto, etc.), com envio simulado de notificações para o usuário e comerciante.

---

## ☁️ Publicação no Azure (Instruções Rápidas)

1. **Azure SQL Database:** Crie um banco SQL no Azure sob a tier básica.
2. **Azure App Service (API):** Crie um Web App Windows ou Linux (.NET 8). Adicione a connection string do banco em `ConnectionStrings__DefaultConnection` e configure as chaves JWT nas configurações de aplicativo (`Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`).
3. **Azure App Service (Web MVC):** Crie um Web App para o MVC e defina a variável `ApiBaseUrl` apontando para a URL da API publicada.


