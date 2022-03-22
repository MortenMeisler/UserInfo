using UserInfo.Library;
using UserInfo.Library.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRazorPages();

var options = new UserInfoOptions();
builder.Configuration.GetSection(UserInfoOptions.UserInfo).Bind(options);

//builder.Services.AddUserInfoService(options 
//    => options.Caching = false)
//    .WithAzureManagedIdentity();

builder.Services.AddUserInfoService()
    .WithAzureManagedIdentity();


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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
