using Microsoft.AspNetCore.Mvc;
using Zava.Web.Models;
using Zava.Web.Services;

namespace Zava.Web.Controllers;

public sealed class ProductsController(ICatalogApiClient catalogApiClient) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var products = await catalogApiClient.GetProductsAsync(cancellationToken);
        return View(new ProductPageViewModel { Products = products });
    }

    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
    {
        var products = await catalogApiClient.GetProductsAsync(cancellationToken);
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }
}