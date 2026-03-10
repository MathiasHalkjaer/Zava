using Microsoft.AspNetCore.Mvc;
using Zava.Web.Extensions;
using Zava.Web.Models;
using Zava.Web.Services;

namespace Zava.Web.Controllers;

public sealed class CartController(ICatalogApiClient catalogApiClient) : Controller
{
    private const string CartSessionKey = "cart-items";

    [HttpGet]
    public async Task<IActionResult> Index(bool checkedOut, CancellationToken cancellationToken)
    {
        var items = await BuildCartAsync(cancellationToken);
        return View(new CartPageViewModel
        {
            Items = items,
            WasCheckedOut = checkedOut
        });
    }

    [HttpPost]
    public IActionResult Add(string productId)
    {
        var lines = GetCart();
        var line = lines.FirstOrDefault(item => item.ProductId == productId);
        if (line is null)
        {
            lines.Add(new CartLine(productId, 1));
        }
        else
        {
            lines[lines.IndexOf(line)] = line with { Quantity = line.Quantity + 1 };
        }

        SaveCart(lines);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Increase(string productId)
    {
        return UpdateQuantity(productId, 1);
    }

    [HttpPost]
    public IActionResult Decrease(string productId)
    {
        return UpdateQuantity(productId, -1);
    }

    [HttpPost]
    public IActionResult Remove(string productId)
    {
        var lines = GetCart();
        lines.RemoveAll(item => item.ProductId == productId);
        SaveCart(lines);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Checkout()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return RedirectToAction(nameof(Index), new { checkedOut = true });
    }

    private IActionResult UpdateQuantity(string productId, int delta)
    {
        var lines = GetCart();
        var line = lines.FirstOrDefault(item => item.ProductId == productId);
        if (line is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var nextQuantity = Math.Max(0, line.Quantity + delta);
        lines.RemoveAll(item => item.ProductId == productId);
        if (nextQuantity > 0)
        {
            lines.Add(line with { Quantity = nextQuantity });
        }

        SaveCart(lines);
        return RedirectToAction(nameof(Index));
    }

    private List<CartLine> GetCart()
    {
        return HttpContext.Session.GetObject<List<CartLine>>(CartSessionKey) ?? [];
    }

    private void SaveCart(List<CartLine> lines)
    {
        HttpContext.Session.SetObject(CartSessionKey, lines);
    }

    private async Task<IReadOnlyList<CartLineViewModel>> BuildCartAsync(CancellationToken cancellationToken)
    {
        var products = await catalogApiClient.GetProductsAsync(cancellationToken);
        var lookup = products.ToDictionary(product => product.Id, StringComparer.OrdinalIgnoreCase);

        return GetCart()
            .Where(line => lookup.ContainsKey(line.ProductId))
            .Select(line => new CartLineViewModel(lookup[line.ProductId], line.Quantity))
            .OrderBy(line => line.Product.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}