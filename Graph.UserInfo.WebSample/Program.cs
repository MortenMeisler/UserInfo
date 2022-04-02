using Azure.Identity;
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

// ******** Option 1: The most simple setup using Azure Managed Identity and passing Configuration as argument. ********
// Make sure appsettings or web.config contains a section called UserInfo with minimum tenantId for testng locally.
builder.Services.AddUserInfoService(builder.Configuration)
    .WithManagedIdentity();

// ******** Option 2: Bind the option class and configure the options needed. ********

//var options = new UserInfoOptions();
//builder.Configuration.GetSection(UserInfoOptions.UserInfo).Bind(options);

//builder.Services.AddUserInfoService(config
//    =>
//{
//    config.Caching = options.Caching; // default true if not set.
//    config.TenantId = options.TenantId; // Only required when testing localhost and using Azure Managed Identity.
//})
//    .WithManagedIdentity();

// ******** Option 3: Client Credentials. This needs a few more setup options.********

//builder.Services.AddUserInfoService(config
//    =>
//{
//    config.Caching = options.Caching;
//    config.TenantId = options.TenantId;
//    config.ClientId = options.ClientId;
//    config.ClientSecret = options.ClientSecret;
//    config.Domain = options.Domain;
//})
//    .WithClientCredentials();

// ******** Option 4: Provide your own auth provider (or perhaps one from MSAL). ********
//var credential = new ChainedTokenCredential(
//        new ManagedIdentityCredential(),
//        new EnvironmentCredential());
//var token = credential.GetToken(
//    new Azure.Core.TokenRequestContext(
//        new[] { "https://graph.microsoft.com/.default" }));
//var accessToken = token.Token;
//var provider = new DelegateAuthenticationProvider((requestMessage) =>
//{
//    requestMessage
//    .Headers
//    .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

//    return Task.CompletedTask;
//});

//builder.Services.AddUserInfoService(builder.Configuration)
//    .WithAuthenticationProvider(provider);


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
