namespace PaderConference.Core.Services.Equipment
{
    public record UseMediaStateInfo(bool Connected, bool Enabled, bool Paused, CurrentStreamInfo? StreamInfo);
}
