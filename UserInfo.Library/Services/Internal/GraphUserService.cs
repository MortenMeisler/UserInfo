using Microsoft.Graph;
using UserInfo.Library.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UserInfo.Library.Models;

namespace UserInfo.Library.Services
{
    /// <summary>
    /// Graph User service for retrieving user info.
    /// </summary>
    /// <seealso cref="UserInfoService" />
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "documented within public service")]
    internal class GraphUserService
    {
        private readonly GraphServiceClient _graphServiceClient;

        internal GraphUserService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        internal async Task<UserSimple?> GetUser(string objectId, CancellationToken ct)
        {
            var user = await _graphServiceClient.Users[objectId].Request()
                .Select(x => new { x.UserPrincipalName, x.DisplayName, x.Id })
                .GetAsync(ct).ConfigureAwait(false);

            return user != null ? new UserSimple(user) : null;
        }

        internal async Task<IEnumerable<UserSimple>> GetUsers(CancellationToken ct)
        {
            var users = await _graphServiceClient.Users.Request()
                .Select(x => new { x.UserPrincipalName, x.DisplayName, x.Id })
                .GetAsync(ct).ConfigureAwait(false);

            return users?.Select(x => new UserSimple(x)) ?? Enumerable.Empty<UserSimple>();
        }

        internal async Task<IEnumerable<UserSimple>> GetUsersByObjectIds(IEnumerable<string> objectIds, CancellationToken ct)
        {
            var users = await GetUsersInBatch(objectIds, ct).ConfigureAwait(false);

            return users?.Select(x => new UserSimple(x)) ?? Enumerable.Empty<UserSimple>();
        }

        internal async Task<string?> GetUserNameByObjectId(string objectId, CancellationToken ct)
        {
            Guard.AgainstNull(objectId, nameof(objectId));

            var user = await _graphServiceClient.Users[objectId].Request()
                .Select(x => new { x.UserPrincipalName })
                .GetAsync(ct).ConfigureAwait(false);

            return user?.UserName();
        }

        internal async Task<IDictionary<string, string>> GetUserNamesByObjectIds(IEnumerable<string> objectIds, CancellationToken ct)
        {
            var users = await GetUsersInBatch(objectIds, ct).ConfigureAwait(false);

            return users.ToDictionary(x => x.Id, x => x.UserName());
        }

        internal async Task<IEnumerable<UserSimple>> SearchUsers(string filterString, CancellationToken ct)
        {
            Guard.AgainstNullOrEmpty(filterString, nameof(filterString), "filterString cannot be null or empty.");

            var users = await _graphServiceClient.Users.Request()
                .Filter(filterString)
                .GetAsync(ct).ConfigureAwait(false);

            return users?.Select(x => new UserSimple(x)) ?? Enumerable.Empty<UserSimple>();
        }

        private async Task<IEnumerable<User>> GetUsersInBatch(IEnumerable<string> objectIds, CancellationToken ct)
        {
            Guard.AgainstNull(objectIds, nameof(objectIds));

            using var batchRequestContent = new BatchRequestContent();
            var requestIdList = new List<string>();
            var users = new List<User>();

            foreach (var objectIdgroup in objectIds.Distinct().MakeGroupsOf(15))
            {
                var filterString = string.Join(" or ", objectIdgroup.Where(x => !string.IsNullOrEmpty(x)).Select(objectId => $"id eq '{objectId}'"));

                var userRequest = _graphServiceClient.Users.Request()
                                                     .Select(x => new { x.UserPrincipalName, x.DisplayName, x.Id })
                                                     .Filter(filterString);

                requestIdList.Add(batchRequestContent.AddBatchRequestStep(userRequest));
            }

            var batchResponse = await _graphServiceClient.Batch.Request().PostAsync(batchRequestContent, ct).ConfigureAwait(false);

            foreach (var requestId in requestIdList)
            {
                try
                {
                    var usersBatch = await batchResponse.GetResponseByIdAsync<GraphServiceUsersCollectionResponse>(requestId).ConfigureAwait(false);
                    users.AddRange(usersBatch.Value);
                }
                catch (ServiceException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    // No results for the given batch
                }
            }

            return users;
        }
    }
}
