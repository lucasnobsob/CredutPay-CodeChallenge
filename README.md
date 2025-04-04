# CredutPay-CodeChallenge

## Stack Técnica
- .NET 8.0
- .NET WebApi
- .NET Identity
- Entity Framework 8.0
- .NET Core Native DI
- AutoMapper
- FluentValidator
- MediatR
- Swagger UI
- MSSQL
- xUnit
- Moq
- Fluent Assertions
- Polly
- Refit

## Design Patterns
- Domain Driven Design
- Domain Events
- Domain Notification
- CQRS
- Event Sourcing
- Unit Of Work
- Repository & Generic Repository
- Inversion of Control / Dependency injection
- ORM
- Mediator
- Specification Pattern
- Options Pattern

Este projeto é composto por:

- **Backend:** API desenvolvida em .NET 8
- **Frontend:** Aplicação React + TypeScript + Tailwind CSS com Next.js

---

## 🚀 Como rodar o projeto

### ✅ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js LTS](https://nodejs.org/)
- [Yarn](https://classic.yarnpkg.com/lang/en/) ou [npm](https://www.npmjs.com/)
- [Docker](https://www.docker.com/) (caso deseje usar containers)

---

## 🐳 Rodando o projeto com Docker

> ⚠️ Apenas a API será executada no Docker. O frontend deverá ser iniciado manualmente.

1. Clone o repositório:
   ```bash
   git clone https://github.com/lucasnobsob/CredutPay-CodeChallenge.git
   cd seu-repositorio
   ```

2. Navegue até a pasta do backend:
   ```bash
   cd backend
   ```

3. Construa e suba os containers:
   ```bash
   docker-compose up
   ```

> A API estará disponível em `http://localhost:8080`

4. Em outro terminal, inicie o frontend manualmente:
   ```bash
   cd ../frontend
   yarn install
   yarn dev
   ```

> O frontend estará rodando em `http://localhost:3000`

---

## 🧪 Rodando o projeto sem Docker

1. Clone o repositório:
   ```bash
   git clone https://github.com/lucasnobsob/CredutPay-CodeChallenge.git
   cd seu-repositorio
   ```

### 🔧 Rodando o Backend (.NET 8)

2. Vá para a pasta do backend:
   ```bash
   cd backend
   ```

3. Restaure os pacotes e rode a aplicação:
   ```bash
   dotnet restore
   dotnet run
   ```

> A API estará disponível em `http://localhost:44376` conforme especificado no `launchSettings.json`.

### 🎨 Rodando o Frontend (React + Next.js)

4. Vá para a pasta do frontend:
   ```bash
   cd ../frontend
   ```

5. Instale as dependências:
   ```bash
   yarn install
   ```

6. Inicie o servidor de desenvolvimento:
   ```bash
   yarn dev
   ```

> A aplicação estará disponível em `http://localhost:3000`

---

## 🛠️ Variáveis de ambiente

- Existe um arquivo `.env` na raiz da pasta `frontend`
- Edite conforme necessário, por exemplo:

```env
# frontend/.env.local
NEXT_PUBLIC_API_URL=http://localhost:8080
```

---

## 📄 Licença

Este projeto está licenciado sob a [GNU License](LICENSE).
