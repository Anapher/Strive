import { Fab, makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import NavigationIcon from '@material-ui/icons/Navigation';
import AddIcon from '@material-ui/icons/Add';

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
   extendedIcon: {
      marginRight: theme.spacing(1),
   },
   fabMargin: {
      marginTop: theme.spacing(2),
   },
   invisible: {
      opacity: 0,
      display: 'none',
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
               <Fab color="secondary" variant="extended">
                  <AddIcon className={classes.extendedIcon} />
                  Start new conference
               </Fab>
               <Fab color="primary" variant="extended" className={classes.fabMargin}>
                  <NavigationIcon className={classes.extendedIcon} />
                  Join conference
               </Fab>
            </div>
         </div>
      </div>
   );
}
