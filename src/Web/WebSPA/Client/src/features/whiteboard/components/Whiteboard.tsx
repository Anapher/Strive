import React, { useEffect, useRef, useState } from 'react';
import WhiteboardController from '../whiteboard-controller';
import ToolsContainer, { getTool, ToolType } from './ToolsContainer';
import '../fabric-style';

export default function Whiteboard() {
   const [selectedTool, setSelectedTool] = useState<ToolType>('select');

   const canvasRef = useRef<HTMLCanvasElement>(null);
   const controllerRef = useRef<WhiteboardController | undefined>();

   useEffect(() => {
      if (canvasRef.current && !controllerRef.current) {
         controllerRef.current = new WhiteboardController(canvasRef.current, getTool(selectedTool));
      }
   }, [canvasRef.current]);

   const handleChangeSelectedTool = (type: ToolType) => {
      setSelectedTool(type);

      if (controllerRef.current) {
         controllerRef.current.setTool(getTool(type));
      }
   };

   return (
      <div style={{ display: 'flex' }}>
         <ToolsContainer selectedTool={selectedTool} onSelectedToolChanged={handleChangeSelectedTool} />

         <div style={{ width: 1200, height: 800 }}>
            <canvas id="test-canvas" ref={canvasRef}>
               Sorry, Canvas HTML5 element is not supported by your browser :(
            </canvas>
         </div>
      </div>
   );
}
