namespace UserInfo.Library
{
    /// <summary>
    /// The User Info options class for configuration and setup.
    /// </summary>
    public class UserInfoOptions
    {
        /// <summary>
        /// The name of the user info configuration section.
        /// </summary>
        public const string UserInfo = "UserInfo";

        /// <summary>
        /// The base url of graph api.
        /// </summary>
        public const string GraphUrl = "https://graph.microsoft.com/";

        /// <summary>
        /// Gets or sets a value indicating whether memory caching is enabled. Default is <c>true</c>.
        /// </summary>
        public bool Caching { get; set; } = true;

        /// <summary>
        /// Gets or sets the graph default scopes set on app registration.
        /// </summary>
        public string DefaultScopes { get; set; } = "https://graph.microsoft.com/.default";

        /// <summary>
        /// Gets or sets the value for email when no user is returned. Default is empty string.
        /// </summary>
        public string UnknownEmail { get; set; } = string.Empty;

        /// <summary>
        /// TenantId of the Azure AD tenant. 
        /// Not required for Azure Managed Identity provider (unless testing in dev locally).
        /// </summary>
        public string? TenantId { get; set; }

        /// <summary>
        /// ClientId (AppId) of the Azure App when using Client Credentials provider.
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// ClientSecret of the Azure App when using Client Credentials provider.
        /// </summary>
        public string? ClientSecret { get; set; }

        /// <summary>
        /// Tenant domain of the Azure App when using Client Credentials provider.
        /// </summary>
        public string? Domain { get; set; }
    }
}
