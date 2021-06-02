import { makeStyles } from '@material-ui/core';
import React from 'react';

const useStyles = makeStyles((theme) => ({
   marginLeft: {
      marginLeft: theme.spacing(1),
   },
}));

export default function PollOptionsPopper() {
   const classes = useStyles();
   return <div />;
}
