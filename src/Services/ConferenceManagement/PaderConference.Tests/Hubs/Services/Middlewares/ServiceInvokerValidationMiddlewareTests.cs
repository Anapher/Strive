using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using PaderConference.Hubs.Services;
using PaderConference.Hubs.Services.Middlewares;
using Xunit;

namespace PaderConference.Tests.Hubs.Services.Middlewares
{
    public class ServiceInvokerValidationMiddlewareTests : MiddlewareTestBase
    {
        protected override IServiceRequestBuilder<string> Execute(IServiceRequestBuilder<string> builder)
        {
            return builder.ValidateObject(132);
        }

        [Fact]
        public async Task ValidateObject_ValidatorDoesNotExist_Succeed()
        {
            // arrange
            var context = CreateContext();

            // act
            var result = await ServiceInvokerValidationMiddleware.ValidateObject(context, new TestObj());

            // assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ValidateObject_ValidationSucceeds_Succeed()
        {
            // arrange
            var context = CreateContext(builder =>
                builder.RegisterInstance(new TestObjValidator()).AsImplementedInterfaces());

            // act
            var result =
                await ServiceInvokerValidationMiddleware.ValidateObject(context, new TestObj {HelloWorld = "test"});

            // assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ValidateObject_ValidationFails_Failed()
        {
            // arrange
            var context = CreateContext(builder =>
                builder.RegisterInstance(new TestObjValidator()).AsImplementedInterfaces());

            // act
            var result = await ServiceInvokerValidationMiddleware.ValidateObject(context, new TestObj());

            // assert
            Assert.False(result.Success);
        }

        private class TestObj
        {
            public string? HelloWorld { get; set; }
        }

        private class TestObjValidator : AbstractValidator<TestObj>
        {
            public TestObjValidator()
            {
                RuleFor(x => x.HelloWorld).NotEmpty();
            }
        }
    }
}
