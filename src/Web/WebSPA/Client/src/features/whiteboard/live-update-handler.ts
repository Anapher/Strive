import { fabric } from 'fabric';
import { applyPatch } from 'fast-json-patch';
import _ from 'lodash';
import { applyObjectConfig, getId, setIsLiveObj } from './fabric-utils';
import {
   CanvasLiveAction,
   DrawingLineCanvasLiveAction,
   FreeDrawingCanvasLiveAction,
   ModifyingObjectsCanvasLiveAction,
   PanningCanvasLiveAction,
   WhiteboardCanvas,
} from './types';

export default interface LiveUpdateControl<T extends CanvasLiveAction> {
   apply(fc: fabric.Canvas, action: T, currentCanvas: WhiteboardCanvas): void;
   delete(fc: fabric.Canvas, currentCanvas: WhiteboardCanvas): void;
}

export function getUpdateControl<T extends CanvasLiveAction>(type: T['type']): LiveUpdateControl<T> {
   switch (type) {
      case 'panning':
         return new PanningCanvasLiveUpdateControl() as any;
      case 'drawingLine':
         return new DrawingLineCanvasLiveUpdateControl() as any;
      case 'modifying':
         return new ModifyingObjectsCanvasLiveUpdateControl() as any;
      case 'freeDrawing':
         return new FreeDrawingCanvasLiveUpdateControl() as any;
      default:
         throw new Error();
   }
}

class DrawingLineCanvasLiveUpdateControl implements LiveUpdateControl<DrawingLineCanvasLiveAction> {
   private currentLine: fabric.Line | undefined;

   apply(fc: fabric.Canvas, action: DrawingLineCanvasLiveAction): void {
      if (this.currentLine) {
         this.currentLine.set({ x1: action.startX, y1: action.startY, x2: action.endX, y2: action.endY });
         return;
      }

      this.currentLine = new fabric.Line([action.startX, action.startY, action.endX, action.endY], {
         strokeWidth: action.strokeWidth,
         fill: action.color,
         stroke: action.color,
         originX: 'center',
         originY: 'center',
         selectable: false,
         evented: false,
      });
      setIsLiveObj(this.currentLine);
      fc.add(this.currentLine);
   }

   delete(fc: fabric.Canvas): void {
      if (this.currentLine) {
         fc.remove(this.currentLine);
      }
   }
}

class PanningCanvasLiveUpdateControl implements LiveUpdateControl<PanningCanvasLiveAction> {
   apply(fc: fabric.Canvas, action: PanningCanvasLiveAction): void {
      fc.absolutePan(new fabric.Point(action.panX, action.panY));
   }

   delete(fc: fabric.Canvas, currentCanvas: WhiteboardCanvas): void {
      fc.absolutePan(new fabric.Point(currentCanvas.panX, currentCanvas.panY));
   }
}

class ModifyingObjectsCanvasLiveUpdateControl implements LiveUpdateControl<ModifyingObjectsCanvasLiveAction> {
   private modifiedObjs = new Array<string>();

   apply(fc: fabric.Canvas, action: ModifyingObjectsCanvasLiveAction, currentCanvas: WhiteboardCanvas): void {
      const objects = fc.getObjects();

      const previousEdited = [...this.modifiedObjs];
      this.modifiedObjs = [];

      for (const { objectId, patch } of action.patches) {
         const obj = objects.find((x) => getId(x) === objectId);
         if (!obj) continue;

         const existing = currentCanvas.objects.find((x) => x.id === objectId);
         if (!existing) continue;

         const data = _.cloneDeep(existing.data);
         applyPatch(data, patch);

         obj.set(data);
         this.modifiedObjs.push(objectId);
      }

      for (const id of previousEdited.filter((x) => !this.modifiedObjs.includes(x))) {
         this.resetObject(id, fc, currentCanvas);
      }
   }

   delete(fc: fabric.Canvas, currentCanvas: WhiteboardCanvas): void {
      for (const id of this.modifiedObjs) {
         this.resetObject(id, fc, currentCanvas);
      }

      this.modifiedObjs = [];
   }

   private resetObject(id: string, fc: fabric.Canvas, currentCanvas: WhiteboardCanvas) {
      const existing = fc.getObjects().find((x) => getId(x) === id);
      if (!existing) return;

      const actualObj = currentCanvas.objects.find((x) => x.id === id);
      if (!actualObj) return;

      applyObjectConfig(existing, actualObj.data);
   }
}

class FreeDrawingCanvasLiveUpdateControl implements LiveUpdateControl<FreeDrawingCanvasLiveAction> {
   private currentBrush: any | undefined;

   apply(fc: fabric.Canvas, action: FreeDrawingCanvasLiveAction): void {
      let index = 0;
      if (!this.currentBrush) {
         this.currentBrush = new fabric.PencilBrush();
         this.currentBrush.initialize(fc);
         this.currentBrush.color = action.color;
         this.currentBrush.width = action.width;
         this.currentBrush.onMouseDown(action.appendPoints[index++], { e: { isPrimary: true } });
      }

      for (; index < action.appendPoints.length; index++) {
         this.currentBrush.onMouseMove(action.appendPoints[index], { e: { isPrimary: true } });
      }
   }

   delete(): void {
      if (this.currentBrush) {
         this.currentBrush.oldEnd = undefined;
         this.currentBrush._reset();
      }
   }
}
