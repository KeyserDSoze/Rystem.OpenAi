using Polly;
using Rystem.OpenAi;
using Rystem.OpenAi.WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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


var app = builder.Build();

app.UseExceptionHandler("/Error");
app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
