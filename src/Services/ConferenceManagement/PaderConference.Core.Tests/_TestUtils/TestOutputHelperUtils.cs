using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests._TestUtils
{
    public static class TestOutputHelperUtils
    {
        public static ILogger<T> CreateLogger<T>(this ITestOutputHelper output)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.TestOutput(output).CreateLogger();
            return new SerilogLoggerFactory(logger).CreateLogger<T>();
        }
    }
}
