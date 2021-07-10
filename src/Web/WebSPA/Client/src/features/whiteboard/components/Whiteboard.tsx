import { makeStyles } from '@material-ui/core';
import React, { useEffect, useRef, useState } from 'react';
import '../fabric-style';
import NoTool from '../tools/NoTool';
import { CanvasPushAction, WhiteboardCanvas } from '../types';
import WhiteboardController, { LiveUpdateHandler } from '../whiteboard-controller';
import { WhiteboardToolOptions } from '../whiteboard-tool';
import ToolsContainer, { getTool, ToolType } from './ToolsContainer';

const useStyles = makeStyles({
   root: {
      display: 'flex',
      height: '100%',
      width: '100%',
      flexDirection: 'column',
      justifyContent: 'center',
   },
   container: {
      display: 'flex',
   },
   toolsContainer: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'flex-end',
      marginLeft: 16,
      marginRight: 8,
   },
   flexFill: {
      flex: 1,
   },
});

type Props = {
   canvas: WhiteboardCanvas;
   onPushAction: (action: CanvasPushAction) => Promise<number>;
   canUndo: boolean;
   onUndo: () => void;
   canRedo: boolean;
   onRedo: () => void;
   participants: string[];
   liveUpdateHandler?: LiveUpdateHandler;

   readOnly?: boolean;
};

export default function Whiteboard({
   canvas,
   onPushAction,
   canUndo,
   onUndo,
   canRedo,
   onRedo,
   liveUpdateHandler,
   participants,
   readOnly,
}: Props) {
   const classes = useStyles();

   const [selectedTool, setSelectedTool] = useState<ToolType | undefined>('select');
   const [options, setOptions] = useState<WhiteboardToolOptions>({ color: 'black', fontSize: 36, lineWidth: 5 });

   const canvasRef = useRef<HTMLCanvasElement>(null);
   const controllerRef = useRef<WhiteboardController | undefined>();

   useEffect(() => {
      if (canvasRef.current && !controllerRef.current) {
         controllerRef.current = new WhiteboardController(
            canvasRef.current,
            selectedTool ? getTool(selectedTool) : new NoTool(),
            options,
         );
      }
   }, [canvasRef.current]);

   useEffect(() => {
      if (controllerRef.current) {
         controllerRef.current.updateCanvas(canvas);
      }
   }, [controllerRef.current, canvas]);

   useEffect(() => {
      if (readOnly) {
         setSelectedTool(undefined);
         if (controllerRef.current) {
            controllerRef.current.setTool(new NoTool());
         }
      }
   }, [readOnly, controllerRef.current]);

   useEffect(() => {
      const controller = controllerRef.current;
      if (controller) {
         controller.pushAction = onPushAction;

         return () => {
            controller.pushAction = undefined;
         };
      }
   }, [onPushAction, controllerRef.current]);

   useEffect(() => {
      const controller = controllerRef.current;
      if (controller && liveUpdateHandler) {
         controller.setupLiveUpdateHandler(liveUpdateHandler);
      }
   }, [liveUpdateHandler, controllerRef.current]);

   useEffect(() => {
      controllerRef.current?.updateParticipants(participants);
   }, [participants, controllerRef.current]);

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
      if (readOnly) return;

      if (controllerRef.current) {
         controllerRef.current.clearWhiteboard();
      }
   };

   const handleKeyPress = (key: string, ctrl: boolean, shift: boolean) => {
      if (readOnly) return;

      if (key === 'z' && ctrl && !shift) {
         if (canUndo) onUndo();
      }

      if (key === 'Z' && ctrl && shift) {
         if (canRedo) onRedo();
      }

      if (!ctrl && !shift) {
         switch (key) {
            case 'v':
               handleChangeSelectedTool('select');
               break;
            case 't':
               handleChangeSelectedTool('text');
               break;
            case 'h':
               handleChangeSelectedTool('pan');
               break;
            case 'u':
               handleChangeSelectedTool('line');
               break;
            case 'b':
               handleChangeSelectedTool('pencil');
               break;
            case 'Delete':
               controllerRef.current?.deleteSelection();
               break;
            default:
               break;
         }
      }
   };

   return (
      <div className={classes.root} onKeyDown={(e) => handleKeyPress(e.key, e.ctrlKey, e.shiftKey)} tabIndex={-1}>
         <div className={classes.container}>
            <div className={classes.toolsContainer}>
               {!readOnly && (
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
               )}
            </div>
            <div style={{ width: 1280, height: 720 }}>
               <canvas id="test-canvas" ref={canvasRef}>
                  Sorry, Canvas HTML5 element is not supported by your browser :(
               </canvas>
            </div>
            <div className={classes.flexFill} />
         </div>
      </div>
   );
}
