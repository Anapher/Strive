import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';

const useStyles = makeStyles(() => ({
   root: {
      height: '100%',
      width: '100%',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
   },
   textAlignCenter: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
}));

export default function RequestUserInteractionView() {
   const classes = useStyles();

   return (
      <div className={classes.root}>
         <div className={classes.textAlignCenter}>
            <Typography variant="h3">Please do something</Typography>
            <Typography variant="h6">Click anywhere | Press any key | Tap anywhere</Typography>
         </div>
      </div>
   );
}
