import { Box, CircularProgress, makeStyles, Typography } from '@material-ui/core';
import React from 'react';

const useStyles = makeStyles((theme) => ({
   root: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
   },
   text: {
      marginLeft: theme.spacing(3),
   },
}));

type Props = {
   isReconnecting: boolean;
};

export default function ConferenceConnecting({ isReconnecting }: Props) {
   const classes = useStyles();

   return (
      <div className={classes.root}>
         <Box display="flex" flexDirection="row" alignItems="center">
            <CircularProgress />
            <Typography className={classes.text}>
               {isReconnecting ? 'Reconnecting to conference...' : 'Connecting to conference...'}
            </Typography>
         </Box>
      </div>
   );
}
