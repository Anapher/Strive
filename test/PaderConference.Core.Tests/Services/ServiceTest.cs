using System;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Services;
using Serilog;
using Serilog.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services
{
    public abstract class ServiceTest<T> where T : IConferenceService
    {
        protected ILogger<T> Logger;

        protected ServiceTest(ITestOutputHelper output)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.TestOutput(output).CreateLogger();

            Logger = new SerilogLoggerFactory(logger).CreateLogger<T>();
        }

        protected void AssertSuccess(SuccessOrError successOrError)
        {
            Assert.True(successOrError.Success, $"Method failed with {successOrError.Error}");
        }

        protected void AssertFailed(SuccessOrError successOrError)
        {
            Assert.False(successOrError.Success, "Method succeeded");
        }

        protected TResult AssertSuccess<TResult>(SuccessOrError<TResult> successOrError)
        {
            if (!successOrError.Success)
            {
                Assert.True(successOrError.Success, $"Method failed with {successOrError.Error}");
                throw new Exception();
            }

            return successOrError.Response;
        }
    }
}
