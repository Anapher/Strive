import { makeStyles, Theme } from '@material-ui/core';
import React from 'react';
import SignInDialog from 'src/features/auth/components/SignInDialog';

const useStyles = makeStyles((theme: Theme) => ({
   root: {
      paddingTop: theme.spacing(8),
      marginLeft: theme.spacing(3),
      marginRight: theme.spacing(3),
      [theme.breakpoints.up(400 + theme.spacing(6))]: {
         width: 400,
         marginLeft: 'auto',
         marginRight: 'auto',
      },
   },
}));

export default function AuthRoute() {
   const classes = useStyles();

   return (
      <div className={classes.root}>
         <SignInDialog />
      </div>
   );
}
