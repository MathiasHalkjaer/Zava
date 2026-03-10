using Microsoft.AspNetCore.Http.HttpResults;
using Zava.Models;

namespace Products.Endpoints;

public static class ProductActions
{
    public static IEndpointRouteBuilder MapProductActions(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");

        group.MapGet("/products", GetProducts)
            .WithName("GetProducts");

        group.MapGet("/search", Search)
            .WithName("SearchProducts");

        return app;
    }

    public static Ok<IReadOnlyList<Product>> GetProducts()
    {
        return TypedResults.Ok(ProductCatalog.All);
    }

    public static Ok<SearchResponse> Search(string search)
    {
        var normalized = search?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return TypedResults.Ok(new SearchResponse(string.Empty, false, "keyword", []));
        }

        var matches = ProductCatalog.All
            .Where(product =>
                product.Name.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                product.Description.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                product.Keywords.Any(keyword => keyword.Contains(normalized, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        return TypedResults.Ok(new SearchResponse(normalized, false, "keyword", matches));
    }
}