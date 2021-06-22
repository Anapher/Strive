export type Point = { x: number; y: number };

export type CanvasObjectInfo = {
   scaleX: 1;
   scaleY: 1;
};

export type FreeDrawingAction = {
   type: 'free-drawing';

   color: string;
   width: number;

   appendPoints: Point[];
};

export type AddPathAction = {
   type: 'add-path';

   color: string;
   width: number;

   points: Point[];
};

export type PanningAction = {
   type: 'panning';
   moveBy: Point;
};

export type PanAction = {
   type: 'pan';
   moveBy: Point;
};

export type ObjectAddedAction = {
   type: 'added';
   obj: any;
   id: string;
};

export type WhiteboardAction = FreeDrawingAction | PanningAction | PanAction | ObjectAddedAction;
