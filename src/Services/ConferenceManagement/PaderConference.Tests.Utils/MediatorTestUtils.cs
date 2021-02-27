using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Xunit;

namespace PaderConference.Tests.Utils
{
    public static class MediatorTestUtils
    {
        public static CapturedRequest<T, TResponse> CaptureRequest<T, TResponse>(this Mock<IMediator> mediator)
            where T : IRequest<TResponse>
        {
            var returnVal = new TaskCompletionSource<T>();

            mediator.Setup(x => x.Send(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<TResponse> request, CancellationToken _) => returnVal.SetResult((T) request));

            return new CapturedRequest<T, TResponse>(mediator, returnVal.Task);
        }

        public static CapturedRequest<T, TResponse> HandleRequest<T, TResponse>(this Mock<IMediator> mediator,
            Func<T, TResponse> responseFactory) where T : IRequest<TResponse>
        {
            var returnVal = new TaskCompletionSource<T>();

            mediator.Setup(x => x.Send(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<TResponse> request, CancellationToken _) => returnVal.SetResult((T) request))
                .ReturnsAsync((IRequest<TResponse> request, CancellationToken _) => responseFactory((T) request));

            return new CapturedRequest<T, TResponse>(mediator, returnVal.Task);
        }

        public static CapturedRequest<T, TResponse> HandleRequest<T, TResponse>(this Mock<IMediator> mediator,
            TResponse response) where T : IRequest<TResponse>
        {
            return mediator.HandleRequest<T, TResponse>(_ => response);
        }

        public static CapturedNotification<T> CaptureNotification<T>(this Mock<IMediator> mediator)
            where T : INotification
        {
            var returnVal = new TaskCompletionSource<T>();

            mediator.Setup(x => x.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>())).Callback(
                (INotification notification, CancellationToken _) => returnVal.SetResult((T) notification));

            return new CapturedNotification<T>(mediator, returnVal.Task);
        }
    }

    public class CapturedRequest<T, TResponse> where T : IRequest<TResponse>
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Task<T> _task;
        private bool _asserted;

        public CapturedRequest(Mock<IMediator> mediator, Task<T> task)
        {
            _mediator = mediator;
            _task = task;
        }

        public void AssertReceived()
        {
            if (_asserted) return;

            Assert.True(_task.IsCompleted);
            _mediator.Verify(x => x.Send(It.IsAny<T>(), It.IsAny<CancellationToken>()));

            _asserted = true;
        }

        public void AssertNotReceived()
        {
            Assert.False(_task.IsCompleted);
        }

        public T GetRequest()
        {
            AssertReceived();
            return _task.Result;
        }
    }

    public class CapturedNotification<T> where T : INotification
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Task<T> _task;
        private bool _asserted;

        public CapturedNotification(Mock<IMediator> mediator, Task<T> task)
        {
            _mediator = mediator;
            _task = task;
        }

        public void AssertReceived()
        {
            if (_asserted) return;

            Assert.True(_task.IsCompleted);
            _mediator.Verify(x => x.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>()));

            _asserted = true;
        }

        public T GetNotification()
        {
            AssertReceived();
            return _task.Result;
        }
    }
}
