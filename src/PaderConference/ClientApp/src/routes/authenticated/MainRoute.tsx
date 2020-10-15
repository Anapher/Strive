import { Button, makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { useDispatch } from 'react-redux';
import { signOut } from 'src/features/auth/authSlice';
import ConferenceControls from 'src/features/create-conference/components/ConferenceControls';

const useStyles = makeStyles((theme) => ({
   root: {
      position: 'relative',
      height: '100%',
      width: '100%',
   },
   container: {
      width: '100%',
      height: '100%',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
   },
   contentContainer: {
      display: 'flex',
      alignItems: 'center',
      flexDirection: 'column',
   },
   title: {
      marginBottom: theme.spacing(4),
      height: 72,
      marginTop: -72,
   },
   buttonContainer: {
      display: 'flex',
      flexDirection: 'column',
      width: 400,
   },
   signOutButton: {
      right: theme.spacing(2),
      top: theme.spacing(2),
      position: 'absolute',
   },
}));

export default function MainRoute() {
   const classes = useStyles();

   const dispatch = useDispatch();
   const handleSignOut = () => dispatch(signOut());

   return (
      <div className={classes.root}>
         <Button className={classes.signOutButton} onClick={handleSignOut}>
            Sign out
         </Button>
         <div className={classes.container}>
            <div className={classes.contentContainer}>
               <Typography variant="h2" className={classes.title} align="center">
                  Welcome to PaderConference
               </Typography>
               <div className={classes.buttonContainer}>
                  <ConferenceControls />
               </div>
            </div>
         </div>
      </div>
   );
}
