namespace Strive.Core.Services.WhiteboardService.CanvasData
{
    public record CanvasLine : CanvasObject
    {
        public override string Type => "line";

        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
    }
}
