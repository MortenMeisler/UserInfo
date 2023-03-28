using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UserInfo.Library;

namespace UserInfo.Providers
{
    /// <summary>
    /// Authentication provider for Azure Managed Identity.
    /// </summary>
    internal class ClientCredentialsAuthenticationProvider : IAuthenticationProvider
    {
        private readonly UserInfoOptions _options;
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedIdentityAuthenticationProvider"/> class.
        /// </summary>
        internal ClientCredentialsAuthenticationProvider(UserInfoOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Adds an authorization header to an HttpRequestMessage.
        /// </summary>
        /// <param name="request">HttpRequest message to authenticate.</param>
        /// <returns>A Task (as this is an async method).</returns>

        public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            var app = ConfidentialClientApplicationBuilder.Create(_options.ClientId)
                .WithClientSecret(_options.ClientSecret)
                .WithAuthority($"https://login.microsoftonline.com/{_options.Domain}/")
                .Build();

            var resourceIds = new[] { $"{UserInfoOptions.GraphUrl}.default" };
            var token = await app.AcquireTokenForClient(resourceIds).ExecuteAsync();

            request.Headers.Add(HttpRequestHeader.Authorization.ToString(), new string[] { $"Bearer {token.AccessToken}" } );
        }
    }
}
