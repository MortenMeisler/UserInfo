using Microsoft.Extensions.DependencyInjection;

namespace UserInfo.Library.DependencyInjection
{
    /// <summary>
    /// The User Info builder
    /// </summary>
    public interface IUserInfoBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/>.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Gets the <see cref="UserInfoOptions"/>.
        /// </summary>
        UserInfoOptions UserInfoOptions { get; }
    }
}
