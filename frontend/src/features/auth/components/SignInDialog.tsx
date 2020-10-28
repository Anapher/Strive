import { Avatar, Box, Paper, Theme, Typography } from '@material-ui/core';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import { makeStyles } from '@material-ui/styles';
import React from 'react';
import SignInForm from './SignInForm';

const useStyles = makeStyles((theme: Theme) => ({
   avatar: {
      margin: theme.spacing(1),
      backgroundColor: theme.palette.secondary.main,
   },
   paper: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      padding: theme.spacing(2, 3, 3),
   },
}));

export default function SignInDialog() {
   const classes = useStyles();

   return (
      <Paper className={classes.paper}>
         <Avatar className={classes.avatar}>
            <LockOutlinedIcon />
         </Avatar>
         <Typography component="h1" variant="h5">
            Sign In
         </Typography>

         <Box mt={2}>
            <SignInForm />
         </Box>
      </Paper>
   );
}
