import { fabric } from 'fabric';
import { Canvas, IEvent } from 'fabric/fabric-impl';
import { WhiteboardToolBase } from '../whiteboard-tool';

export default class LineTool extends WhiteboardToolBase {
   private currentLine: fabric.Line | undefined;

   configureCanvas(canvas: Canvas) {
      super.configureCanvas(canvas);

      this.currentLine = undefined;

      canvas.isDrawingMode = false;
      canvas.selection = false;
      canvas.forEachObject((o) => {
         o.selectable = false;
         o.evented = false;
      });

      canvas.defaultCursor = 'default';
   }

   onMouseDown(event: IEvent): void {
      const canvas = this.getCanvas();
      const { lineWidth, color: lineColor } = this.getOptions();

      const pointer = canvas.getPointer(event.e);

      const points = [pointer.x, pointer.y, pointer.x, pointer.y];
      this.currentLine = new fabric.Line(points, {
         strokeWidth: lineWidth,
         fill: lineColor,
         stroke: lineColor,
         originX: 'center',
         originY: 'center',
         selectable: false,
         evented: false,
      });
      canvas.add(this.currentLine);
   }

   onMouseMove(event: IEvent): void {
      if (!this.currentLine) return;

      const canvas = this.getCanvas();

      const pointer = canvas.getPointer(event.e);
      this.currentLine.set({ x2: pointer.x, y2: pointer.y });
      this.currentLine.setCoords();

      canvas.renderAll();
   }

   onMouseUp(): void {
      this.onFinish();
   }

   onMouseOut(): void {
      this.onFinish();
   }

   onFinish(): void {
      if (this.currentLine) {
         this.emit('update', { type: 'added', id: '', obj: this.currentLine.toJSON() });
         this.currentLine = undefined;
      }
   }
}
