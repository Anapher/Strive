import { Operation } from 'fast-json-patch';

export type Point = { x: number; y: number };

export type CanvasObject = {
   type: 'path' | 'i-text' | 'line';
};

export type VersionedCanvasObject = {
   data: CanvasObject;
   id: string;
   version: number;
};

export type WhiteboardCanvas = {
   objects: VersionedCanvasObject[];
   backgroundColor: string;
   panX: number;
   panY: number;
};

export type SynchronizedParticipantState = {
   canUndo: boolean;
   canRedo: boolean;
};

export type WhiteboardInfo = {
   friendlyName: string;
   everyoneCanEdit: boolean;
   version: number;
   canvas: WhiteboardCanvas;

   participantStates: Record<string, SynchronizedParticipantState>;
};

export type SynchronizedWhiteboards = {
   whiteboards: Record<string, WhiteboardInfo>;
};

export type AddCanvasPushAction = {
   type: 'add';
   object: CanvasObject;
};

export type DeleteCanvasPushAction = {
   type: 'delete';
   objectIds: string[];
};

export type PanCanvasPushAction = {
   type: 'pan';
   panX: number;
   panY: number;
};

export type CanvasObjectPatch = {
   objectId: string;
   patch: Operation[];
};

export type UpdateCanvasPushAction = {
   type: 'update';
   patches: CanvasObjectPatch[];
};

export type CanvasPushAction =
   | AddCanvasPushAction
   | DeleteCanvasPushAction
   | PanCanvasPushAction
   | UpdateCanvasPushAction;

export type WhiteboardPushActionDto = {
   whiteboardId: string;
   action: CanvasPushAction;
};

export type DrawingLineCanvasLiveAction = {
   type: 'drawingLine';
   color: string;
   strokeWidth: number;
   startX: number;
   startY: number;
   endX: number;
   endY: number;
};

export type FreeDrawingCanvasLiveAction = {
   type: 'freeDrawing';
   color: string;
   width: number;

   appendPoints: Point[];
};

export type PanningCanvasLiveAction = {
   type: 'panning';

   panX: number;
   panY: number;
};

export type ModifyingObjectsCanvasLiveAction = {
   type: 'modifying';

   patches: CanvasObjectPatch[];
};

export type EndCanvasLiveAction = {
   type: 'end';
};

export type CanvasLiveAction =
   | DrawingLineCanvasLiveAction
   | FreeDrawingCanvasLiveAction
   | PanningCanvasLiveAction
   | ModifyingObjectsCanvasLiveAction
   | EndCanvasLiveAction;

export type WhiteboardLiveUpdateDto = {
   whiteboardId: string;
   participantId: string;
   action: CanvasLiveAction;
};

export type WhiteboardLiveActionDto = {
   whiteboardId: string;
   action: CanvasLiveAction;
};
