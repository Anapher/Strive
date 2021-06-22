/* eslint-disable @typescript-eslint/no-unused-vars */
import EventEmitter from 'events';
import { Canvas, IEvent } from 'fabric/fabric-impl';
import { TypedEmitter } from 'tiny-typed-emitter';
import { WhiteboardAction } from './action-types';

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

   onMouseDown(event: IEvent): void;
   onMouseUp(event: IEvent): void;
   onMouseMove(event: IEvent): void;
   onMouseOut(event: IEvent): void;

   dispose(): void;
}

interface WebRtcConnectionEvents {
   update: (action: WhiteboardAction) => void;
}

export abstract class WhiteboardToolBase extends TypedEmitter<WebRtcConnectionEvents> implements WhiteboardTool {
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
