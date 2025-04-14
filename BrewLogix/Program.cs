using BrewLogix;
using BrewLogix.Components;
using BrewLogix.dbhelpers;
using BrewLogix.Models;
using BrewLogix.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<StockEntriesService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<BatchService>();
builder.Services.AddScoped<KegService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<IDbContextProvider, DbContextProvider>();
builder.Services.AddScoped<ProtectedSessionStorage>();


builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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