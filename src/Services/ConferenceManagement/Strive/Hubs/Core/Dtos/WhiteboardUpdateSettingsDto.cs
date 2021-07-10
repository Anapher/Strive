using Strive.Core.Services.WhiteboardService;

namespace Strive.Hubs.Core.Dtos
{
    public record WhiteboardUpdateSettingsDto(string WhiteboardId, WhiteboardSettings Settings);
}
