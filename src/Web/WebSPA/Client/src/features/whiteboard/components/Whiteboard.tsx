import React, { useEffect, useRef, useState } from 'react';
import '../fabric-style';
import { CanvasPushAction, WhiteboardCanvas } from '../types';
import WhiteboardController from '../whiteboard-controller';
import { WhiteboardToolOptions } from '../whiteboard-tool';
import ToolsContainer, { getTool, ToolType } from './ToolsContainer';

type Props = {
   canvas: WhiteboardCanvas;
   onPushAction: (action: CanvasPushAction) => void;
   canUndo: boolean;
   onUndo: () => void;
   canRedo: boolean;
   onRedo: () => void;
};

export default function Whiteboard({ canvas, onPushAction, canUndo, onUndo, canRedo, onRedo }: Props) {
   const [selectedTool, setSelectedTool] = useState<ToolType>('select');
   const [options, setOptions] = useState<WhiteboardToolOptions>({ color: 'black', fontSize: 36, lineWidth: 8 });

   const canvasRef = useRef<HTMLCanvasElement>(null);
   const controllerRef = useRef<WhiteboardController | undefined>();

   useEffect(() => {
      if (canvasRef.current && !controllerRef.current) {
         controllerRef.current = new WhiteboardController(canvasRef.current, getTool(selectedTool), options);
      }
   }, [canvasRef.current]);

   useEffect(() => {
      if (controllerRef.current) {
         controllerRef.current.updateCanvas(canvas);
      }
   }, [controllerRef.current, canvas]);

   useEffect(() => {
      const controller = controllerRef.current;
      if (controller) {
         controller.on('pushAction', onPushAction);

         return () => {
            controller.off('pushAction', onPushAction);
         };
      }
   }, [onPushAction, controllerRef.current]);

   const handleChangeSelectedTool = (type: ToolType) => {
      setSelectedTool(type);

      if (controllerRef.current) {
         controllerRef.current.setTool(getTool(type));
      }
   };

   const handleOptionsChanged = (value: WhiteboardToolOptions) => {
      setOptions(value);
      controllerRef.current?.updateOptions(value);
   };

   const handleClear = () => {
      if (controllerRef.current) {
         controllerRef.current.clearWhiteboard();
      }
   };

   return (
      <div
         style={{ display: 'flex', height: '100%', width: '100%', flexDirection: 'column', justifyContent: 'center' }}
      >
         <div style={{ display: 'flex' }}>
            <div
               style={{
                  flex: 1,
                  display: 'flex',
                  flexDirection: 'column',
                  alignItems: 'flex-end',
                  marginLeft: 16,
                  marginRight: 8,
               }}
            >
               <ToolsContainer
                  selectedTool={selectedTool}
                  onSelectedToolChanged={handleChangeSelectedTool}
                  options={options}
                  onOptionsChanged={handleOptionsChanged}
                  onClear={handleClear}
                  canUndo={canUndo}
                  onUndo={onUndo}
                  canRedo={canRedo}
                  onRedo={onRedo}
               />
            </div>
            <div style={{ width: 1280, height: 720 }}>
               <canvas id="test-canvas" ref={canvasRef}>
                  Sorry, Canvas HTML5 element is not supported by your browser :(
               </canvas>
            </div>
            <div style={{ flex: 1 }} />
         </div>
      </div>
   );
}
