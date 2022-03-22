using System;
using System.Diagnostics.CodeAnalysis;

namespace UserInfo.Library
{
    /// <summary>
    /// Guard class to protect against null.
    /// </summary>
    /// <!-- Inspiration: https://github.com/ardalis/GuardClauses/blob/master/src/GuardClauses/GuardClauseExtensions.cs -->
    [ExcludeFromCodeCoverage]
    internal static class Guard
    {
        /// <summary>
        /// Validates against null.
        /// </summary>
        /// <param name="value">Value to be validated.</param>
        /// <param name="parameterName">Name of parameter.</param>
        /// <exception cref="ArgumentException"> if value is null.</exception>
        internal static void AgainstNull([ValidatedNotNull] object value, string parameterName)
        {
            if (value is null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Validates against null.
        /// </summary>
        /// <param name="value">Value to be validated.</param>
        /// <param name="parameterName">Name of parameter.</param>
        /// <param name="message">Message if validation fails.</param>
        /// <exception cref="ArgumentException"> if value is null.</exception>
        internal static void AgainstNull([ValidatedNotNull] object value, string parameterName, string message)
        {
            if (value is null)
            {
                throw new ArgumentNullException(parameterName, message);
            }
        }

        /// <summary>
        /// Validates against null.
        /// </summary>
        /// <param name="value">Value to be validated.</param>
        /// <param name="parameterName">Name of parameter.</param>
        /// <param name="message">Message if validation fails.</param>
        /// <exception cref="ArgumentException"> if value is null.</exception>
        internal static void AgainstNullOrEmpty([ValidatedNotNull] string value, string parameterName, string message)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(parameterName, message);
            }
        }

        /// <summary>
        /// Validates against empty Guid.
        /// </summary>
        /// <param name="value">Guid to validate.</param>
        /// <param name="parameterName">Name of parameter.</param>
        /// <param name="message">Message if validation fails.</param>
        internal static void AgainstEmptyGuid(Guid value, string parameterName, string message)
        {
            if (Guid.Empty.Equals(value))
            {
                throw new ArgumentException(parameterName, message);
            }
        }
    }
}