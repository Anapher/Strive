import { Box, Grid, IconButton } from '@material-ui/core';
import React, { useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import ColorIcon from './ColorIcon';
import ToolIcon from './ToolIcon';
import ToolPopper from './ToolPopper';

const availableColors = [
   '#000000',
   '#ffffff',
   '#e74c3c',
   '#2980b9',
   '#27ae60',
   '#8e44ad',
   '#f39c12',
   '#16a085',
   '#7f8c8d',
];

type Props = {
   value: string;
   onChange: (value: string) => void;
};

export default function ToolColorPicker({ value, onChange }: Props) {
   const { t } = useTranslation();

   const [open, setOpen] = useState(false);
   const anchorEl = useRef(null);
   const [previousColor, setPreviousColor] = useState(value);

   const handleClose = () => setOpen(false);
   const handleOpen = () => setOpen(true);

   const handleSelectColor = (color: string) => () => {
      onChange(color);
      handleClose();

      setPreviousColor(value);
   };

   const handleSwitchColor = () => {
      onChange(previousColor);
      setPreviousColor(value);
   };

   return (
      <>
         <ToolIcon
            title={t('conference.whiteboard.toolbar.color')}
            icon={<ColorIcon color={value} />}
            ref={anchorEl}
            onClick={handleOpen}
            onDoubleClick={handleSwitchColor}
         />

         <ToolPopper open={open} anchorEl={anchorEl.current} onClose={handleClose}>
            <Box p={1} width={160}>
               <Grid container justify="center">
                  {availableColors.map((color) => (
                     <Grid item key={color}>
                        <IconButton onClick={handleSelectColor(color)}>
                           <ColorIcon color={color} />
                        </IconButton>
                     </Grid>
                  ))}
               </Grid>
            </Box>
         </ToolPopper>
      </>
   );
}
