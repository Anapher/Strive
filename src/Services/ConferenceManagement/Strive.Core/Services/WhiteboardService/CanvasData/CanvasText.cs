#pragma warning disable 8618

namespace Strive.Core.Services.WhiteboardService.CanvasData
{
    public record CanvasText : CanvasObject
    {
        public override string Type { get; } = "i-text";
        public double CharSpacing { get; set; }
        public string Direction { get; set; }
        public string FontFamily { get; set; }
        public double FontSize { get; set; }
        public string FontStyle { get; set; }
        public string FontWeight { get; set; }
        public double LineHeight { get; set; }
        public bool Linethrough { get; set; }
        public bool Overline { get; set; }
        public string Text { get; set; }
        public string TextAlign { get; set; }
        public string TextBackgroundColor { get; set; }
        public bool Underline { get; set; }
    }
}
