using Microsoft.AspNetCore.Http.HttpResults;
using Zava.AI;
using Zava.Models;

namespace Products.Endpoints;

public static class ProductAiActions
{
    public static IEndpointRouteBuilder MapProductAiActions(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/search/ai", AISearch)
            .WithName("AISearchProducts");

        return app;
    }

    public static async Task<Ok<SearchResponse>> AISearch(
        string search,
        IProductSearchService searchService,
        CancellationToken cancellationToken)
    {
        var result = await searchService.SearchAsync(search, ProductCatalog.All, true, cancellationToken);
        return TypedResults.Ok(result);
    }
}