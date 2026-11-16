namespace PortfolioAPI.Features.Blogposts;

public record BlogpostDto(
    int Id,
    string Titel, 
    string Inhoud, 
    DateTime Datum
);

public record BlogpostRequestDto(
    string Titel, 
    string Inhoud, 
    DateTime Datum
);