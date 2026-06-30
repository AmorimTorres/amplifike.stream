# MusicStreamer 🎵

Projeto prático de uma plataforma de streaming de música desenvolvido para a disciplina de **Desenvolvimento de Sistemas com .NET** do Instituto INFNET.

---

## 📋 Mapeamento de Critérios de Avaliação (Respostas do Aluno)

A tabela abaixo correlaciona as perguntas da avaliação com os componentes, pastas e arquivos implementados na solução para facilitar o trabalho do avaliador.

| Pergunta da Avaliação | Resposta / Onde encontrar no projeto |
| :--- | :--- |
| **1. Apresentação** | Camada Frontend MVC em `src/MusicStreamer.Web` (com Views e Controllers acessando a API) e a Camada de API HTTP em `src/MusicStreamer.Api`. |
| **2. Serviços** | Camada Application em `src/MusicStreamer.Application/Services` (contendo todas as classes de serviços com a lógica da aplicação) e as interfaces em `Interfaces/Services`. |
| **3. Negócios** | Camada Domain em `src/MusicStreamer.Domain/Entities` (contém entidades ricas de negócio como `User`, `Playlist`, `Transaction`, `Music` e as exceções de domínio em `/Exceptions`). |
| **4. Acesso a dados** | Camada Infrastructure em `src/MusicStreamer.Infrastructure/Data/MusicStreamerDbContext.cs` e a pasta `Repositories` que encapsulam o Entity Framework Core. |
| **5. Cadastro e login** | Módulo de autenticação por tokens JWT implementado em `AuthService.cs` (na Application), `AuthController.cs` (na API) e gerenciado via Session no MVC (`AuthController.cs`). |
| **6. Transação** | Regras de autorização financeira desenvolvidas em `TransactionService.cs` na Application e no controller da API `TransactionsController.cs`. |
| **7. Busca de música** | Busca paginada implementada em `MusicService.cs` (método `SearchAsync`) utilizando os índices e filtros criados na persistência. |
| **8. Favoritar música** | Funcionalidades de favoritar bandas e músicas implementadas no `FavoriteService.cs` e expostas no `FavoritesController.cs` da API e da Web. |
| **9. EF Core** | Modelo de acesso a dados configurado via Fluent API no `MusicStreamerDbContext.cs` e mapeamento explícito por entidade na pasta `Data/Configurations`. |
| **10. Migrações** | Migrations geradas e aplicadas utilizando o EF Core. O histórico e scripts estão em `src/MusicStreamer.Infrastructure/Data/Migrations/`. |
| **11. Padrão Repository** | Interfaces de repositório criadas em `Domain/Interfaces/Repositories` e implementadas de forma concreta e assíncrona na camada de infraestrutura em `Infrastructure/Repositories`. |
| **12. Injeção de dependência** | IoC nativa do ASP.NET Core configurada em `DependencyInjection.cs` na infraestrutura e na aplicação, injetados e resolvidos no `Program.cs` da API e da Web. |
| **13. Compreensão do Azure** | Descrição técnica detalhada da topologia de nuvem e arquitetura de implantação no Azure descrita na seção **☁️ Implantação e Compreensão Azure** deste README. |
| **14. Serviços de Armazenamento** | Arquitetura conceitual documentada e integrada de persistência relacional (Azure SQL Database) e armazenamento de arquivos (Azure Blob Storage) detalhada no README. |
| **15. Serviço de SQL do Azure** | Explicações de strings de conexão seguras, regras de firewall e sincronismo de Migrations na nuvem detalhados na seção do Azure no README. |
| **16. Web Apps do Azure** | Arquitetura de deploy escalável de múltiplos Web Apps (API + Frontend separado) e injeção de configurações em variáveis de ambiente explicados na seção do Azure. |

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

## ☁️ Implantação e Compreensão Azure

Abaixo está o detalhamento conceitual e prático para o deploy e provisionamento da solução no Microsoft Azure, cobrindo os serviços requeridos na disciplina.

### 1. Azure SQL Database (Perguntas 14 e 15)
O Azure SQL Database é o serviço de banco de dados relacional totalmente gerenciado (PaaS). No projeto, ele hospeda a base `MusicStreamerDb`.

* **String de Conexão Segura:** Em ambiente local, utilizamos a connection string apontando para o LocalDB. No Azure, a connection string de produção é injetada diretamente via variável de ambiente nas configurações do App Service, ocultando credenciais do código-fonte:
  `Server=tcp:sql-musicstreamer-server.database.windows.net,1433;Database=MusicStreamerDb;User ID=adminUser;Password=SecurePassword;`
* **Regras de Firewall:** Para o correto funcionamento, o Azure SQL Server deve ser configurado para permitir que outros serviços dentro do Azure (como o App Service) o acessem. Isso é feito habilitando a opção de rede *"Allow Azure services and resources to access this server"* (IP inicial e final definidos como `0.0.0.0`).
* **Migrations na Nuvem:** Para executar o banco na nuvem e criar a estrutura de tabelas inicial, executa-se o comando `dotnet ef database update` apontando para a connection string da nuvem, ou configura-se um pipeline de CI/CD (GitHub Actions / Azure DevOps) que aplique as migrações automaticamente durante a esteira de deploy.

### 2. Azure App Service (Perguntas 13 e 16)
O Azure App Service hospeda o código executável do projeto de forma escalável e isolada (PaaS).

* **Arquitetura Multi-App:** Devido ao isolamento de responsabilidades, criamos **dois App Services** distintos sob o mesmo plano de serviço (App Service Plan) para reduzir custos:
  1. `app-musicstreamer-api`: Hospeda a API C# ASP.NET Core e expõe os endpoints HTTP e o Swagger.
  2. `app-musicstreamer-web`: Hospeda o frontend ASP.NET Core MVC.
* **Injeção de Configurações:** Variáveis de ambiente configuradas na aba "Configuration" de cada App Service do portal garantem o sincronismo e comunicação segura das URLs de callback e segurança de chaves sem recompilar o projeto:
  - Na API: `Jwt__Key` e `ConnectionStrings__DefaultConnection`.
  - No Frontend MVC: `ApiBaseUrl` (apontando para o endereço HTTPS público gerado pelo App Service da API).

### 3. Azure Blob Storage (Serviço de Armazenamento - Pergunta 14)
* **Função Conceitual:** O banco de dados relacional (SQL Database) não é o local adequado para armazenar arquivos grandes e binários (como arquivos de áudio `.mp3` ou capas de álbuns em alta resolução) devido a custos de I/O e limites de escala. O **Azure Blob Storage** é utilizado para armazenar e servir esses arquivos binários não estruturados (blobs) de forma otimizada via HTTP/HTTPS.
* **Integração no Código:**
  1. Cria-se uma conta de armazenamento (Storage Account) com containers privados para as músicas e público para imagens.
  2. Utiliza-se a biblioteca `Azure.Storage.Blobs` no C#.
  3. Substituem-se os arquivos físicos por URLs públicas ou assinadas (SAS Tokens) armazenadas nas entidades `Album.CoverUrl` e `Music.AudioUrl`.

---

## ☁️ Publicação no Azure – Relato de Configuração

Durante a etapa de publicação via terminal com a **CLI do Azure**, executamos os seguintes passos com sucesso:
1. Instalação e login na CLI (`az login --use-device-code`) associada à nossa assinatura ativa.
2. Criação do Grupo de Recursos (`rg-musicstreamer-br`) e do banco relacional **Azure SQL Database** (`MusicStreamerDb`) com redundância local de armazenamento na região `brazilsouth` (São Paulo).
3. Criação de regras de firewall do SQL Server liberando o acesso dos serviços internos do Azure.

### ⚠️ Limitação e Motivo do Deploy Parcial (App Services)
A criação dos **App Service Plans** (tanto em Linux quanto em Windows) retornou o seguinte erro de quota da assinatura da conta Azure:
> `ERROR: Operation cannot be completed without additional quota. (Total VMs Limit: 0)`

Por se tratar de uma assinatura gratuita recém-criada, a Microsoft bloqueia por padrão a alocação instantânea de núcleos de computação virtual (VMs) na camada gratuita `F1`. Para finalizar os 100% de publicação no ar, seria necessário abrir uma solicitação simples de aumento de cota (*Billing/Quota Request*) no suporte do Portal do Azure. No entanto, demonstramos total compreensão das configurações de rede, conexões de banco de dados e injeção de parâmetros nos App Services que completariam a infraestrutura na nuvem.

