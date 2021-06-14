import { Divider, makeStyles, SvgIcon } from '@material-ui/core';
import _ from 'lodash';
import {
   CursorDefaultOutline,
   Delete,
   FormatText,
   PencilOutline,
   Minus,
   Undo,
   Redo,
   FormatLineStyle,
   FormatSize,
} from 'mdi-material-ui';
import React from 'react';
import { LineTool } from '../tools/LineTool';
import { PencilTool } from '../tools/PencilTool';
import { SelectTool } from '../tools/SelectTool';
import { TextTool } from '../tools/TextTool';
import WhiteboardTool, { WhiteboardToolOptions } from '../whiteboard-tool';
import ToolIcon from './ToolIcon';

const useStyles = makeStyles((theme) => ({
   root: {
      backgroundColor: theme.palette.background.paper,
      borderRadius: 24,
      marginTop: 64,
      marginLeft: 16,
      marginRight: 8,
      padding: theme.spacing(1, 0),
      height: 'fit-content',
   },
   hidden: {
      visibility: 'hidden',
   },
}));

type OptionType = keyof WhiteboardToolOptions;

type ToolViewModel = {
   labelTranslationKey: string;
   Icon: typeof SvgIcon;
   options: readonly OptionType[];
   toolFactory: () => WhiteboardTool;
};

type Tools = {
   select: ToolViewModel;
   pencil: ToolViewModel;
   text: ToolViewModel;
   line: ToolViewModel;
};

const tools: Tools = {
   select: {
      labelTranslationKey: 'Select',
      Icon: CursorDefaultOutline,
      options: [],
      toolFactory: () => new SelectTool(),
   },
   pencil: {
      labelTranslationKey: 'Pencil',
      Icon: PencilOutline,
      options: ['lineWidth', 'color'],
      toolFactory: () => new PencilTool(),
   },
   text: {
      labelTranslationKey: 'Text',
      Icon: FormatText,
      options: ['fontSize', 'color'],
      toolFactory: () => new TextTool(),
   },
   line: {
      labelTranslationKey: 'Line',
      Icon: Minus,
      options: ['color', 'lineWidth'],
      toolFactory: () => new LineTool(),
   },
} as const;

const maxSupportedOptions = _.max(Object.values(tools).map((x) => x.options.length)) ?? 0;

export type ToolType = keyof Tools;

export const getTool = (type: ToolType) => tools[type].toolFactory();

type Props = {
   selectedTool: ToolType;
   onSelectedToolChanged: (tool: ToolType) => void;
};

export default function ToolsContainer({ selectedTool, onSelectedToolChanged }: Props) {
   const classes = useStyles();
   const options = tools[selectedTool].options;

   return (
      <div className={classes.root}>
         {Object.entries(tools).map(([type, { Icon }]) => (
            <div key={type}>
               <ToolIcon
                  icon={<Icon fontSize="small" />}
                  selected={selectedTool === type}
                  onClick={() => onSelectedToolChanged(type as ToolType)}
               />
            </div>
         ))}
         <Divider className={options.length === 0 ? classes.hidden : undefined} />
         <div style={{ height: maxSupportedOptions * 36 }}>
            {options.includes('lineWidth') && <ToolIcon icon={<FormatLineStyle fontSize="small" />} />}
            {options.includes('color') && <ToolIcon icon={<FormatLineStyle fontSize="small" />} />}
            {options.includes('fontSize') && <ToolIcon icon={<FormatSize fontSize="small" />} />}
         </div>
         <Divider />
         <ToolIcon icon={<Undo fontSize="small" />} />
         <ToolIcon icon={<Redo fontSize="small" />} />

         <Divider style={{ marginTop: 64 }} />
         <ToolIcon icon={<Delete fontSize="small" />} />
      </div>
   );
}
