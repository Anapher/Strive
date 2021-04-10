import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';

const useStyles = makeStyles({
   root: {
      flex: 1,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
   },
});

export default function NewChat() {
   const classes = useStyles();
   return (
      <div className={classes.root}>
         <Typography color="textSecondary">Say hi!</Typography>
      </div>
   );
}
