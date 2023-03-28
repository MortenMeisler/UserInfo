using LazyCache;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserInfo.Library.Models;

namespace UserInfo.Library.Services
{
    /// <summary>
    /// Service for retrieving user info memory cached.
    /// </summary>
    /// <seealso cref="MemoryCachedUserInfoService" />
    internal class MemoryCachedUserInfoService : IUserInfoService
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IAppCache _cache;
        private readonly UserInfoOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCachedUserInfoService"/> class.
        /// </summary>
        /// <param name="authenticationProvider">the authentication provider.</param>
        /// <param name="cache">the cache.</param>
        /// <param name="options">the options.</param>
        public MemoryCachedUserInfoService(IAuthenticationProvider authenticationProvider, IAppCache cache, UserInfoOptions options)
        {
            Guard.AgainstNull(cache, nameof(cache));

            _cache = cache;
            _options = options;
            _userInfoService = new UserInfoService(authenticationProvider);
        }

        /// <inheritdoc />
        public async Task<UserSimple?> GetUserByObjectId(string objectId, CancellationToken ct = default)
        {
            var user = await _cache.GetOrAddAsync(objectId, async entry =>
                (await _userInfoService.GetUserByObjectId(objectId, ct).ConfigureAwait(false)) ?? EmptyUser(objectId, _options.UnknownEmail))
                .ConfigureAwait(false);

            if (user.UserNotFoundInAD)
            {
                return null;
            }

            return user;
        }

        /// <inheritdoc />
        public async Task<string?> GetUserNameByObjectId(string objectId, CancellationToken ct = default)
        {
            var user = await GetUserByObjectId(objectId, ct).ConfigureAwait(false);
            return user?.UserName;
        }

        /// <inheritdoc />
        public async Task<IDictionary<string, string>> GetUserNamesByObjectIds(IList<string> objectIds, CancellationToken ct = default)
        {
            var users = await GetUsersByObjectIds(objectIds, ct).ConfigureAwait(false);
            return users.Distinct().ToDictionary(x => x.ObjectId, x => x.UserName);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserSimple>> GetUsers(CancellationToken ct = default)
        {
            // no caching for get all.
            return await _userInfoService.GetUsers(ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserSimple>> GetUsersByObjectIds(IList<string> objectIds, CancellationToken ct = default)
        {
            Guard.AgainstNull(objectIds, nameof(objectIds));

            // Get cached users
            var cachedUsers = objectIds!.Select(objectId =>
            {
                var user = _cache.Get<UserSimple>(objectId);
                var isCached = user != null;
                return new { isCached, user, objectId };
            }).ToList();

            // Fetch any missing userIds not in cache
            var missingUserIds = cachedUsers!.Where(cu => !cu.isCached).Select(cu => cu.objectId).ToList();
            var usersByObjectId = missingUserIds.Any() ? (await _userInfoService.GetUsersByObjectIds(missingUserIds, ct).ConfigureAwait(false)).ToList() : Enumerable.Empty<UserSimple>();

            // Add fetched users to cache
            foreach (var user in usersByObjectId)
            {
                _cache.Add(user.ObjectId, user);
            }

            // Add empty placeholders for any userids that were not found.
            foreach (var user in missingUserIds.Where(id => !usersByObjectId.Any(user => user.ObjectId == id)))
            {
                _cache.GetOrAdd(user, () => EmptyUser(user, _options.UnknownEmail));
            }

            return cachedUsers
                .Where(cu => cu.isCached && cu.user?.UserNotFoundInAD != true)
                .Select(cu => cu.user)
                .Concat(usersByObjectId)!;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserSimple>> SearchUsers(string filterString, CancellationToken ct = default)
        {
            // no caching for searching.
            return await _userInfoService.SearchUsers(filterString, ct).ConfigureAwait(false);
        }

        private static UserSimple EmptyUser(string userId, string unknownUserEmail)
        {
            return new UserSimple(new User { Id = userId, UserPrincipalName = unknownUserEmail }) { UserNotFoundInAD = true };
        }
    }
}
