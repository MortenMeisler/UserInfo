using LazyCache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Graph;
using System;
using UserInfo.Library.Services;
using UserInfo.Providers;

namespace UserInfo.Library.DependencyInjection
{
    /// <summary>
    /// Extension for adding DI Support.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add support to call UserInfoService via dependency injection.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <returns>The services.</returns>
        public static IUserInfoBuilder AddUserInfoService(
            this IServiceCollection services)
        {
            return new UserInfoBuilder(services);
        }

        /// <summary>
        /// Add support to call UserInfoService via dependency injection.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="options">The UserInfo options.</param>
        /// <returns>The services.</returns>
        public static IUserInfoBuilder AddUserInfoService(
            this IServiceCollection services, Action<UserInfoOptions> options)
        {
            var builder = services.AddUserInfoService();

            builder.Services.Configure(options);
            options.Invoke(builder.UserInfoOptions);

            return builder;
        }

        /// <summary>
        /// Adds Azure Managed Identity as authenticaion provider.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IUserInfoBuilder WithAzureManagedIdentity(
            this IUserInfoBuilder builder)
        {
            builder.AddAuthenticationProvider(new ManagedIdentityAuthenticationProvider(builder.UserInfoOptions));

            return new UserInfoBuilder(builder.Services);
        }

        private static IServiceCollection AddAuthenticationProvider(this IUserInfoBuilder builder, IAuthenticationProvider authenticationProvider)
        {
            if (builder.UserInfoOptions.Caching)
            {
                builder.Services.AddLazyCache();
                builder.Services.TryAdd(ServiceDescriptor.Singleton<IUserInfoService, MemoryCachedUserInfoService>(serviceProvider =>
                {
                    return new MemoryCachedUserInfoService(authenticationProvider, serviceProvider.GetRequiredService<IAppCache>(), builder.UserInfoOptions);
                }));
            }
            else
            {
                builder.Services.TryAdd(ServiceDescriptor.Singleton<IUserInfoService, UserInfoService>(serviceProvider =>
                {
                    return new UserInfoService(authenticationProvider);
                }));
            }

            return builder.Services;
        }
    }
}
