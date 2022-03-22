using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserInfo.Library.Models;

namespace UserInfo.Library.Services
{
    /// <summary>
    /// The UserInfoService interface.
    /// </summary>
    public interface IUserInfoService
    {
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="objectId">the objectId or userPrincipalName.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The <see cref="UserSimple"/> object.</returns>
        Task<UserSimple?> GetUserByObjectId(string objectId, CancellationToken ct = default);

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of users.</returns>
        Task<IEnumerable<UserSimple>> GetUsers(CancellationToken ct = default);

        /// <summary>
        /// Get users by objectIds.
        /// </summary>
        /// <param name="objectIds">list of objectIds.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of <see cref="UserSimple"/> objects.</returns>
        Task<IEnumerable<UserSimple>> GetUsersByObjectIds(IList<string> objectIds, CancellationToken ct = default);

        /// <summary>
        /// Gets the UserName by objectId.
        /// </summary>
        /// <param name="objectId">the objectId or userPrincipalName.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The UserName. Returns null if objectId not found in store or objectId is null or empty.</returns>
        Task<string?> GetUserNameByObjectId(string objectId, CancellationToken ct = default);

        /// <summary>
        /// Gets a dictionary of objectIds and UserNames.
        /// </summary>
        /// <param name="objectIds">list of objectIds.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="Dictionary{String, String}" /> of objectId and UserName.</returns>
        Task<IDictionary<string, string>> GetUserNamesByObjectIds(IList<string> objectIds, CancellationToken ct = default);

        /// <summary>
        /// Search users by specified filterString.
        /// <see href="https://docs.microsoft.com/en-us/graph/query-parameters#filter-parameter">Check filter documentation here.</see>.
        /// </summary>
        /// <example>
        /// filterString = "startswith(displayName, 'Conf')"
        /// filterString = "jobTitle eq 'Assassin'".
        /// </example>
        /// <param name="filterString">The filter string.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of users.</returns>
        Task<IEnumerable<UserSimple>> SearchUsers(string filterString, CancellationToken ct = default);
    }
}