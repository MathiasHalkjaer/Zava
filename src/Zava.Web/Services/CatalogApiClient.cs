using System.Net.Http.Json;
using System.Text.Json;
using Zava.Models;

namespace Zava.Web.Services;

public interface ICatalogApiClient
{
    Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken = default);

    Task<SearchResponse> SearchAsync(string query, bool useSemanticSearch, CancellationToken cancellationToken = default);
}

public sealed class CatalogApiClient(HttpClient httpClient) : ICatalogApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await httpClient.GetFromJsonAsync<IReadOnlyList<Product>>("api/products", JsonOptions, cancellationToken);
        return products ?? [];
    }

    public async Task<SearchResponse> SearchAsync(string query, bool useSemanticSearch, CancellationToken cancellationToken = default)
    {
        var path = useSemanticSearch
            ? $"api/search/ai?search={Uri.EscapeDataString(query)}"
            : $"api/search?search={Uri.EscapeDataString(query)}";

        var response = await httpClient.GetFromJsonAsync<SearchResponse>(path, JsonOptions, cancellationToken);
        return response ?? new SearchResponse(query, useSemanticSearch, useSemanticSearch ? "semantic" : "keyword", []);
    }
}