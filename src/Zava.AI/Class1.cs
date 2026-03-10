using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zava.Models;

namespace Zava.AI;

public sealed class EmbeddingSearchOptions
{
	public const string SectionName = "OpenAI";

	public string? Endpoint { get; init; }

	public string? ApiKey { get; init; }

	public string Model { get; init; } = "text-embeddings-ada-002";

	public bool IsConfigured =>
		!string.IsNullOrWhiteSpace(Endpoint) &&
		!string.IsNullOrWhiteSpace(ApiKey);
}

public interface IEmbeddingClient
{
	Task<IReadOnlyList<float>> CreateEmbeddingAsync(string input, CancellationToken cancellationToken = default);
}

public interface IProductSearchService
{
	Task<SearchResponse> SearchAsync(
		string query,
		IReadOnlyList<Product> products,
		bool useSemanticSearch,
		CancellationToken cancellationToken = default);
}

public sealed class ProductSearchService(IEmbeddingClient embeddingClient, ILogger<ProductSearchService> logger) : IProductSearchService
{
	public async Task<SearchResponse> SearchAsync(
		string query,
		IReadOnlyList<Product> products,
		bool useSemanticSearch,
		CancellationToken cancellationToken = default)
	{
		var normalizedQuery = query.Trim();
		if (string.IsNullOrWhiteSpace(normalizedQuery))
		{
			return new SearchResponse(string.Empty, useSemanticSearch, useSemanticSearch ? "semantic" : "keyword", []);
		}

		return useSemanticSearch
			? await SemanticSearchAsync(normalizedQuery, products, cancellationToken)
			: KeywordSearch(normalizedQuery, products);
	}

	private async Task<SearchResponse> SemanticSearchAsync(
		string query,
		IReadOnlyList<Product> products,
		CancellationToken cancellationToken)
	{
		var queryEmbedding = await embeddingClient.CreateEmbeddingAsync(query, cancellationToken);
		if (queryEmbedding.Count == 0)
		{
			logger.LogInformation("Semantic search configuration is missing or unavailable. Falling back to keyword search.");
			return KeywordSearch(query, products, "keyword-fallback");
		}

		var scored = new List<(Product Product, double Score)>();
		foreach (var product in products)
		{
			var productEmbedding = await embeddingClient.CreateEmbeddingAsync(BuildSearchDocument(product), cancellationToken);
			if (productEmbedding.Count == 0)
			{
				return KeywordSearch(query, products, "keyword-fallback");
			}

			scored.Add((product, CosineSimilarity(queryEmbedding, productEmbedding)));
		}

		var matches = scored
			.OrderByDescending(item => item.Score)
			.ThenBy(item => item.Product.Name, StringComparer.OrdinalIgnoreCase)
			.Take(6)
			.Select(item => item.Product)
			.ToArray();

		return new SearchResponse(query, true, "semantic", matches);
	}

	private static SearchResponse KeywordSearch(string query, IReadOnlyList<Product> products, string engine = "keyword")
	{
		var tokens = query
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(token => token.ToLowerInvariant())
			.ToArray();

		var matches = products
			.Select(product => new { Product = product, Score = ScoreKeyword(product, tokens) })
			.Where(item => item.Score > 0)
			.OrderByDescending(item => item.Score)
			.ThenBy(item => item.Product.Name, StringComparer.OrdinalIgnoreCase)
			.Select(item => item.Product)
			.ToArray();

		return new SearchResponse(query, false, engine, matches);
	}

	private static int ScoreKeyword(Product product, IReadOnlyList<string> tokens)
	{
		var text = BuildSearchDocument(product).ToLowerInvariant();
		return tokens.Sum(token =>
			text.Contains(token, StringComparison.Ordinal)
				? product.Name.Contains(token, StringComparison.OrdinalIgnoreCase) ? 5 : 2
				: 0);
	}

	private static string BuildSearchDocument(Product product)
	{
		var builder = new StringBuilder();
		builder.Append(product.Name).Append(' ')
			.Append(product.Description).Append(' ')
			.Append(product.Category).Append(' ')
			.AppendJoin(' ', product.Keywords);

		return builder.ToString();
	}

	private static double CosineSimilarity(IReadOnlyList<float> left, IReadOnlyList<float> right)
	{
		if (left.Count == 0 || right.Count == 0 || left.Count != right.Count)
		{
			return 0;
		}

		double dotProduct = 0;
		double leftMagnitude = 0;
		double rightMagnitude = 0;

		for (var index = 0; index < left.Count; index++)
		{
			dotProduct += left[index] * right[index];
			leftMagnitude += left[index] * left[index];
			rightMagnitude += right[index] * right[index];
		}

		if (leftMagnitude == 0 || rightMagnitude == 0)
		{
			return 0;
		}

		return dotProduct / (Math.Sqrt(leftMagnitude) * Math.Sqrt(rightMagnitude));
	}
}

public sealed class OpenAiEmbeddingClient(
	HttpClient httpClient,
	IOptions<EmbeddingSearchOptions> options,
	ILogger<OpenAiEmbeddingClient> logger) : IEmbeddingClient
{
	private readonly EmbeddingSearchOptions _options = options.Value;

	public async Task<IReadOnlyList<float>> CreateEmbeddingAsync(string input, CancellationToken cancellationToken = default)
	{
		if (!_options.IsConfigured)
		{
			return [];
		}

		using var request = new HttpRequestMessage(HttpMethod.Post, _options.Endpoint!.TrimEnd('/') + "/embeddings")
		{
			Content = JsonContent.Create(new EmbeddingRequest(_options.Model, input))
		};
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

		using var response = await httpClient.SendAsync(request, cancellationToken);
		if (!response.IsSuccessStatusCode)
		{
			logger.LogWarning("Embedding request failed with status code {StatusCode}.", response.StatusCode);
			return [];
		}

		var payload = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(cancellationToken: cancellationToken);
		return payload?.Data.FirstOrDefault()?.Embedding ?? [];
	}

	private sealed record EmbeddingRequest(string Model, string Input);

	private sealed record EmbeddingResponse([property: JsonPropertyName("data")] IReadOnlyList<EmbeddingItem> Data);

	private sealed record EmbeddingItem([property: JsonPropertyName("embedding")] IReadOnlyList<float> Embedding);
}
