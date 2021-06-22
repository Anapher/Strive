import { fabric } from 'fabric';
import { IEvent } from 'fabric/fabric-impl';
import { WhiteboardAction } from './action-types';
import WhiteboardTool, { WhiteboardToolOptions } from './whiteboard-tool';

export default class WhiteboardController {
   private fc: fabric.Canvas;
   private tool: WhiteboardTool;
   private options: WhiteboardToolOptions;
   private unsubscribeTool: (() => void) | undefined;

   constructor(canvas: HTMLCanvasElement, initialTool: WhiteboardTool, initialOptions: WhiteboardToolOptions) {
      this.fc = new fabric.Canvas(canvas);

      this.options = initialOptions;

      this.tool = initialTool;
      this.setTool(initialTool);

      this.fc.on('object:added', this.onObjectAdded.bind(this));
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

      this.fc.setBackgroundColor('white', () => {
         //asd
      });
   }

   private onObjectAdded(e: IEvent) {
      console.log('added', e);
      const asd = e.target?.toJSON();
      console.log('JSON', asd);
   }

   /** object:modified at the end of a transform or any change when statefull is true */
   private onObjectModified(e: IEvent) {
      console.log('modified', JSON.stringify(e.target?.toJSON()));
   }

   private onTextChanged(e: IEvent) {
      console.log('text changed', e.target?.toJSON());
   }

   private onObjectRemoved(e: IEvent) {
      // console.log('removed', e);
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

   private onToolUpdate(update: WhiteboardAction): void {
      console.log('update', update);
   }

   public updateOptions(options: WhiteboardToolOptions) {
      this.options = options;
      this.tool.applyOptions(options);
   }

   get selectedTool(): WhiteboardTool {
      return this.tool;
   }

   public clearWhiteboard() {
      this.fc.clear();
      this.fc.setWidth(1280);
      this.fc.setHeight(720);

      this.fc.setBackgroundColor('white', () => {
         //asd
      });
   }
}
