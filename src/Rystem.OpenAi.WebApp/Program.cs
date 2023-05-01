using Rystem.OpenAi;
using Rystem.OpenAi.WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable S1481 // Unused local variables should be removed
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
var apiKey = builder.Configuration["OpenAi:ApiKey"];
var resourceName = builder.Configuration["Azure:ResourceName"];
var clientId = builder.Configuration["AzureAd:ClientId"];
var clientSecret = builder.Configuration["AzureAd:ClientSecret"];
var tenantId = builder.Configuration["AzureAd:TenantId"];
var managedIdentityId = builder.Configuration["ManagedIdentity:ClientId"];
builder.Services.AddOpenAi(settings =>
{
    //settings.ApiKey = apiKey;
    settings.Azure.ResourceName = resourceName;
    //settings.Azure.AppRegistration.ClientId = clientId;
    //settings.Azure.AppRegistration.ClientSecret = clientSecret;
    //settings.Azure.AppRegistration.TenantId = tenantId;
    settings.Azure.ManagedIdentity.Id = managedIdentityId;
    //settings.Azure.ManagedIdentity.UseDefault = true;
    settings.Azure
        .MapDeploymentTextModel("Test", TextModelType.CurieText)
        .MapDeploymentTextModel("text-davinci-002", TextModelType.DavinciText2)
        .MapDeploymentEmbeddingModel("Test", EmbeddingModelType.AdaTextEmbedding);
});

#pragma warning restore S125 // Sections of code should not be commented out, but it's only a debug purpose of a test application.
#pragma warning restore S1481 // Unused local variables should be removed, but it's only a debug purpose of a test application.

var app = builder.Build();
await app.Services.MapDeploymentsAutomaticallyAsync(true, "");
app.UseExceptionHandler("/Error");
app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
