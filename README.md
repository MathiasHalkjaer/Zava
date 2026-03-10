# Zava Demo

Zava is a mock e-commerce demo built as a standard .NET 8 multi-project solution.

## Solution layout

- `src/Zava.Web`: ASP.NET Core MVC frontend with Home, Products, Search, Cart, and left navigation layout.
- `src/Zava.Api`: ASP.NET Core minimal API for product listing, keyword search, and AI search.
- `src/Zava.Models`: Shared domain models and in-memory catalog.
- `src/Zava.AI`: Search service and OpenAI embedding client configured for `text-embeddings-ada-002`.
- `tests/Zava.Api.Tests`: xUnit coverage for the AI search endpoint behavior.

## Demo behaviors

- Search results intentionally do not render Add to Cart buttons.
- Proceed to Checkout intentionally clears the cart instead of running a real checkout workflow.
- Product data is in-memory for demo simplicity.

## AI search

The AI search endpoint is `GET /api/search/ai?search=...`.

Configuration lives in `src/Zava.Api/appsettings.json`:

```json
"OpenAI": {
  "Endpoint": "",
  "ApiKey": "",
  "Model": "text-embeddings-ada-002"
}
```

If `Endpoint` and `ApiKey` are not configured, semantic search falls back to keyword search while preserving the demo flow.

## Run locally

1. Start the API: `dotnet run --project src/Zava.Api`
2. Start the web app: `dotnet run --project src/Zava.Web`
3. Open the frontend at `http://localhost:5085`

Default local URLs:

- Web: `http://localhost:5085`
- API: `http://localhost:5086`

## Verification

- Build: `dotnet build Zava.sln`
- Tests: `dotnet test Zava.sln`