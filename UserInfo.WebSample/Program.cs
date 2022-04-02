using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using UserInfo.Library;
using UserInfo.Library.DependencyInjection;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages()
    .AddMvcOptions(options => { })
    .AddMicrosoftIdentityUI();


var options = new UserInfoOptions();
builder.Configuration.GetSection(UserInfoOptions.UserInfo).Bind(options);

//builder.Services.AddUserInfoService(config
//    =>
//{
//    config.Caching = options.Caching;
//    config.TenantId = options.TenantId; // Only required when testing localhost and using Azure Managed Identity.
//    config.ClientId = options.ClientId;
//    config.ClientSecret = options.ClientSecret;
//    config.Domain = options.Domain;
//})
//    .WithClientCredentials();

builder.Services.AddUserInfoService()
    .WithClientCredentials();

//builder.Services.AddUserInfoService()
//    .WithAzureManagedIdentity();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
