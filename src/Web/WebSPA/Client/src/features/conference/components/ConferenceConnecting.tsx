import { Box, CircularProgress, makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';

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
   const { t } = useTranslation();

   return (
      <div className={classes.root}>
         <Box display="flex" flexDirection="row" alignItems="center">
            <CircularProgress />
            <Typography className={classes.text}>
               {isReconnecting
                  ? t('conference_not_open.reconnecting_to_conference')
                  : t('conference_not_open.connecting_to_conference')}
            </Typography>
         </Box>
      </div>
   );
}
