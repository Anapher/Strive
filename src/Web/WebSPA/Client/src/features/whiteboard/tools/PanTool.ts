import { fabric } from 'fabric';
import { Canvas, IEvent } from 'fabric/fabric-impl';
import { Point } from '../types';
import { WhiteboardToolBase } from '../whiteboard-tool';

export default class PanTool extends WhiteboardToolBase {
   private startPoint: undefined | Point;
   private totalPanned: undefined | Point;

   configureCanvas(canvas: Canvas) {
      super.configureCanvas(canvas);

      canvas.isDrawingMode = false;
      canvas.selection = false;

      canvas.forEachObject((o) => {
         o.selectable = false;
         o.evented = false;
      });

      canvas.defaultCursor = 'grab';
   }

   configureNewObjects(obj: fabric.Object[]): void {
      obj.forEach((o) => {
         o.selectable = false;
         o.evented = false;
      });
   }

   onMouseDown(event: IEvent): void {
      const canvas = this.getCanvas();

      const pointer = canvas.getPointer(event.e);
      this.startPoint = pointer;
      this.totalPanned = undefined;
   }

   onMouseMove(event: IEvent): void {
      if (!this.startPoint) return;

      const canvas = this.getCanvas();
      const pointer = canvas.getPointer(event.e);
      const moveBy: Point = { x: pointer.x - this.startPoint.x, y: pointer.y - this.startPoint.y };

      canvas.relativePan(new fabric.Point(moveBy.x, moveBy.y));
      canvas.requestRenderAll();

      if (!this.totalPanned) {
         this.totalPanned = { x: 0, y: 0 };
      }
      this.totalPanned = { x: this.totalPanned.x + moveBy.x, y: this.totalPanned.y + moveBy.y };

      this.emit('updating', {
         type: 'panning',
         ...this.getAbsolutePan(this.totalPanned),
      });
   }

   onMouseUp(): void {
      this.startPoint = undefined;

      if (this.totalPanned) {
         this.emit('update', {
            type: 'pan',
            ...this.getAbsolutePan(this.totalPanned),
         });
         this.emit('updating', { type: 'end' });
      }

      console.log(this.getCanvas().getObjects());
   }

   private getAbsolutePan(totalPanned: Point): { panX: number; panY: number } {
      const canvas = this.getCanvas();
      const currentPanned = ((canvas as any).currentPan as Point) ?? { x: 0, y: 0 };
      return {
         panX: currentPanned.x - totalPanned.x,
         panY: currentPanned.y - totalPanned.y,
      };
   }
}
