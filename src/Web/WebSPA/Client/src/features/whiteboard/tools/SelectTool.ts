import { Canvas } from 'fabric/fabric-impl';
import { WhiteboardToolBase } from '../whiteboard-tool';

export default class SelectTool extends WhiteboardToolBase {
   configureCanvas(canvas: Canvas) {
      super.configureCanvas(canvas);

      canvas.isDrawingMode = false;
      canvas.selection = true;
      canvas.forEachObject((o) => {
         o.selectable = true;
         o.evented = true;
      });

      canvas.defaultCursor = 'default';
   }

   configureNewObjects(obj: fabric.Object[]): void {
      obj.forEach((o) => {
         o.selectable = true;
         o.evented = true;
      });
   }
}
