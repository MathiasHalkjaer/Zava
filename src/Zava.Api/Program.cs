using Products.Endpoints;
using Zava.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<EmbeddingSearchOptions>(builder.Configuration.GetSection(EmbeddingSearchOptions.SectionName));
builder.Services.AddHttpClient<IEmbeddingClient, OpenAiEmbeddingClient>();
builder.Services.AddScoped<IProductSearchService, ProductSearchService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapProductActions();
app.MapProductAiActions();

app.Run();

public partial class Program;
