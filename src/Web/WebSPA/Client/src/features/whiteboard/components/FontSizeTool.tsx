import { Box, Grid, IconButton, makeStyles, Typography } from '@material-ui/core';
import { FormatSize } from '@material-ui/icons';
import clsx from 'classnames';
import React, { useRef, useState } from 'react';
import ToolIcon from './ToolIcon';
import ToolPopper from './ToolPopper';

const availableFontSizes = [12, 18, 24, 30, 36];

const useStyles = makeStyles((theme) => ({
   strokeButton: {
      height: '100%',
      width: 64,
   },
   strokeButtonSelected: {
      backgroundColor: theme.palette.grey[800],
   },
}));

type Props = {
   value: number;
   onChange: (value: number) => void;
};

export default function FontSizeTool({ value, onChange }: Props) {
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
         <ToolIcon icon={<FormatSize fontSize="small" />} ref={anchorEl} onClick={handleOpen} />

         <ToolPopper open={open} anchorEl={anchorEl.current} onClose={handleClose}>
            <Box p={1}>
               <Grid container>
                  {availableFontSizes.map((size) => (
                     <Grid item key={size}>
                        <IconButton
                           onClick={handleChange(size)}
                           className={clsx(classes.strokeButton, value === size && classes.strokeButtonSelected)}
                           title={`${size}px`}
                        >
                           <Typography style={{ fontSize: size }}>A</Typography>
                        </IconButton>
                     </Grid>
                  ))}
               </Grid>
            </Box>
         </ToolPopper>
      </>
   );
}
