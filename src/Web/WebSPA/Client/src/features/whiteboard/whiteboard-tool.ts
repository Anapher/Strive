/* eslint-disable @typescript-eslint/no-unused-vars */
import { fabric } from 'fabric';
import { Canvas, IEvent } from 'fabric/fabric-impl';
import { TypedEmitter } from 'tiny-typed-emitter';
import { AddCanvasPushAction, CanvasLiveAction, CanvasPushAction } from './types';

export type WhiteboardToolOptions = {
   lineWidth: number;
   color: string;
   fontSize: number;
};

export default interface WhiteboardTool extends TypedEmitter<WebRtcConnectionEvents> {
   /** called once when the tool is selected */
   configureCanvas(fs: Canvas): void;

   /** called immediately after configureCanvas() and when selected if the options changed */
   applyOptions(options: WhiteboardToolOptions): void;

   configureNewObjects(obj: fabric.Object[]): void;

   onMouseDown(event: IEvent): void;
   onMouseUp(event: IEvent): void;
   onMouseMove(event: IEvent): void;
   onMouseOut(event: IEvent): void;

   dispose(): void;
}

interface WebRtcConnectionEvents {
   /** Add an object by issuing a push action and handle replacing the locally added object with the update */
   addObj: (action: AddCanvasPushAction, obj: fabric.Object) => void;

   update: (action: CanvasPushAction) => void;
   updating: (action: CanvasLiveAction) => void;
}

export abstract class WhiteboardToolBase extends TypedEmitter<WebRtcConnectionEvents> implements WhiteboardTool {
   configureNewObjects(obj: fabric.Object[]): void {
      // do nothing
   }
   protected fs: Canvas | undefined;
   protected options: WhiteboardToolOptions | undefined;

   configureCanvas(fs: Canvas): void {
      this.fs = fs;
   }

   applyOptions(options: WhiteboardToolOptions): void {
      this.options = options;
   }

   onMouseDown(event: IEvent): void {
      // do nothing
   }

   onMouseUp(event: IEvent): void {
      // do nothing
   }

   onMouseMove(event: IEvent): void {
      // do nothing
   }

   onMouseOut(event: IEvent): void {
      // do nothing
   }

   dispose(): void {
      // do nothing
   }

   protected getCanvas(): Canvas {
      if (!this.fs) throw new Error('Tool not initialized');
      return this.fs;
   }

   protected getOptions(): WhiteboardToolOptions {
      if (!this.options) throw new Error('Tool not initialized');
      return this.options;
   }
}
