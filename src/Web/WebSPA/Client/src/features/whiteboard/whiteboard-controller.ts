import { fabric } from 'fabric';
import { IEvent } from 'fabric/fabric-impl';
import jsonPatch from 'fast-json-patch';
import _ from 'lodash';
import { TypedEmitter } from 'tiny-typed-emitter';
import { objectToJson, setObjectAbsolutePosition } from './fabric-utils';
import { CanvasObjectPatch, CanvasPushAction, VersionedCanvasObject, WhiteboardCanvas } from './types';
import WhiteboardTool, { WhiteboardToolOptions } from './whiteboard-tool';

interface WhiteboardControllerEvents {
   pushAction: (action: CanvasPushAction) => void;
}

export default class WhiteboardController extends TypedEmitter<WhiteboardControllerEvents> {
   private fc: fabric.Canvas;
   private tool: WhiteboardTool;
   private options: WhiteboardToolOptions;
   private unsubscribeTool: (() => void) | undefined;
   private currentVersion: number | undefined;
   private currentCanvas: WhiteboardCanvas | undefined;
   private currentDeletionId: string | undefined;

   constructor(canvas: HTMLCanvasElement, initialTool: WhiteboardTool, initialOptions: WhiteboardToolOptions) {
      super();

      this.fc = new fabric.Canvas(canvas);

      this.options = initialOptions;

      this.tool = initialTool;
      this.setTool(initialTool);

      this.fc.on('object:modified', this.onObjectModified.bind(this));
      this.fc.on('object:removed', this.onObjectRemoved.bind(this));
      this.fc.on('object:moving', this.onObjectMoving.bind(this));

      this.fc.on('text:changed', this.onTextChanged.bind(this));

      this.fc.on('mouse:down', this.onMouseDown.bind(this));
      this.fc.on('mouse:move', this.onMouseMove.bind(this));
      this.fc.on('mouse:up', this.onMouseUp.bind(this));
      this.fc.on('mouse:out', this.onMouseOut.bind(this));

      this.fc.setWidth(1280);
      this.fc.setHeight(720);
   }

   public updateCanvas(canvas: WhiteboardCanvas) {
      if (!this.currentVersion) {
         this.fc.loadFromJSON(
            {
               objects: canvas.objects.map(({ id, data, version }) => ({ ...data, id, version })),
               background: canvas.backgroundColor,
            },
            () => {
               this.fc.absolutePan(new fabric.Point(canvas.panX, canvas.panY));
               this.fc.requestRenderAll();
            },
         );
      } else {
         const appliedVersion = this.currentVersion;
         const updatedObjects = canvas.objects.filter((x) => x.version > appliedVersion);

         const existingObjects = this.fc.getObjects();
         const newObjects = new Array<VersionedCanvasObject>();

         for (const obj of updatedObjects) {
            const existing = existingObjects.find((x) => (x as any).id === obj.id);
            if (existing) {
               console.log('update existing');

               existing.set(obj.data);
               setObjectAbsolutePosition(existing);
               existing.setCoords();
               existing.group?.setCoords();
            } else {
               console.log('add new');

               newObjects.push(obj);
            }
         }

         const deletedObjects = existingObjects.filter((x) => !canvas.objects.find((y) => (x as any).id === y.id));
         this.fc.remove(...deletedObjects);

         fabric.util.enlivenObjects(
            newObjects.map(({ id, data, version }) => ({ ...data, id, version })),
            (enlivenedObjects: any[]) => {
               if (enlivenedObjects.length > 0) {
                  this.fc.add(...enlivenedObjects);
                  this.tool.configureNewObjects(enlivenedObjects);
               }
               this.fc.requestRenderAll();
            },
            '',
         );
      }

      this.fc.absolutePan(new fabric.Point(canvas.panX, canvas.panY));

      this.currentVersion = _.maxBy(canvas.objects, (x) => x.version)?.version ?? 0;
      this.currentCanvas = canvas;
      (this.fc as any).currentPan = { x: canvas.panX, y: canvas.panY };
   }

   /** object:modified at the end of a transform or any change when statefull is true */
   private onObjectModified(e: IEvent) {
      if (!e.target) return;

      if (e.target.type === 'activeSelection') {
         const selection = e.target as fabric.ActiveSelection;

         // const activeObjects = this.fc.getActiveObject();

         //Discard group to get position relative to canvas and not group selection
         this.fc.discardActiveObject();

         // const objs = this.fc.getObjects().filter(x => x.id)

         this.onUpdateObjects(selection.getObjects());

         // this.fc.setActiveObject(activeObjects);
      } else {
         this.onUpdateObjects([e.target]);
      }
   }

   private onUpdateObjects(objects: fabric.Object[]): void {
      if (!this.currentCanvas) return;

      const patches = new Array<CanvasObjectPatch>();
      for (const updatedObj of objects) {
         const original = this.currentCanvas.objects.find((x) => x.id === (updatedObj as any).id);
         if (!original) continue;

         const modified = objectToJson(updatedObj);

         const patch = jsonPatch.compare(original.data, modified);
         patches.push({ objectId: original.id, patch });
      }

      console.log(patches);

      this.emit('pushAction', { type: 'update', patches });
   }

   private onTextChanged(e: IEvent) {
      console.log('text changed', e.target?.toJSON());
   }

   private onObjectRemoved(e: IEvent) {
      const deletionId = (e.target as any).deletionId;

      if (deletionId) {
         if (this.currentDeletionId === deletionId) return;
         this.currentDeletionId = deletionId;

         const objectIds = (e.target as any).deletedWith.map((x: any) => x.id);
         this.emit('pushAction', { type: 'delete', objectIds: objectIds });
      } else {
         const targetId = (e.target as any).id;
         if (targetId) {
            this.emit('pushAction', { type: 'delete', objectIds: [targetId] });
         }
      }
   }

   private onObjectMoving(e: IEvent) {
      console.log('moving', e);
   }

   private onMouseDown(e: IEvent) {
      this.tool.onMouseDown(e);
   }

   private onMouseMove(e: IEvent) {
      this.tool.onMouseMove(e);
   }

   private onMouseOut(e: IEvent) {
      this.tool.onMouseOut(e);
   }

   private onMouseUp(e: IEvent) {
      this.tool.onMouseUp(e);
   }

   public setTool(tool: WhiteboardTool) {
      this.unsubscribeTool?.();

      this.tool = tool;

      const onUpdateHandler = this.onToolUpdate.bind(this);
      tool.on('update', onUpdateHandler);

      this.unsubscribeTool = () => {
         tool.dispose();
         tool.off('update', onUpdateHandler);
      };

      tool.configureCanvas(this.fc);
      tool.applyOptions(this.options);

      this.fc.discardActiveObject();
      this.fc.requestRenderAll();
   }

   private onToolUpdate(update: CanvasPushAction): void {
      this.emit('pushAction', update);
   }

   public updateOptions(options: WhiteboardToolOptions) {
      this.options = options;
      this.tool.applyOptions(options);
   }

   get selectedTool(): WhiteboardTool {
      return this.tool;
   }

   public clearWhiteboard() {
      this.fc.discardActiveObject();
      this.emit('pushAction', { type: 'delete', objectIds: this.fc.getObjects().map((x) => (x as any).id) });
   }
}
