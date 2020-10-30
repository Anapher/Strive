using System;
using System.Reflection;
using PaderConference.Infrastructure.Services;

namespace PaderConference.Infrastructure.Commander
{
    public class CommandExecutionInfo
    {
        public CommandExecutionInfo(IConferenceServiceManager serviceManager, string name, Type? payloadType,
            MethodInfo methodInfo)
        {
            ServiceManager = serviceManager;
            Name = name;
            PayloadType = payloadType;
            MethodInfo = methodInfo;
        }

        public IConferenceServiceManager ServiceManager { get; }

        public string Name { get; }

        public Type? PayloadType { get; }

        //public Type? ReturnType { get; set; }

        public MethodInfo MethodInfo { get; }
    }
}
