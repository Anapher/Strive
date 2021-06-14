import { fabric } from 'fabric';
import { IEvent } from 'fabric/fabric-impl';
import WhiteboardTool from './whiteboard-tool';

export default class WhiteboardController {
   private fc: fabric.Canvas;
   private tool: WhiteboardTool;

   constructor(canvas: HTMLCanvasElement, initialTool: WhiteboardTool) {
      this.fc = new fabric.Canvas(canvas);

      this.tool = initialTool;
      this.setTool(initialTool);

      this.fc.on('object:added', this.onObjectAdded.bind(this));
      this.fc.on('object:modified', this.onObjectModified.bind(this));
      this.fc.on('object:removed', this.onObjectRemoved.bind(this));

      this.fc.on('mouse:down', this.onMouseDown.bind(this));
      this.fc.on('mouse:move', this.onMouseMove.bind(this));
      this.fc.on('mouse:up', this.onMouseUp.bind(this));
      this.fc.on('mouse:out', this.onMouseOut.bind(this));

      this.fc.setWidth(1200);
      this.fc.setHeight(800);

      this.fc.setBackgroundColor('white', () => {
         //asd
      });
   }

   private onObjectAdded(e: IEvent) {
      console.log('added', e);
   }

   /** object:modified at the end of a transform or any change when statefull is true */
   private onObjectModified(e: IEvent) {
      console.log('modified', e);
   }

   private onObjectRemoved(e: IEvent) {
      console.log('removed', e);
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
      this.tool = tool;

      this.fc.discardActiveObject();
      this.fc.requestRenderAll();

      tool.configureCanvas(this.fc);
      tool.applyOptions({ lineWidth: 5, color: 'red', fontSize: 36 });
   }

   get selectedTool(): WhiteboardTool {
      return this.tool;
   }
}
