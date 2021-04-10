import { makeStyles } from '@material-ui/core';
import React from 'react';

const useStyles = makeStyles({
   root: {
      width: 10,
      height: 10,
      backgroundColor: '#f1c40f',
      borderRadius: 5,
      marginLeft: 12,
   },
});

export default function NewMessagesIndicator() {
   const classes = useStyles();
   return <div className={classes.root} />;
}
