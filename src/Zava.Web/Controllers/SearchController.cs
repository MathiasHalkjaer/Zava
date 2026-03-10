using Microsoft.AspNetCore.Mvc;
using Zava.Models;
using Zava.Web.Models;
using Zava.Web.Services;

namespace Zava.Web.Controllers;

public sealed class SearchController(ICatalogApiClient catalogApiClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? query, bool useSemanticSearch, CancellationToken cancellationToken)
    {
        SearchResponse? result = null;
        if (!string.IsNullOrWhiteSpace(query))
        {
            result = await catalogApiClient.SearchAsync(query, useSemanticSearch, cancellationToken);
        }

        return View(new SearchPageViewModel
        {
            Query = query ?? string.Empty,
            UseSemanticSearch = useSemanticSearch,
            Result = result
        });
    }
}