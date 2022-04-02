using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var credentials = new ClientCredential(_options.ClientId, _options.ClientSecret);
            var authContext = new AuthenticationContext($"https://login.microsoftonline.com/{_options.Domain}/");
            var token = await authContext.AcquireTokenAsync(UserInfoOptions.GraphUrl, credentials);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }
    }
}
