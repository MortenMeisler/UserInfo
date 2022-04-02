# Graph User Info Service

## Introduction 
This is a simple Microsoft Graph wrapper for retrieving Azure AD Users using the most optimal approach in terms of filtering, batching and caching.

A common scenario is when we only have the user objectId (perhaps retrieved from authorization context) set as createdByUser or modifiedByUser - and now we need to translate that into a full name, email and/or username.

## Getting Started
Graph UserInfo uses Microsoft Graph underneath. This requires that App Registration in Azure has permissions to read user information i AAD.

Make sure your App has at least the following API Permissions:
 - API: Microsoft Graph
 - Name / Scope: User.Read.All
 - Type: Application
 - Admin consent.

For setting up Managed Identity for your web app and grant it the right Microsoft Graph permissions, please follow this guide: https://docs.microsoft.com/en-us/azure/app-service/scenario-secure-app-access-microsoft-graph-as-app?tabs=azure-powershell

appsettings setup:

```csharp
// ** Minimum setup for running locally using Managed Identity. ** //
  "UserInfo": {
    "TenantId": "22222222-2222-2222-2222-222222222222" // only needed when testing locally.
  }
```
Check web app sample for more setup options.

Program.cs (or Startup.cs):

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...

   // ******** Option 1: The most simple setup using Azure Managed Identity and passing Configuration as argument. ********
   // Make sure appsettings or web.config contains a section called UserInfo with minimum tenantId for testng locally.
    builder.Services.AddUserInfoService(builder.Configuration)
    .WithManagedIdentity();

    // ...
}
```
Check web app sample for more setup options.

By default memory caching is used (lazy cache), but you can turn this off by setting Caching to false in appsettings.
Currently there is no expiration on the caching since objectIds and usernames will not change over time. If needed you can always flush the cache by restarting the web app. Or make an issue and I will provide more caching options.

## Usage
Check WebAppSample app for different uses and setup.

Example:
```csharp
private readonly IUserInfoService _userService;

public SomeService(IUserInfoService userService)
{
    _userService = userService;
}

public async Task SomeServiceMethod()
{
    // Get a specific user
    var user = await _userService.GetUser("03c0532c-3a9b-4875-8c15-5930e0394eb6").ConfigureAwait(false);

    // Get specific users
    var listOfIds = new List<string> { "03c0532c-3a9b-4875-8c15-5930e0394eb6", "ee7ffd7a-736b-4bc3-91d4-6bed8c261daf" };
    var users = await _userService.GetUsersByObjectIds(listOfIds).ConfigureAwait(false);

    // Get users by search
    var users = await _userService.SearchUsers("startsWith(displayname, 'Morten')").ConfigureAwait(false);

}

```
