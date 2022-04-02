using Microsoft.Extensions.DependencyInjection;
using System;

namespace UserInfo.Library.DependencyInjection
{
    internal class UserInfoBuilder : IUserInfoBuilder
    {
        /// <summary>
        /// Initializes a new <see cref="UserInfoBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="options">The UserInfo options.</param>
        public UserInfoBuilder(IServiceCollection services, UserInfoOptions? options = null)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            UserInfoOptions = options ?? new UserInfoOptions();
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public UserInfoOptions UserInfoOptions { get; }
    }
}
