using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserInfo.Library.Models;

namespace UserInfo.Library.Services
{
    /// <summary>
    /// Service for retrieving user info.
    /// </summary>
    /// <seealso cref="UserInfoService" />
    internal class UserInfoService : BaseUserInfoService, IUserInfoService
    {
        private readonly GraphUserService _graphUserService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoService"/> class.
        /// </summary>
        /// <param name="authenticationProvider">The authentication provider used for graph service.</param>
        public UserInfoService(IAuthenticationProvider authenticationProvider)
        {
            Guard.AgainstNull(authenticationProvider, nameof(authenticationProvider));

            _graphUserService = new GraphUserService(new GraphServiceClient(authenticationProvider));
        }

        /// <inheritdoc />
        public async Task<UserSimple?> GetUserByObjectId(string objectId, CancellationToken ct = default) =>
            await InvokeRequestAsync(() => _graphUserService.GetUser(objectId, ct)).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IEnumerable<UserSimple>> GetUsers(CancellationToken ct = default) =>
            await InvokeRequestAsync(() => _graphUserService.GetUsers(ct)).ConfigureAwait(false) ?? Enumerable.Empty<UserSimple>();

        /// <inheritdoc />
        public async Task<IEnumerable<UserSimple>> GetUsersByObjectIds(IList<string> objectIds, CancellationToken ct = default) =>
            await InvokeRequestAsync(() => _graphUserService.GetUsersByObjectIds(objectIds, ct)).ConfigureAwait(false) ?? Enumerable.Empty<UserSimple>();

        /// <inheritdoc />
        public async Task<string?> GetUserNameByObjectId(string objectId, CancellationToken ct = default) =>
            await InvokeRequestAsync(() => _graphUserService.GetUserNameByObjectId(objectId, ct)).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IDictionary<string, string>> GetUserNamesByObjectIds(IList<string> objectIds, CancellationToken ct = default) =>
             await InvokeRequestAsync(() => _graphUserService.GetUserNamesByObjectIds(objectIds, ct)).ConfigureAwait(false) ?? new Dictionary<string, string>();

        /// <inheritdoc />
        public async Task<IEnumerable<UserSimple>> SearchUsers(string filterString, CancellationToken ct = default) =>
            await InvokeRequestAsync(() => _graphUserService.SearchUsers(filterString, ct)).ConfigureAwait(false) ?? Enumerable.Empty<UserSimple>();
    }
}
