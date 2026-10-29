using Microsoft.EntityFrameworkCore;
using PortfolioAPI.Data;

namespace PortfolioAPI.Features.Blogposts;

public static class BlogpostEndpoints
{
    public static void MapBlogpostEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/blogposts").WithTags("Blogposts");

        // GET all blogposts, 200 OK
        group.MapGet("/", async (PortfolioContext db) =>
        {
            var blogpost = await db.Blogposts.ToListAsync();

            var dtos = blogpost.Select(p => new BlogpostDto(
                p.Id, p.Titel, p.Inhoud, p.Publicatiedatum
            ));

            return Results.Ok(dtos);
        });

        // GET on Id and on 1 blogpost, 200 OK or 404 NotFound
        group.MapGet("/{id:int}", async (int id, PortfolioContext db) =>
        {
            var b = await db.Blogposts.FindAsync(id);
            if (b is null) return Results.NotFound();

            var dto = new BlogpostDto(
                b.Id, b.Titel, b.Inhoud, b.Publicatiedatum
            );
            return Results.Ok(dto);
        });


        // POST new blogpost, 201 created
        group.MapPost("/", async (BlogpostRequestDto request, PortfolioContext db) =>
        {
            var nieuweBlogpost = new Blogpost
            {
                Titel = request.Titel,
                Inhoud = request.Inhoud,
                Publicatiedatum = request.Datum
            };

            db.Blogposts.Add(nieuweBlogpost);
            await db.SaveChangesAsync();

            var responseDto = new BlogpostDto(
                nieuweBlogpost.Id, nieuweBlogpost.Titel, nieuweBlogpost.Inhoud, nieuweBlogpost.Publicatiedatum
            );
            return Results.Created($"/api/blogposts/{nieuweBlogpost.Id}", responseDto);
        });

        // PUT edit blogpost, 204 NoContent or 404 NotFound
        group.MapPut("/{id:int}", async (int id, BlogpostRequestDto request, PortfolioContext db)=>
        {
            var bestaandeBlogpost = await db.Blogposts.FindAsync(id);
            if (bestaandeBlogpost is null) return Results.NotFound();

            bestaandeBlogpost.Titel = request.Titel;
            bestaandeBlogpost.Inhoud = request.Inhoud;
            bestaandeBlogpost.Publicatiedatum = request.Datum;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // DELETE blogpost delete, 204 NoContent of 404 NotFound 
        group.MapDelete("/{id:int}", async (int id, PortfolioContext db) =>
        {
            var blogpost = await db.Blogposts.FindAsync(id);
            if (blogpost is null) return Results.NotFound();

            db.Blogposts.Remove(blogpost);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}