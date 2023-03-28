using Microsoft.Graph.Models;
using System;

namespace UserInfo.Library.Extensions
{
    /// <summary>
    /// User Extensions.
    /// </summary>
    internal static class UserExtensions
    {
        /// <summary>
        /// Extracts username from UPN (email).
        /// </summary>
        /// <param name="value">The graph user.</param>
        /// <returns>The username.</returns>
        internal static string UserName(this User value)
            => string.IsNullOrEmpty(value.UserPrincipalName) ? "" : value.UserPrincipalName.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0];
    }
}
