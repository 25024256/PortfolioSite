using Microsoft.EntityFrameworkCore;
using PortfolioAPI.Data;

namespace PortfolioAPI.Features.Projects;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects").WithTags("Projects");

        //GET all projects, 200 OK 
        group.MapGet("/", async (PortfolioContext db) =>
        {
            var projects = await db.Projects.ToListAsync();

            var dtos = projects.Select(p => new ProjectDto(
                p.Id, p.Titel, p.Beschrijving, p.Categorie, p.GitHubUrl, p.Datum
            ));

            return Results.Ok(dtos);
        });

        //GET on Id and on 1 project, 200 OK or 404 NotFound
        group.MapGet("/{id:int}", async (int id, PortfolioContext db) =>
        {
            var p = await db.Projects.FindAsync(id);
            if (p is null) return Results.NotFound();

            var dto = new ProjectDto(
                p.Id, p.Titel, p.Beschrijving, p.Categorie, p.GitHubUrl, p.Datum
            );
            return Results.Ok(dto);
        });


        // POST new project, 201 created
        group.MapPost("/", async (ProjectRequestDto request, PortfolioContext db) =>
        {
            var nieuwProject = new Project
            {
                Titel = request.Titel,
                Beschrijving = request.Beschrijving,
                Categorie = request.Categorie,
                GitHubUrl = request.GitHubUrl,
                Datum = request.Datum
            };

            db.Projects.Add(nieuwProject);
            await db.SaveChangesAsync();

            var responseDto = new ProjectDto(
                nieuwProject.Id, nieuwProject.Titel, nieuwProject.Beschrijving, nieuwProject.Categorie, nieuwProject.GitHubUrl, nieuwProject.Datum
            );
            return Results.Created($"/api/projects/{nieuwProject.Id}", responseDto);
        });

        //PUT edit project, 204 NoContent or 404 NotFound
        group.MapPut("{id:int}", async (int id, ProjectRequestDto request, PortfolioContext db)=>
        {
            var bestaandProject = await db.Projects.FindAsync(id);
            if (bestaandProject is null) return Results.NotFound();

            bestaandProject.Titel = request.Titel;
            bestaandProject.Beschrijving = request.Beschrijving;
            bestaandProject.Categorie = request.Categorie;
            bestaandProject.GitHubUrl = request.GitHubUrl;
            bestaandProject.Datum = request.Datum;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // DELETE project delete, 204 NoContent of 404 NotFound 
        group.MapDelete("/{id:int}", async (int id, PortfolioContext db) =>
        {
            var project = await db.Projects.FindAsync(id);
            if (project is null) return Results.NotFound();

            db.Projects.Remove(project);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}