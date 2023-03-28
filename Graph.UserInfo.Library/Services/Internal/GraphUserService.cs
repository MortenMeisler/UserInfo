using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UserInfo.Library.Extensions;
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
            var user = await _graphServiceClient.Users[objectId]
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "id", "userPrincipalName", "displayName" };
                }, cancellationToken: ct).ConfigureAwait(false);

            return user != null ? new UserSimple(user) : null;
        }

        internal async Task<IEnumerable<UserSimple>> GetUsers(CancellationToken ct)
        {
            var users = await _graphServiceClient.Users
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "id", "userPrincipalName", "displayName" };
                }, cancellationToken: ct).ConfigureAwait(false);

            return users?.Value.Select(x => new UserSimple(x)) ?? Enumerable.Empty<UserSimple>();
        }

        internal async Task<IEnumerable<UserSimple>> GetUsersByObjectIds(IEnumerable<string> objectIds, CancellationToken ct)
        {
            var users = await GetUsersInBatch(objectIds, ct).ConfigureAwait(false);

            return users?.Select(x => new UserSimple(x)) ?? Enumerable.Empty<UserSimple>();
        }

        internal async Task<string?> GetUserNameByObjectId(string objectId, CancellationToken ct)
        {
            Guard.AgainstNull(objectId, nameof(objectId));

            var user = await _graphServiceClient.Users[objectId]
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "userPrincipalName" };
                }, cancellationToken: ct).ConfigureAwait(false);

            return user?.UserPrincipalName;
        }

        internal async Task<IDictionary<string, string>> GetUserNamesByObjectIds(IEnumerable<string> objectIds, CancellationToken ct)
        {
            var users = await GetUsersInBatch(objectIds, ct).ConfigureAwait(false);

            return users.ToDictionary(x => x.Id, x => x.UserName());
        }

        internal async Task<IEnumerable<UserSimple>> SearchUsers(string filterString, CancellationToken ct)
        {
            Guard.AgainstNullOrEmpty(filterString, nameof(filterString), "filterString cannot be null or empty.");

            var users = await _graphServiceClient.Users
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "id", "userPrincipalName", "displayName" };
                    requestConfiguration.QueryParameters.Filter = filterString;
                }, cancellationToken: ct).ConfigureAwait(false);

            return users?.Value.Select(x => new UserSimple(x)) ?? Enumerable.Empty<UserSimple>();
        }

        private async Task<IEnumerable<User>> GetUsersInBatch(IEnumerable<string> objectIds, CancellationToken ct)
        {
            Guard.AgainstNull(objectIds, nameof(objectIds));

            using var batchRequestContent = new BatchRequestContent(_graphServiceClient);
            var requestIdList = new List<string>();
            var users = new List<User>();

            foreach (var objectIdgroup in objectIds.Distinct().MakeGroupsOf(15))
            {
                var filterString = string.Join(" or ", objectIdgroup.Where(x => !string.IsNullOrEmpty(x)).Select(objectId => $"id eq '{objectId}'"));

                var userRequest = _graphServiceClient.Users
                    .ToGetRequestInformation
                    (requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Select = new string[] { "id", "userPrincipalName", "displayName" };
                        requestConfiguration.QueryParameters.Filter = filterString;
                    });

                var requestStepId = await batchRequestContent.AddBatchRequestStepAsync(userRequest);

                requestIdList.Add(requestStepId);
            }

            var batchResponse = await _graphServiceClient.Batch.PostAsync(batchRequestContent, ct).ConfigureAwait(false);

            foreach (var requestId in requestIdList)
            {
                try
                {
                    var usersBatch = await batchResponse.GetResponseByIdAsync<UserCollectionResponse>(requestId).ConfigureAwait(false);
                    users.AddRange(usersBatch.Value);
                }
                catch (ServiceException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
                {
                    // No results for the given batch
                }
            }

            return users;
        }
    }
}
