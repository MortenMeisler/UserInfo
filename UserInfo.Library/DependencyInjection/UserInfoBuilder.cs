using LazyCache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInfo.Library.DependencyInjection
{
    internal class UserInfoBuilder : IUserInfoBuilder
    {
        private UserInfoOptions? _userInfoOptions;

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
