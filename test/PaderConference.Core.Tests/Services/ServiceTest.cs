using Microsoft.Extensions.Logging;
using PaderConference.Core.Services;
using Serilog;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services
{
    public abstract class ServiceTest<T> where T : IConferenceService
    {
        protected ILogger<T> Logger;

        public ServiceTest(ITestOutputHelper output)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.TestOutput(output).CreateLogger();

            Logger = new SerilogLoggerFactory(logger).CreateLogger<T>();
        }
    }
}
