namespace PortfolioAPI.Features.Projects;

public record ProjectDto(
    int Id, 
    string Titel, 
    string Beschrijving, 
    string Categorie, 
    string GitHubUrl, 
    DateTime Datum
);

public record ProjectRequestDto(
    string Titel, 
    string Beschrijving, 
    string Categorie, 
    string GitHubUrl, 
    DateTime Datum
);