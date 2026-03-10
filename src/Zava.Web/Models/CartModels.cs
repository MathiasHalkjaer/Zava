using Zava.Models;

namespace Zava.Web.Models;

public sealed record CartLine(string ProductId, int Quantity);

public sealed record CartLineViewModel(Product Product, int Quantity)
{
    public decimal LineTotal => Product.Price * Quantity;
}

public sealed class CartPageViewModel
{
    public required IReadOnlyList<CartLineViewModel> Items { get; init; }

    public bool WasCheckedOut { get; init; }

    public decimal Subtotal => Items.Sum(item => item.LineTotal);

    public decimal Tax => Math.Round(Subtotal * 0.08m, 2);

    public decimal Total => Subtotal + Tax;
}

public sealed class ProductPageViewModel
{
    public required IReadOnlyList<Product> Products { get; init; }
}

public sealed class SearchPageViewModel
{
    public string Query { get; init; } = string.Empty;

    public bool UseSemanticSearch { get; init; }

    public SearchResponse? Result { get; init; }
}