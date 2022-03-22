using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInfo.Library
{
    public class UserInfoOptions
    {
        public const string UserInfo = "UserInfo";

        /// <summary>
        /// 
        /// </summary>
        public UserInfoOptions()
        {
        }

        public bool Caching { get; set; } = true;

        /// <summary>
        /// Gets or sets the graph default scopes set on app registration.
        /// </summary>
        public string DefaultScopes { get; set; } = "https://graph.microsoft.com/.default";

        public string UnknownEmail { get; set; } = string.Empty;
    }
}
