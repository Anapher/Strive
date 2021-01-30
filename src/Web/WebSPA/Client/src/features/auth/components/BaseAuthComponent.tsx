import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';

const useStyles = makeStyles({
   root: {
      height: '100%',
      width: '100%',
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
   },
   content: {
      display: 'flex',
      alignItems: 'center',
      flexDirection: 'column',
   },
});

type Props = {
   title: string;
   text: string;
   children?: React.ReactNode;
};

export default function BaseAuthComponent({ title, text, children }: Props) {
   const classes = useStyles();

   return (
      <div className={classes.root}>
         <div className={classes.content}>
            <Typography variant="h3" gutterBottom>
               {title}
            </Typography>
            <Typography>{text}</Typography>
            {children}
         </div>
      </div>
   );
}
