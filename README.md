# My Portfolio

Welcome to my web development portfolio!  
👉 [Click here to view my live website](https://25024256.github.io/PortfolioSite/)

## Frontend

Single Page Application (SPA) in ontwikkeling in de map `Frontend/`.

---

## Backend API (.NET 10)

RESTful Web API gebouwd met **.NET 10**, **Entity Framework Core (SQL Server)** en **ASP.NET Core Identity** met rolgebaseerde autorisatie (RBAC), opgezet volgens **Vertical Slice Architecture** (`Projects`, `Blogposts` en `Auth`).

### Projectstructuur (`Backend/`)

```text
Backend/
├── Backend.slnx       # Solution-bestand voor de backend
├── PortfolioAPI/      # Minimal API met Projects, Blogposts en Identity Auth
└── Tests/             # xUnit integratietests (32 tests)
```
## Database, Migraties & API Opstarten
1. Controleer of SQL Server (Express) lokaal draait. De connection string (DefaultConnection) staat in Backend/PortfolioAPI/appsettings.json en verwijst standaard naar de lokale database PortfolioDB.

2. Open een terminal in Backend/PortfolioAPI en pas de EF Core- en Identity-migraties toe:

```text
PowerShell
cd Backend/PortfolioAPI
dotnet ef database update
```

3. Start de API:

```text
PowerShell
dotnet run
```
4. Open Swagger UI in de browser via http://localhost:5255/swagger.

## Rollen & Gebruikersbeheer (Gast, Editor, Admin)
Registratie (Gast): Nieuwe gebruikers kunnen zich registreren via POST /api/auth/register en krijgen automatisch de rol Gast. Deze rol heeft alleen leesrechten (GET). Publieke registratie kent nooit de rol Editor of Admin toe.

Inloggen (Cookies): Inloggen verloopt via POST /api/auth/login?useCookies=true, waarna de API een httpOnly authenticatiecookie teruggeeft.

Editor & Admin rechten: Aanmaken, wijzigen en verwijderen (POST, PUT, DELETE) op /api/projects en /api/blogposts is uitsluitend toegestaan voor gebruikers met de rol Editor of Admin. Zonder geldige sessie geeft de API 401 Unauthorized; zonder vereiste rol geeft de API 403 Forbidden.

Lokaal toekennen van rollen:

Bij het opstarten maakt DbSeeder automatisch de rollen Gast, Editor en Admin aan en voorziet lokale testaccounts van deze rollen.

Daarnaast kan een rol handmatig in de lokale SQL Server-database worden toegekend door in de tabel AspNetUserRoles het UserId (uit AspNetUsers) te koppelen aan het RoleId van Editor of Admin (uit AspNetRoles). Na een rolwijziging moet de gebruiker opnieuw inloggen.

## API Endpoints
Authentication: POST /api/auth/register, POST /api/auth/login?useCookies=true, POST /api/auth/refresh, GET /api/auth/manage/info

Projects: GET /api/projects, GET /api/projects/{id} (publiek) | POST, PUT, DELETE /api/projects (Editor / Admin)

Blogposts: GET /api/blogposts, GET /api/blogposts/{id} (publiek) | POST, PUT, DELETE /api/blogposts (Editor / Admin)

## Geautomatiseerde Tests uitvoeren (dotnet test)
De integratietests staan in het afzonderlijke testproject Backend/Tests (xUnit + WebApplicationFactory) en zijn opgezet volgens het Arrange-Act-Assert (AAA) patroon. Voer alle 32 tests uit vanuit de map Backend:

```text
PowerShell
cd Backend
dotnet test
```
