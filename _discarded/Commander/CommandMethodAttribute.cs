using System;
using PaderConference.Infrastructure.Services;

namespace PaderConference.Infrastructure.Commander
{
    /// <summary>
    ///     Applied on a method inside a <see cref="IConferenceService" /> declares the method as command
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandMethodAttribute : Attribute
    {
        /// <summary>
        ///     The name of the command. If null, the name of the method will be used
        /// </summary>
        public string? MethodName { get; set; }
    }
}
