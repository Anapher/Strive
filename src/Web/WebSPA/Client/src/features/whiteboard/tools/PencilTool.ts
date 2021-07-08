import { Canvas, IEvent, Point } from 'fabric/fabric-impl';
import { getId, objectToJson } from '../fabric-utils';
import { compressPathData } from '../path-compression';
import { WhiteboardToolBase, WhiteboardToolOptions } from '../whiteboard-tool';

export default class PencilTool extends WhiteboardToolBase {
   private isDown = false;
   private lastUpdateIndex = 0;
   private disposeAction: (() => void) | undefined;

   constructor() {
      super();
   }

   configureCanvas(canvas: Canvas) {
      super.configureCanvas(canvas);

      (canvas.freeDrawingBrush as any).decimate = 5;
      canvas.isDrawingMode = true;
      canvas.defaultCursor = 'default';
      this.lastUpdateIndex = 0;

      this.disposeAction?.();

      const handler = this.onElementAdded.bind(this);
      canvas.on('object:added', handler);

      this.disposeAction = () => {
         canvas.off('object:added', handler);
      };
   }

   dispose() {
      this.disposeAction?.();
      this.disposeAction = undefined;
   }

   applyOptions({ lineWidth, color }: WhiteboardToolOptions) {
      const canvas = this.getCanvas();

      canvas.freeDrawingBrush.width = lineWidth;
      canvas.freeDrawingBrush.color = color;
   }

   onMouseDown(): void {
      this.lastUpdateIndex = 0;
      this.isDown = true;
   }

   onMouseUp(): void {
      this.isDown = false;
      this.emit('updating', { type: 'end' });
   }

   onMouseMove(): void {
      this.onUpdatePath();
   }

   onUpdatePath(): void {
      if (!this.isDown) return;

      const canvas = this.getCanvas();
      const points = (canvas.freeDrawingBrush as any)._points as Point[];

      const addedPoints = points.slice(this.lastUpdateIndex);
      this.lastUpdateIndex = points.length;

      this.emit('updating', {
         type: 'freeDrawing',
         color: canvas.freeDrawingBrush.color,
         width: canvas.freeDrawingBrush.width,
         appendPoints: addedPoints.map(({ x, y }) => ({ x, y })),
      });
   }

   onElementAdded(e: IEvent) {
      if (!e.target) return;
      if (e.target.type !== 'path') return;
      if (getId(e.target)) return;

      const data = objectToJson(e.target);
      compressPathData(data);

      this.emit('update', { type: 'add', object: data });
   }
}
