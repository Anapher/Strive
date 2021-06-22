import { Canvas, IEvent } from 'fabric/fabric-impl';
import { WhiteboardToolBase } from '../whiteboard-tool';
import { fabric } from 'fabric';
import _ from 'lodash';
import { Point } from '../action-types';

export default class PanTool extends WhiteboardToolBase {
   private startPoint: undefined | Point;
   private totalPanned: undefined | Point;
   private throttledUpdate: _.DebouncedFunc<(p: Point) => void>;

   constructor() {
      super();

      this.throttledUpdate = _.throttle(this.onUpdatePan.bind(this), 1000 / 30); // 30 FPS
   }

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

      this.throttledUpdate(moveBy);

      if (!this.totalPanned) {
         this.totalPanned = { x: 0, y: 0 };
      }
      this.totalPanned = { x: this.totalPanned.x + moveBy.x, y: this.totalPanned.y + moveBy.y };
   }

   onMouseUp(): void {
      this.startPoint = undefined;
      this.throttledUpdate.cancel();

      if (this.totalPanned) {
         this.emit('update', { type: 'pan', moveBy: this.totalPanned });
      }
   }

   onUpdatePan(moveBy: Point): void {
      this.emit('update', {
         type: 'panning',
         moveBy,
      });
   }
}
