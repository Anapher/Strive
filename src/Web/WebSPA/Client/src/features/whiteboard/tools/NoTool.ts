import { Canvas } from 'fabric/fabric-impl';
import { WhiteboardToolBase } from '../whiteboard-tool';

export default class NoTool extends WhiteboardToolBase {
   configureCanvas(canvas: Canvas) {
      super.configureCanvas(canvas);

      canvas.isDrawingMode = false;
      canvas.selection = false;

      canvas.forEachObject((o) => {
         o.selectable = false;
         o.evented = false;
      });

      canvas.defaultCursor = 'default';
   }

   configureNewObjects(obj: fabric.Object[]): void {
      obj.forEach((o) => {
         o.selectable = false;
         o.evented = false;
      });
   }
}
