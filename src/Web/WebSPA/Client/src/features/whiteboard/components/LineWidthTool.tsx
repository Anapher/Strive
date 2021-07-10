import { Box, Grid, IconButton, makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import { FormatLineStyle } from 'mdi-material-ui';
import React, { useRef, useState } from 'react';
import ToolIcon from './ToolIcon';
import ToolPopper from './ToolPopper';

const availableStrokes = [3, 5, 10, 15, 20];

const useStyles = makeStyles((theme) => ({
   strokeButton: {
      height: '100%',
      width: 44,
   },
   strokeButtonSelected: {
      backgroundColor: theme.palette.grey[800],
   },
}));

type Props = {
   value: number;
   onChange: (value: number) => void;
};

export default function LineWidthTool({ value, onChange }: Props) {
   const classes = useStyles();

   const [open, setOpen] = useState(false);
   const anchorEl = useRef(null);

   const handleClose = () => setOpen(false);
   const handleOpen = () => setOpen(true);

   const handleChange = (width: number) => () => {
      onChange(width);
      handleClose();
   };

   return (
      <>
         <ToolIcon icon={<FormatLineStyle fontSize="small" />} ref={anchorEl} onClick={handleOpen} />

         <ToolPopper open={open} anchorEl={anchorEl.current} onClose={handleClose}>
            <Box p={1}>
               <Grid container>
                  {availableStrokes.map((width) => (
                     <Grid item key={width}>
                        <IconButton
                           onClick={handleChange(width)}
                           className={clsx(classes.strokeButton, value === width && classes.strokeButtonSelected)}
                           title={`${width}px`}
                        >
                           <div style={{ width, height: width, borderRadius: width / 2, backgroundColor: 'white' }} />
                        </IconButton>
                     </Grid>
                  ))}
               </Grid>
            </Box>
         </ToolPopper>
      </>
   );
}
