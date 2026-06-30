# MusicStreamer 🎵

Projeto acadêmico de plataforma de streaming de música desenvolvido para a disciplina de **Desenvolvimento de Sistemas com .NET** do Instituto INFNET.

## Tecnologias

| Camada | Tecnologia |
|--------|-----------|
| API | ASP.NET Core 8 Web API |
| Auth | JWT Bearer Tokens |
| ORM | Entity Framework Core 8 |
| Banco | SQL Server (LocalDB) |
| Frontend | ASP.NET Core MVC 8 |
| Testes | xUnit + Moq |
| Docs | Swagger / OpenAPI |

## Estrutura da Solução

```
MusicStreamer/
├── src/
│   ├── MusicStreamer.Api/             ← Controllers, JWT, Swagger, Middlewares
│   ├── MusicStreamer.Application/     ← Services, DTOs, Interfaces
│   ├── MusicStreamer.Domain/          ← Entidades, Enums, Exceções, IRepositories
│   ├── MusicStreamer.Infrastructure/  ← EF Core, Repositórios, Migrations
│   └── MusicStreamer.Web/             ← Frontend MVC
├── tests/
│   └── MusicStreamer.Tests/           ← xUnit + Moq
├── .gitignore
├── MusicStreamer.http
└── README.md
```

## Executando Localmente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (incluído no Visual Studio 2022)
- [dotnet-ef tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

### 1. Clonar o repositório

```bash
git clone https://github.com/AmorimTorres/amplifike.stream
cd amplifike_musica
```

### 2. Instalar ferramentas EF

```bash
dotnet tool install --global dotnet-ef --version 8.0.11
```

### 3. Criar o banco de dados

```bash
dotnet ef database update \
  --project src/MusicStreamer.Infrastructure \
  --startup-project src/MusicStreamer.Api
```

Este comando criará o banco `MusicStreamerDb` no LocalDB e aplicará os seeds com:
- 3 planos de assinatura (Gratuito, Premium, Família)
- 1 usuário administrador (`admin@musicstreamer.com` / `Admin@123`)
- 4 bandas com álbuns e músicas

### 4. Executar a API

```bash
cd src/MusicStreamer.Api
dotnet run
```

Swagger disponível em: **https://localhost:7001/swagger**

### 5. Executar o Frontend MVC

Em outro terminal:

```bash
cd src/MusicStreamer.Web
dotnet run
```

Frontend disponível em: **https://localhost:7002**

### 6. Executar os testes

```bash
dotnet test tests/MusicStreamer.Tests
```

---

## Endpoints da API

### Autenticação
```
POST /api/auth/register     - Registrar usuário
POST /api/auth/login        - Autenticar
GET  /api/auth/me           - Perfil do usuário autenticado
```

### Planos e Assinaturas
```
GET  /api/subscription-plans         - Listar planos
POST /api/subscriptions              - Assinar plano [Auth]
GET  /api/subscriptions/current      - Assinatura atual [Auth]
```

### Bandas
```
GET    /api/bands               - Listar todas
GET    /api/bands/{id}          - Detalhes
GET    /api/bands/search        - Buscar (paginado)
POST   /api/bands               - Criar [Admin]
PUT    /api/bands/{id}          - Atualizar [Admin]
DELETE /api/bands/{id}          - Excluir [Admin]
```

### Álbuns
```
GET  /api/albums         - Listar
GET  /api/albums/{id}    - Detalhes
POST /api/albums         - Criar [Admin]
```

### Músicas
```
GET  /api/musics              - Listar todas
GET  /api/musics/{id}         - Detalhes
GET  /api/musics/search       - Buscar (paginado)
POST /api/musics              - Criar [Admin]
PUT  /api/musics/{id}         - Atualizar [Admin]
```

### Busca Global
```
GET /api/search?term=texto&page=1&pageSize=10
```

### Playlists
```
GET    /api/playlists                          - Minhas playlists [Auth]
GET    /api/playlists/{id}                     - Detalhes [Auth]
POST   /api/playlists                          - Criar [Auth]
DELETE /api/playlists/{id}                     - Excluir [Auth]
POST   /api/playlists/{pid}/musics/{mid}       - Adicionar música [Auth]
DELETE /api/playlists/{pid}/musics/{mid}       - Remover música [Auth]
```

### Favoritos
```
GET    /api/favorites/musics           - Músicas favoritas [Auth]
POST   /api/favorites/musics/{id}      - Favoritar música [Auth]
DELETE /api/favorites/musics/{id}      - Desfavoritar [Auth]
GET    /api/favorites/bands            - Bandas favoritas [Auth]
POST   /api/favorites/bands/{id}       - Favoritar banda [Auth]
DELETE /api/favorites/bands/{id}       - Desfavoritar [Auth]
```

### Transações
```
POST /api/transactions/authorize   - Autorizar transação [Auth]
GET  /api/transactions             - Minhas transações [Auth]
GET  /api/transactions/{id}        - Detalhes [Auth]
```

---

## Publicação no Azure

### 1. Azure SQL Database

No [portal Azure](https://portal.azure.com):

1. Criar um **Azure SQL Server** e um **Azure SQL Database** (tier Basic para fins acadêmicos)
2. Configurar a regra de firewall para permitir acesso do Azure App Service
3. Obter a connection string na aba **Connection strings** do banco

### 2. Azure App Service – API

```bash
# Publicar via CLI Azure
az webapp up \
  --name musicstreamer-api \
  --resource-group rg-musicstreamer \
  --sku B1 \
  --runtime "dotnet:8"

# Ou via Visual Studio: clique direito no projeto → Publish → Azure App Service
```

### 3. Configurar a Connection String no Azure

No App Service → **Configuration** → **Application settings**:

```
ConnectionStrings__DefaultConnection = Server=tcp:<server>.database.windows.net,1433;Database=MusicStreamerDb;User ID=<user>;Password=<pass>;Encrypt=True;
```

### 4. Configurar o JWT no Azure

Em **Configuration** → **Application settings**:
```
Jwt__Key       = <sua-chave-secreta-de-producao>
Jwt__Issuer    = https://musicstreamer-api.azurewebsites.net
Jwt__Audience  = https://musicstreamer-web.azurewebsites.net
```

> ⚠️ Nunca coloque segredos no `appsettings.json` em produção! Use **Azure Key Vault** ou variáveis de ambiente.

### 5. Executar Migrations no Azure

Após o deploy, via **Kudu Console** (App Service → Advanced Tools → Bash):

```bash
dotnet ef database update \
  --project MusicStreamer.Infrastructure.dll \
  --startup-project MusicStreamer.Api.dll
```

Ou adicione no `Program.cs` (apenas para ambientes de staging):
```csharp
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<MusicStreamerDbContext>();
db.Database.Migrate();
```

### 6. Azure App Service – Frontend MVC

```bash
az webapp up \
  --name musicstreamer-web \
  --resource-group rg-musicstreamer \
  --sku B1 \
  --runtime "dotnet:8"
```

Configure `ApiBaseUrl` nas Application Settings do App Service do frontend:
```
ApiBaseUrl = https://musicstreamer-api.azurewebsites.net
```

### 7. Logs no Azure

```bash
az webapp log tail --name musicstreamer-api --resource-group rg-musicstreamer
```

Ou: App Service → **Log stream** no portal Azure.

### 8. Azure Blob Storage (Futura Implementação)

Para armazenar capas de álbuns e arquivos de áudio:

1. Criar uma **Storage Account** com um container `album-covers` e `audio-files`
2. Instalar: `Azure.Storage.Blobs`
3. Criar `IBlobStorageService` com upload/download
4. Adicionar campos `CoverImageUrl` e `AudioFileUrl` nas entidades `Album` e `Music`
5. Configurar CORS no Storage Account para o domínio da API

```csharp
// Exemplo de interface
public interface IBlobStorageService
{
    Task<string> UploadAsync(string containerName, string fileName, Stream fileStream);
    Task<bool> DeleteAsync(string containerName, string fileName);
}
```

---

## Credenciais Padrão (Desenvolvimento)

| Campo | Valor |
|-------|-------|
| Admin e-mail | `admin@musicstreamer.com` |
| Admin senha | `Admin@123` |
| API Swagger | https://localhost:7001/swagger |
| Frontend | https://localhost:7002 |

---

## Decisões de Arquitetura

- **JWT puro** (sem ASP.NET Core Identity): maior controle e transparência para fins acadêmicos
- **Repository Pattern simples**: repositórios específicos por entidade, sem Generic Repository
- **AsNoTracking()** em todas as consultas de leitura para melhor performance
- **Projeção direta** nos repositórios evita N+1 via ThenInclude
- **Índices** nas colunas `Band.Name` e `Music.Title` para performance de busca
- **CancellationToken** em todos os métodos assíncronos
- **Middleware global** para tratamento de exceções de domínio → HTTP status codes
