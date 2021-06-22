using MediatR;
using Moq;
using Strive.Core.Services.Whiteboard.Actions;
using Strive.Core.Services.Whiteboard.UseCases;

namespace Strive.Core.Tests.Services.Whiteboard.UseCases
{
    public class PushActionUseCaseTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<ICanvasActionUtils> _actionUtils = new();

        private PushActionUseCase Create()
        {
            return new(_mediator.Object, _actionUtils.Object);
        }

        //public async Task Handle
    }
}
