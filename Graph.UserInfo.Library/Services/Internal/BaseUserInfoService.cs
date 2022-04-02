using Microsoft.Graph;
using Polly;
using System;
using System.Net;
using System.Threading.Tasks;

namespace UserInfo.Library.Services
{
    /// <summary>
    /// Base class for invoking actions. Handles retries and exceptions.
    /// </summary>
    internal abstract class BaseUserInfoService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseUserInfoService"/> class.
        /// </summary>
        protected BaseUserInfoService()
        {
        }

        /// <summary>
        /// Invokes the client request, does not expect response object.
        /// </summary>
        /// <typeparam name="T">Expected response type.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        internal static async Task<T?> InvokeRequestAsync<T>(Func<Task<T>> task)
        {
            try
            {
                return await task.Invoke().ConfigureAwait(false);
            }
            catch (ServiceException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
            catch (ServiceException ex) when (ex.StatusCode == (HttpStatusCode)429)
            {
                var completed = false;

                // Retry 3 times with one second delay.
                var retryPolicy = Policy.Handle<ServiceException>()
                                        .WaitAndRetryAsync(
                                                           3,
                                                           (retryAttempt, context) => TimeSpan.FromSeconds(1),
                                                           (exception, time, content) => Console.WriteLine(exception.Message));

                await retryPolicy.ExecuteAsync(() =>
                {
                    var invoked = task.Invoke();
                    completed = true;
                    return invoked;
                }).ConfigureAwait(false);

                if (!completed)
                {
                    throw;
                }
            }

            return default;
        }
    }
}
