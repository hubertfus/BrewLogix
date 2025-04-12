using BrewLogix.Components;
using BrewLogix.Models;
using BrewLogix.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ClientService>();
builder.Services.AddSingleton<IngredientService>();
builder.Services.AddSingleton<StockEntriesService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<BatchService>();
builder.Services.AddScoped<KegService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
