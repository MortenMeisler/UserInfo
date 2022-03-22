using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Azure.Identity;
using UserInfo.Library;

namespace UserInfo.Providers
{
    /// <summary>
    /// Authentication provider for system token acquisitions.
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
            var credential = new ChainedTokenCredential(
                        new ManagedIdentityCredential(),
                         new EnvironmentCredential());
            var token = await credential.GetTokenAsync(
                new Azure.Core.TokenRequestContext(
                    new[] { _options.DefaultScopes }));

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        }
       }
}
