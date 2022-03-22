using Microsoft.Graph;
using UserInfo.Library.Extensions;

namespace UserInfo.Library.Models
{
    /// <summary>
    /// User model with essential info.
    /// </summary>
    public class UserSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSimple"/> class.
        /// </summary>
        /// <param name="user">the graph user object.</param>
        internal UserSimple(User user)
        {
            ObjectId = user.Id;
            UserName = user.UserName();
            UserPrincipalName = user.UserPrincipalName;
            DisplayName = user.DisplayName;
        }

        /// <summary>
        /// Gets the Object Id.
        /// </summary>
        public string ObjectId { get; }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the username.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Gets the UPN.
        /// </summary>
        public string UserPrincipalName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether user was not found in AD.
        /// </summary>
        public bool UserNotFoundInAD { get; set; }
    }
}
