using LazyCache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Linq;
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
        /// Add support to call UserInfoService via dependency injection.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="configuration">The instance of <see cref="IConfiguration"/> containing section with the name UserInfo.</param>
        /// <returns>The services.</returns>
        public static IUserInfoBuilder AddUserInfoService(
            this IServiceCollection services, IConfiguration configuration)
        {
            var builder = services.AddUserInfoService();

            var section = configuration.GetRequiredSection(UserInfoOptions.UserInfo);

            section.GetChildren().ToList<IConfigurationSection>().ForEach(x =>
            {
                SetOptions(x, builder);
            });

            return builder;
        }

        private static void SetOptions(IConfigurationSection x, IUserInfoBuilder builder)
        {
            if (x.Key.Equals(nameof(UserInfoOptions.ClientId)))
            {
                builder.UserInfoOptions.ClientId = x.Value;
            }
            else if (x.Key.Equals(nameof(UserInfoOptions.ClientSecret)))
            {
                builder.UserInfoOptions.ClientSecret = x.Value;
            }
            else if (x.Key.Equals(nameof(UserInfoOptions.TenantId)))
            {
                builder.UserInfoOptions.TenantId = x.Value;
            }
            else if (x.Key.Equals(nameof(UserInfoOptions.Domain)))
            {
                builder.UserInfoOptions.Domain = x.Value;
            }
            else if (x.Key.Equals(nameof(UserInfoOptions.Caching)))
            {
                builder.UserInfoOptions.Caching = bool.Parse(x.Value);
            }
            else if (x.Key.Equals(nameof(UserInfoOptions.UnknownEmail)))
            {
                builder.UserInfoOptions.UnknownEmail = x.Value;
            }
        }

        /// <summary>
        /// Adds Azure Managed Identity as authenticaion provider.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IUserInfoBuilder WithManagedIdentity(
            this IUserInfoBuilder builder)
        {
            builder.AddAuthenticationProvider(new ManagedIdentityAuthenticationProvider(builder.UserInfoOptions));

            return new UserInfoBuilder(builder.Services);
        }

        /// <summary>
        /// Adds Azure Client Credentials as authenticaion provider.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IUserInfoBuilder WithClientCredentials(
            this IUserInfoBuilder builder)
        {
            builder.AddAuthenticationProvider(new ClientCredentialsAuthenticationProvider(builder.UserInfoOptions));

            return new UserInfoBuilder(builder.Services);
        }

        /// <summary>
        /// Adds custom authenticaion provider. Only needed if Client Credentials or Managed Identity does not suffice.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authenticationProvider">The custom authentication provider.</param>
        /// <returns></returns>
        public static IUserInfoBuilder WithAuthenticationProvider(
            this IUserInfoBuilder builder, IAuthenticationProvider authenticationProvider)
        {
            builder.AddAuthenticationProvider(authenticationProvider);

            return new UserInfoBuilder(builder.Services);
        }

        private static IServiceCollection AddAuthenticationProvider(this IUserInfoBuilder builder, IAuthenticationProvider authenticationProvider)
        {
            if (builder.UserInfoOptions.Caching)
            {
                builder.Services.AddLazyCache();
                builder.Services.TryAdd(ServiceDescriptor.Scoped<IUserInfoService, MemoryCachedUserInfoService>(serviceProvider =>
                {
                    return new MemoryCachedUserInfoService(authenticationProvider, serviceProvider.GetRequiredService<IAppCache>(), builder.UserInfoOptions);
                }));
            }
            else
            {
                builder.Services.TryAdd(ServiceDescriptor.Scoped<IUserInfoService, UserInfoService>(serviceProvider =>
                {
                    return new UserInfoService(authenticationProvider);
                }));
            }

            return builder.Services;
        }
    }
}
