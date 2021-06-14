import { Canvas } from 'fabric/fabric-impl';
import { WhiteboardToolBase, WhiteboardToolOptions } from '../whiteboard-tool';

export class PencilTool extends WhiteboardToolBase {
   configureCanvas(canvas: Canvas) {
      super.configureCanvas(canvas);

      canvas.isDrawingMode = true;
   }

   applyOptions({ lineWidth, color }: WhiteboardToolOptions) {
      const canvas = this.getCanvas();

      canvas.freeDrawingBrush.width = lineWidth;
      canvas.freeDrawingBrush.color = color;
   }
}
