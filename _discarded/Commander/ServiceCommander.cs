using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Threading.Tasks;
using PaderConference.Core.Dto;
using PaderConference.Infrastructure.Services;

namespace PaderConference.Infrastructure.Commander
{
    public class ServiceCommander<TContext>
    {
        private readonly IImmutableDictionary<ServiceCommandKey, CommandExecutionInfo> _commands;

        public ServiceCommander(IEnumerable<IConferenceServiceManager> serviceManagers)
        {
            _commands = CommandMap.Build<TContext>(serviceManagers);
        }

        public async ValueTask<CommandExecutionResult> Execute(ServiceCommandDto dto, JsonElement? payload)
        {
            if (dto.Service == null)
                return new CommandExecutionResult(CommanderError.ServiceNotFound);
            if (dto.Method == null)
                return new CommandExecutionResult(CommanderError.MethodNotFound);

            var key = new ServiceCommandKey(dto.Service, dto.Method);
            if (!_commands.TryGetValue(key, out var executionInfo))
                return new CommandExecutionResult(CommanderError.ServiceNotFound);

            object? payloadObj = null;
            if (executionInfo.PayloadType != null)
                if (!payload.HasValue)
                    return new CommandExecutionResult(CommanderError.PayloadMustNotBeNull);

            //JsonSerializer.Deserialize(payload.Value)
        }
    }

    public class CommandExecutionResult
    {
        public CommandExecutionResult(Error error)
        {
            Error = error;
            HasError = true;
        }

        public CommandExecutionResult(object? result)
        {
            Result = result;
            HasError = false;
        }

        public bool HasError { get; }

        public Error? Error { get; }

        public object? Result { get; }
    }
}
