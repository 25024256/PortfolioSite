# My Portfolio

Welcome to my web development portfolio! 
👉 [Click here to view my live website](https://25024256.github.io/PortfolioSite/)

## Frontend 


## Backend API Specifications
This repository also contains the backend API for my portfolio.

- **Framework:** .NET 10.0 ASP.NET Core Web API
- **API Style & Architecture:** Minimal APIs structured according to Vertical Slice Architecture (divided into `Features/Projects` and `Features/Blogposts`).
- **Database:** SQL Server via Entity Framework Core.

### Database Configuration & Creation
1. Ensure SQL Server (Express) is running locally.
2. The connection string is located in `appsettings.json` under `DefaultConnection` and points to the local database `PortfolioDB`.
3. The tables are managed via EF Core Migrations. Run the following command to create the database:
   ```bash
   dotnet ef database update


## Backend API (.NET 10)

RESTful Web API gebouwd met **.NET 10**, **Entity Framework Core (SQL Server)** en **ASP.NET Core Identity** met rolgebaseerde autorisatie (RBAC).

### Projectstructuur (`Backend/`)

```text
Backend/
├── Backend.sln        # Solution-bestand voor de backend
├── PortfolioAPI/      # Minimal API met Projects, Blogposts en Identity Auth
└── Tests/             # xUnit integratietests (32 tests)