namespace Zava.Models;

public sealed record Product(
	string Id,
	string Name,
	string Description,
	decimal Price,
	string ImageName,
	string Category,
	bool InStock,
	IReadOnlyList<string> Keywords);

public sealed record SearchResponse(
	string Query,
	bool UseSemanticSearch,
	string Engine,
	IReadOnlyList<Product> Products);
