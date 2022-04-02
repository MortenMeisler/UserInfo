using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Graph;
using System.Net.Http;
using System.Net.Http.Headers;
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
        /// <param name="request">HttpRequest message to authenticate.</param>
        /// <returns>A Task (as this is an async method).</returns>
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            string token = await azureServiceTokenProvider
                .GetAccessTokenAsync(UserInfoOptions.GraphUrl, _options.TenantId);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
