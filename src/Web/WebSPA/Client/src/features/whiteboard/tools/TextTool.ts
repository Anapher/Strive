import { fabric } from 'fabric';
import { Canvas, IEvent } from 'fabric/fabric-impl';
import _ from 'lodash';
import { objectToJson } from '../fabric-utils';
import { WhiteboardToolBase } from '../whiteboard-tool';

export default class TextTool extends WhiteboardToolBase {
   private pendingTexts = new Array<fabric.IText>();

   configureCanvas(canvas: Canvas) {
      super.configureCanvas(canvas);

      canvas.isDrawingMode = false;
      canvas.selection = false;

      canvas.forEachObject((o) => {
         o.selectable = false;
         o.evented = false;
      });

      canvas.defaultCursor = 'text';
   }

   configureNewObjects(obj: fabric.Object[]): void {
      obj.forEach((o) => {
         o.selectable = false;
         o.evented = false;
      });
   }

   dispose() {
      for (const text of this.pendingTexts) {
         text.exitEditing();
      }
   }

   onMouseDown(event: IEvent): void {
      if (event.target) return;

      const canvas = this.getCanvas();
      if (canvas.getActiveObject()) return;

      const pointer = canvas.getPointer(event.e);
      const iText = new fabric.IText('', {
         fontFamily: 'Roboto, Helvetica, Arial, sans-serif',
         fontSize: this.getOptions().fontSize,
         fill: this.getOptions().color,
      });

      const opts = {
         left: pointer.x - (iText.width ?? 0),
         top: pointer.y - (iText.height ?? 0),
      };

      iText.set({
         left: opts.left,
         top: opts.top,
      });

      canvas.setActiveObject(iText);
      canvas.add(iText);

      iText.enterEditing();
      iText.hiddenTextarea?.focus();

      iText.on('editing:exited', () => {
         if (!iText.text) {
            canvas.remove(iText);
            canvas.requestRenderAll();
         }
      });

      const unsubscribeHandler: { unsub: () => void } = {
         unsub: () => {
            //empty
         },
      };

      const addHandler = () => {
         if (iText.text) {
            this.emit('addObj', { type: 'add', object: objectToJson(iText) }, iText);
         }

         _.remove(this.pendingTexts, iText);

         unsubscribeHandler.unsub();
      };

      iText.on('editing:exited', addHandler);

      unsubscribeHandler.unsub = () => {
         iText.off('editing:exited', addHandler);
      };

      this.pendingTexts.push(iText);
   }
}
