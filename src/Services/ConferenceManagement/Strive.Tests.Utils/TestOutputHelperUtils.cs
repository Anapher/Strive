using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;

namespace Strive.Tests.Utils
{
    public static class TestOutputHelperUtils
    {
        public static ILogger<T> CreateLogger<T>(this ITestOutputHelper output)
        {
            var factory = output.CreateLoggerFactory();
            return factory.CreateLogger<T>();
        }

        public static ILoggerFactory CreateLoggerFactory(this ITestOutputHelper output)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.TestOutput(output).CreateLogger();
            return new SerilogLoggerFactory(logger);
        }
    }
}
