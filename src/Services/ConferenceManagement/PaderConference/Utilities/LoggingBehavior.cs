using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PaderConference.Utilities
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            if (!IsActualRequest<TRequest>()) return await next();

            var requestName = request.GetType().Name;
            var requestGuid = Guid.NewGuid().ToString();

            _logger.LogDebug("[START] {requestName} [{guid}]", requestName, requestGuid);
            _logger.LogTrace("[PROPS] {requestName} [{guid}] {@request}", requestName, requestGuid, request);

            TResponse response;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                response = await next();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogDebug("[END] {requestName} [{guid}]; Execution time={elapsed}ms", requestName, requestGuid,
                    stopwatch.ElapsedMilliseconds);
            }

            _logger.LogTrace("[RETURN] {requestName} [{guid}] {@response}", requestName, requestGuid, response);

            return response;
        }

        private static bool IsActualRequest<T>()
        {
            var type = typeof(T);
            return type.IsClass && !type.IsAbstract && type != typeof(object);
        }
    }
}
