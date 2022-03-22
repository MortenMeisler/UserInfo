using System;
using System.Diagnostics.CodeAnalysis;

namespace UserInfo.Library
{
    /// <summary>
    /// Add to methods that check input for null and throw if the input is null.
    /// </summary>
    /// <!-- Inspiration: https://github.com/ardalis/GuardClauses/blob/master/src/GuardClauses/GuardClauseExtensions.cs -->
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    [ExcludeFromCodeCoverage]
    public sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}