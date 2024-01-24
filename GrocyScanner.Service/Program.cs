using GrocyScanner.Core.Configurations;
using GrocyScanner.Core.GrocyClient;
using GrocyScanner.Core.Providers;
using GrocyScanner.Core.Validators;
using GrocyScanner.Service.Hubs;
using MudBlazor;
using MudBlazor.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfigurationRoot configurationRoot = builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables().Build();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IGtinValidator, GtinValidator>();
builder.Services.AddSingleton<IProductProvider, OpenFoodFactsProductProvider>();
builder.Services.AddSingleton<IGrocyClient, GrocyClient>();
builder.Services.AddSingleton<IGrocyQuantityUnit, GrocyQuantityUnitsMasterData>();
builder.Services.AddSingleton<IGrocyLocations, GrocyLocationMasterData>();
builder.Services.AddSignalR();
builder.Services.AddScoped<QrCodeScanHub>();

builder.Services.Configure<GrocyConfiguration>(configurationRoot.GetSection(GrocyConfiguration.Name));
builder.Services.AddHttpClient();
builder.Services.AddMudServices();
builder.Services.AddScoped<IDialogService, DialogService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<QrCodeScanHub>("/hubs/barcode");
});
app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();