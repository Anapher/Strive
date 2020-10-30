using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Services;

namespace PaderConference.Infrastructure.Commander
{
    public static class CommandMap
    {
        public static IImmutableDictionary<ServiceCommandKey, CommandExecutionInfo> Build<TContext>(
            IEnumerable<IConferenceServiceManager> services)
        {
            var result = new Dictionary<ServiceCommandKey, CommandExecutionInfo>();

            foreach (var serviceManager in services)
            {
                var serviceName = serviceManager.ServiceType.Name.TrimEnd("Service");

                var commandMethods = serviceManager.ServiceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var commandMethod in commandMethods)
                {
                    var attribute = commandMethod.GetCustomAttribute<CommandMethodAttribute>();
                    if (attribute == null) continue;

                    var parameters = commandMethod.GetParameters();
                    if (parameters.Length == 0 || parameters.Length > 2)
                        throw new InvalidOperationException(
                            $"The command method must accept exactly 1-2 parameters. {commandMethod}");

                    var parameter = parameters.First();
                    if (parameter.ParameterType != typeof(TContext))
                        throw new InvalidOperationException(
                            $"The first parameter must have the type {typeof(TContext)}. {commandMethod}");

                    Type? payloadType = null;
                    if (parameters.Length == 2) payloadType = parameters[1].ParameterType;

                    var name = attribute.MethodName ?? commandMethod.Name;

                    var key = new ServiceCommandKey(serviceName, name);
                    var commandInfo = new CommandExecutionInfo(serviceManager, name, payloadType, commandMethod);

                    result.Add(key, commandInfo);
                }
            }

            return result.ToImmutableDictionary();
        }
    }
}
