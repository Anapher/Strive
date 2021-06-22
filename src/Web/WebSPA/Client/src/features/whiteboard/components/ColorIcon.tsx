import { makeStyles } from '@material-ui/core';
import React from 'react';

const useStyles = makeStyles({
   root: {
      width: 18,
      height: 18,
      borderRadius: 9,
   },
});

type Props = {
   color: string;
};

export default function ColorIcon({ color }: Props) {
   const classes = useStyles();
   return <div className={classes.root} style={{ backgroundColor: color }} />;
}
