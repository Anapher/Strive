#pragma warning disable 8618

namespace Strive.Core.Services.Whiteboard.CanvasData
{
    public abstract record CanvasObject
    {
        public abstract string Type { get; }
        public string OriginX { get; set; }
        public string OriginY { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string? Fill { get; set; }
        public string? Stroke { get; set; }
        public int StrokeWidth { get; set; }
        public string StrokeLineCap { get; set; }
        public int StrokeDashOffset { get; set; }
        public string StrokeLineJoin { get; set; }
        public bool StrokeUniform { get; set; }
        public int StrokeMiterLimit { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double Angle { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }
        public double Opacity { get; set; }
        public bool Visible { get; set; }
        public string? BackgroundColor { get; set; }
        public string FillRule { get; set; }
        public string PaintFirst { get; set; }
        public string GlobalCompositeOperation { get; set; }
        public double SkewX { get; set; }
        public double SkewY { get; set; }
    }
}
