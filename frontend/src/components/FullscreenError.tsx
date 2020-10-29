import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';

const useStyles = makeStyles({
   root: {
      height: '100%',
      width: '100%',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
   },
   errorContainer: {
      maxWidth: 992,
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
});

type Props = {
   message: string;
   children?: React.ReactNode;
};

export default function FullscreenError({ message, children }: Props) {
   const classes = useStyles();

   return (
      <div className={classes.root}>
         <div className={classes.errorContainer}>
            <Typography color="error" variant="h4" align="center" gutterBottom>
               {message}
            </Typography>
            {children}
         </div>
      </div>
   );
}
