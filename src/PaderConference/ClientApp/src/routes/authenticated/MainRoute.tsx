import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import ConferenceControls from 'src/features/conference/components/ConferenceControls';

const useStyles = makeStyles((theme) => ({
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
}));

export default function MainRoute() {
   const classes = useStyles();

   return (
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
   );
}
