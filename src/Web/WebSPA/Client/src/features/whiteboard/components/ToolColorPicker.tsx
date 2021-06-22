import { Typography } from '@material-ui/core';
import { Box, ClickAwayListener, Grid, Grow, IconButton, Paper, Popper, Tooltip } from '@material-ui/core';
import React, { useState, useRef } from 'react';
import ColorIcon from './ColorIcon';
import ToolIcon from './ToolIcon';

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
         <Tooltip title="Switch color using double click">
            <ToolIcon
               icon={<ColorIcon color={value} />}
               ref={anchorEl}
               onClick={handleOpen}
               onDoubleClick={handleSwitchColor}
            />
         </Tooltip>

         <Popper open={open} anchorEl={anchorEl.current} transition placement="right-start">
            {({ TransitionProps }) => (
               <Grow {...TransitionProps} style={{ transformOrigin: 'left top' }}>
                  <Paper>
                     <ClickAwayListener onClickAway={handleClose}>
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
                           <Typography variant="body2" style={{ fontSize: 10 }} color="textSecondary" align="center">
                              Swap the color with a double click on the icon in the toolbox
                           </Typography>
                        </Box>
                     </ClickAwayListener>
                  </Paper>
               </Grow>
            )}
         </Popper>
      </>
   );
}
