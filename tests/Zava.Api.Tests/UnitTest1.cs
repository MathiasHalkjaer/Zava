using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;
using Products.Endpoints;
using Zava.AI;
using Zava.Models;

namespace Zava.Api.Tests;

public sealed class ProductAiActionsTests
{
    [Fact]
    public async Task AiSearch_ReturnsPaintProducts_WhenEmbeddingSearchRanksByMeaning()
    {
        var service = new ProductSearchService(new FakeEmbeddingClient(), NullLogger<ProductSearchService>.Instance);

        var result = await ProductAiActions.AISearch("paint", service, CancellationToken.None);

        var payload = Assert.IsType<Ok<SearchResponse>>(result);
        Assert.Equal("semantic", payload.Value!.Engine);
        Assert.Contains(payload.Value.Products, product => product.Name == "Interior Wall Paint - White Matte");
        Assert.Contains(payload.Value.Products, product => product.Name == "Painter's Roller Kit");
    }

    [Fact]
    public async Task AiSearch_FallsBackToKeywordMode_WhenEmbeddingsAreUnavailable()
    {
        var service = new ProductSearchService(new EmptyEmbeddingClient(), NullLogger<ProductSearchService>.Instance);

        var result = await ProductAiActions.AISearch("drill", service, CancellationToken.None);

        var payload = Assert.IsType<Ok<SearchResponse>>(result);
        Assert.Equal("keyword-fallback", payload.Value!.Engine);
        Assert.Equal("Cordless Drill Kit", Assert.Single(payload.Value.Products).Name);
    }

    private sealed class EmptyEmbeddingClient : IEmbeddingClient
    {
        public Task<IReadOnlyList<float>> CreateEmbeddingAsync(string input, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<float>>([]);
        }
    }

    private sealed class FakeEmbeddingClient : IEmbeddingClient
    {
        public Task<IReadOnlyList<float>> CreateEmbeddingAsync(string input, CancellationToken cancellationToken = default)
        {
            var value = input.ToLowerInvariant();

            if (value.Contains("paint") || value.Contains("roller") || value.Contains("wall"))
            {
                return Task.FromResult<IReadOnlyList<float>>([1f, 0f, 0f]);
            }

            if (value.Contains("drill"))
            {
                return Task.FromResult<IReadOnlyList<float>>([0f, 1f, 0f]);
            }

            return Task.FromResult<IReadOnlyList<float>>([0f, 0f, 1f]);
        }
    }
}