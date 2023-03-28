using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
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
    internal class ManagedIdentityAuthenticationProvider : IAuthenticationProvider
    {
        private readonly UserInfoOptions _options;
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedIdentityAuthenticationProvider"/> class.
        /// </summary>
        public ManagedIdentityAuthenticationProvider(UserInfoOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Adds an authorization header to an HttpRequestMessage.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="additionalAuthenticationContext">Additional authentication context.</param>
        /// <param name="cancellationToken">The cancelletationToken.</param>
        /// <returns></returns>

        public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            var azureServiceTokenProvider = new DefaultAzureCredential();
            var tokenRequestContext = new TokenRequestContext(scopes: new string[] { UserInfoOptions.GraphUrl + ".default" }, tenantId: _options.TenantId) { };
            var token = await azureServiceTokenProvider
                .GetTokenAsync(tokenRequestContext);

            request.Headers.Add(HttpRequestHeader.Authorization.ToString(), new string[] { $"Bearer {token.Token}" });
        }
    }
}
