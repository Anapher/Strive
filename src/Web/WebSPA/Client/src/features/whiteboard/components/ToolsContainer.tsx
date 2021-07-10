import { Divider, makeStyles, SvgIcon } from '@material-ui/core';
import _ from 'lodash';
import { CursorDefaultOutline, FormatText, HandRight, Minus, PencilOutline, Redo, Undo } from 'mdi-material-ui';
import React from 'react';
import { useTranslation } from 'react-i18next';
import LineTool from '../tools/LineTool';
import PanTool from '../tools/PanTool';
import PencilTool from '../tools/PencilTool';
import SelectTool from '../tools/SelectTool';
import TextTool from '../tools/TextTool';
import WhiteboardTool, { WhiteboardToolOptions } from '../whiteboard-tool';
import ClearWhiteboardButton from './ClearWhiteboardButton';
import FontSizeTool from './FontSizeTool';
import LineWidthTool from './LineWidthTool';
import ToolColorPicker from './ToolColorPicker';
import ToolIcon from './ToolIcon';

const useStyles = makeStyles((theme) => ({
   root: {
      backgroundColor: theme.palette.background.paper,
      borderRadius: 24,
      padding: theme.spacing(1, 0),
      height: 'fit-content',
      width: 'fit-content',
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
   pan: ToolViewModel;
};

const tools: Tools = {
   select: {
      labelTranslationKey: 'conference.whiteboard.toolbar.select',
      Icon: CursorDefaultOutline,
      options: [],
      toolFactory: () => new SelectTool(),
   },
   pencil: {
      labelTranslationKey: 'conference.whiteboard.toolbar.pencil',
      Icon: PencilOutline,
      options: ['lineWidth', 'color'],
      toolFactory: () => new PencilTool(),
   },
   text: {
      labelTranslationKey: 'conference.whiteboard.toolbar.text',
      Icon: FormatText,
      options: ['fontSize', 'color'],
      toolFactory: () => new TextTool(),
   },
   line: {
      labelTranslationKey: 'conference.whiteboard.toolbar.line',
      Icon: Minus,
      options: ['color', 'lineWidth'],
      toolFactory: () => new LineTool(),
   },
   pan: {
      labelTranslationKey: 'conference.whiteboard.toolbar.pan',
      Icon: HandRight,
      options: [],
      toolFactory: () => new PanTool(),
   },
} as const;

const maxSupportedOptions = _.max(Object.values(tools).map((x) => x.options.length)) ?? 0;

export type ToolType = keyof Tools;

export const getTool = (type: ToolType) => tools[type].toolFactory();

type Props = {
   selectedTool: ToolType | undefined;
   onSelectedToolChanged: (tool: ToolType) => void;

   options: WhiteboardToolOptions;
   onOptionsChanged: (options: WhiteboardToolOptions) => void;

   onClear: () => void;

   canUndo: boolean;
   onUndo: () => void;
   canRedo: boolean;
   onRedo: () => void;
};

export default function ToolsContainer({
   selectedTool,
   onSelectedToolChanged,
   options,
   onOptionsChanged,
   onClear,

   canUndo,
   onUndo,
   canRedo,
   onRedo,
}: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   const requiredOptions = selectedTool ? tools[selectedTool].options : [];

   const handleChangeColor = (color: string) => {
      onOptionsChanged({ ...options, color });
   };

   const handleChangeLineWidth = (lineWidth: number) => {
      onOptionsChanged({ ...options, lineWidth });
   };

   const handleChangeFontSize = (fontSize: number) => {
      onOptionsChanged({ ...options, fontSize });
   };

   return (
      <div className={classes.root}>
         {Object.entries(tools).map(([type, { Icon, labelTranslationKey }]) => (
            <div key={type}>
               <ToolIcon
                  icon={<Icon fontSize="small" />}
                  selected={selectedTool === type}
                  onClick={() => onSelectedToolChanged(type as ToolType)}
                  title={t(labelTranslationKey)}
               />
            </div>
         ))}
         <Divider className={requiredOptions.length === 0 ? classes.hidden : undefined} />
         <div style={{ height: maxSupportedOptions * 36 }}>
            {requiredOptions.includes('color') && (
               <ToolColorPicker value={options.color} onChange={handleChangeColor} />
            )}
            {requiredOptions.includes('lineWidth') && (
               <LineWidthTool value={options.lineWidth} onChange={handleChangeLineWidth} />
            )}

            {requiredOptions.includes('fontSize') && (
               <FontSizeTool value={options.fontSize} onChange={handleChangeFontSize} />
            )}
         </div>
         <Divider />
         <ToolIcon
            disabled={!canUndo}
            onClick={onUndo}
            icon={<Undo fontSize="small" />}
            title={t('conference.whiteboard.toolbar.undo')}
         />
         <ToolIcon
            disabled={!canRedo}
            onClick={onRedo}
            icon={<Redo fontSize="small" />}
            title={t('conference.whiteboard.toolbar.redo')}
         />

         <Divider style={{ marginTop: 64 }} />
         <ClearWhiteboardButton onClick={onClear} />
      </div>
   );
}
