import { fabric } from 'fabric';
import { IEvent } from 'fabric/fabric-impl';
import jsonPatch from 'fast-json-patch';
import _ from 'lodash';
import {
   applyObjectConfig,
   deleteObject,
   getDeleteAtVersion,
   getId,
   isLiveObj,
   objectToJson,
   setDeleteAtVersion,
} from './fabric-utils';
import LiveUpdateControl, { getUpdateControl } from './live-update-handler';
import PathCache from './path-cache';
import {
   CanvasLiveAction,
   CanvasObjectPatch,
   CanvasPushAction,
   VersionedCanvasObject,
   WhiteboardCanvas,
   WhiteboardLiveUpdateDto,
} from './types';
import WhiteboardTool, { WhiteboardToolOptions } from './whiteboard-tool';

export type LiveUpdateHandler = {
   submit: (action: CanvasLiveAction) => void;
   on: (handler: (update: WhiteboardLiveUpdateDto) => void) => void;
};

export default class WhiteboardController {
   private fc: fabric.Canvas;
   private tool: WhiteboardTool;
   private options: WhiteboardToolOptions;
   private unsubscribeTool: (() => void) | undefined;
   private currentVersion: number | undefined;
   private currentCanvas: WhiteboardCanvas | undefined;
   private currentDeletionId: string | undefined;
   private appliedLiveUpdates = new Map<
      string,
      { type: CanvasLiveAction['type']; updateControl: LiveUpdateControl<CanvasLiveAction> }
   >();
   private throttledLiveUpdate: _.DebouncedFunc<(update: CanvasLiveAction) => void> | undefined;
   private pathCache = new PathCache();

   constructor(canvas: HTMLCanvasElement, initialTool: WhiteboardTool, initialOptions: WhiteboardToolOptions) {
      this.fc = new fabric.Canvas(canvas);

      this.options = initialOptions;

      this.tool = initialTool;
      this.setTool(initialTool);

      this.fc.on('object:modified', this.onObjectModified.bind(this));
      this.fc.on('object:removed', this.onObjectRemoved.bind(this));
      this.fc.on('object:moving', this.onObjectModifying.bind(this));
      this.fc.on('object:scaling', this.onObjectModifying.bind(this));
      this.fc.on('object:rotating', this.onObjectModifying.bind(this));
      this.fc.on('object:skewing', this.onObjectModifying.bind(this));

      this.fc.on('mouse:down', this.onMouseDown.bind(this));
      this.fc.on('mouse:move', this.onMouseMove.bind(this));
      this.fc.on('mouse:up', this.onMouseUp.bind(this));
      this.fc.on('mouse:out', this.onMouseOut.bind(this));

      this.fc.setWidth(1280);
      this.fc.setHeight(720);
   }

   public pushAction: ((action: CanvasPushAction) => Promise<number>) | undefined;

   public setupLiveUpdateHandler(handler: LiveUpdateHandler): void {
      handler.on(this.onLiveUpdateReceived.bind(this));
      this.throttledLiveUpdate = _.throttle((update) => {
         handler.submit(update);
      }, 1000 / 15);
   }

   public updateCanvas(canvas: WhiteboardCanvas) {
      canvas = this.pathCache.preprocess(canvas); // paths must be unpacked from point array to svg paths
      const newVersion = _.maxBy(canvas.objects, (x) => x.version)?.version ?? 0;

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

         const existingObjects = Object.fromEntries(
            this.fc
               .getObjects()
               .map((x) => [getId(x) as string, x])
               .filter((x) => x[0]),
         );
         const newObjects = new Array<VersionedCanvasObject>();

         for (const obj of updatedObjects) {
            const existing = existingObjects[obj.id];
            if (existing) {
               applyObjectConfig(existing, obj.data);
            } else {
               newObjects.push(obj);
            }
         }

         const checkObjDeleted = (obj: fabric.Object) => {
            const objId = getId(obj);
            if (objId && !canvas.objects.find((y) => objId === y.id)) {
               return true;
            }

            const deleteAt = getDeleteAtVersion(obj);
            if (deleteAt !== undefined && newVersion >= deleteAt) {
               return true;
            }

            return false;
         };

         const deletedObjects = this.fc.getObjects().filter(checkObjDeleted);
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

         this.fc.absolutePan(new fabric.Point(canvas.panX, canvas.panY));
      }

      this.currentVersion = newVersion;
      this.currentCanvas = canvas;
      (this.fc as any).currentPan = { x: canvas.panX, y: canvas.panY };
   }

   /** object:modified at the end of a transform or any change when statefull is true */
   private onObjectModified(e: IEvent) {
      if (!e.target) return;

      if (e.target.type === 'activeSelection') {
         const selection = e.target as fabric.ActiveSelection;

         //Discard group to get position relative to canvas and not group selection
         this.fc.discardActiveObject();

         this.onUpdateObjects(selection.getObjects());
      } else {
         this.onUpdateObjects([e.target]);
      }
   }

   private onUpdateObjects(objects: fabric.Object[]): void {
      const patches = this.generatePatch(objects);
      if (patches.length === 0) return;

      this.pushActionAndForget({ type: 'update', patches });
   }

   private onLiveUpdateObjects(objects: fabric.Object[]): void {
      const patches = this.generatePatch(objects);
      if (patches.length === 0) return;

      this.throttledLiveUpdate?.({ type: 'modifying', patches });
   }

   private generatePatch(objects: fabric.Object[]): CanvasObjectPatch[] {
      if (!this.currentCanvas) return [];

      const patches = new Array<CanvasObjectPatch>();
      for (const updatedObj of objects) {
         const original = this.currentCanvas.objects.find((x) => x.id === getId(updatedObj));
         if (!original) continue;

         const modified = objectToJson(updatedObj);

         const patch = jsonPatch.compare(original.data, modified);
         patches.push({ objectId: original.id, patch });
      }

      return patches;
   }

   private onObjectRemoved(e: IEvent) {
      if (!e.target) return;
      const deletionId = (e.target as any).deletionId;

      if (deletionId) {
         if (this.currentDeletionId === deletionId) return;
         this.currentDeletionId = deletionId;

         const objectIds = (e.target as any).deletedWith.filter((x: any) => !isLiveObj(x)).map((x: any) => getId(x));
         this.pushActionAndForget({ type: 'delete', objectIds: objectIds });
      } else {
         const targetId = getId(e.target);
         if (targetId) {
            this.pushActionAndForget({ type: 'delete', objectIds: [targetId] });
         }
      }
   }

   private onObjectModifying(e: IEvent) {
      if (!e.target) return;
      if (e.target.type === 'activeSelection') return;

      this.onLiveUpdateObjects([e.target as any]);
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

   private onLiveUpdateReceived(update: WhiteboardLiveUpdateDto) {
      if (!this.currentCanvas) return;

      if (update.action.type === 'end') {
         this.removeLiveUpdate(update.participantId);
         return;
      }

      const existingHandler = this.appliedLiveUpdates.get(update.participantId);
      if (existingHandler) {
         if (update.action.type !== existingHandler.type) {
            this.removeLiveUpdate(update.participantId);
         } else {
            existingHandler.updateControl.apply(this.fc, update.action, this.currentCanvas);
            this.fc.requestRenderAll();
            return;
         }
      }

      const updateControl = getUpdateControl(update.action.type);
      updateControl.apply(this.fc, update.action, this.currentCanvas);

      this.appliedLiveUpdates.set(update.participantId, { updateControl, type: update.action.type });
      this.fc.requestRenderAll();
   }

   public updateParticipants(participantIds: string[]) {
      for (const appliedParticipantId of this.appliedLiveUpdates.keys()) {
         if (!participantIds.includes(appliedParticipantId)) {
            this.removeLiveUpdate(appliedParticipantId);
         }
      }
   }

   private removeLiveUpdate(participantId: string) {
      if (!this.currentCanvas) return;

      const update = this.appliedLiveUpdates.get(participantId);
      if (!update) return;

      update.updateControl.delete(this.fc, this.currentCanvas);
      this.appliedLiveUpdates.delete(participantId);
   }

   public setTool(tool: WhiteboardTool) {
      this.unsubscribeTool?.();

      this.tool = tool;

      const onToolUpdateHandler = this.onToolUpdate.bind(this);
      const onToolUpdatingHandler = this.onToolUpdating.bind(this);
      const onToolAddObjectHandler = this.onToolAddObject.bind(this);

      tool.on('update', onToolUpdateHandler);
      tool.on('updating', onToolUpdatingHandler);
      tool.on('addObj', onToolAddObjectHandler);

      this.unsubscribeTool = () => {
         tool.dispose();
         tool.off('update', onToolUpdateHandler);
         tool.off('updating', onToolUpdatingHandler);
         tool.off('addObj', onToolAddObjectHandler);
      };

      tool.configureCanvas(this.fc);
      tool.applyOptions(this.options);

      this.fc.discardActiveObject();
      this.fc.requestRenderAll();
   }

   private onToolUpdate(update: CanvasPushAction): void {
      this.pushActionAndForget(update);
   }

   private onToolUpdating(update: CanvasLiveAction): void {
      this.throttledLiveUpdate?.(update);
   }

   private async onToolAddObject(update: CanvasPushAction, obj: fabric.Object) {
      if (!this.pushAction) return;

      let version: number;
      try {
         version = await this.pushAction(update);
      } catch (error) {
         return;
      }

      if (this.currentVersion !== undefined && this.currentVersion >= version) {
         this.fc.remove(obj);
         return;
      }

      setDeleteAtVersion(obj, version);
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
      this.pushActionAndForget({
         type: 'delete',
         objectIds: this.fc
            .getObjects()
            .map((x) => getId(x))
            .filter((x): x is string => !!x),
      });
   }

   public deleteSelection() {
      const selected = this.fc.getActiveObject();
      deleteObject(this.fc, selected);
   }

   private pushActionAndForget(action: CanvasPushAction) {
      try {
         this.pushAction?.(action).catch(() => {
            // ignore
         });
      } catch (error) {
         // ignore
      }
   }
}
