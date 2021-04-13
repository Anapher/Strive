import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles(() => ({
   root: {
      height: '100%',
      width: '100%',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexDirection: 'column',
      cursor: 'pointer',
   },
   textAlignCenter: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
   infoContainer: {
      display: 'flex',
      alignItems: 'center',
      flex: 1,
   },
   fill: {
      flex: 1,
   },
}));

export default function RequestUserInteractionView() {
   const classes = useStyles();
   const { t } = useTranslation();

   return (
      <div className={classes.root}>
         <div className={classes.fill} />
         <div className={classes.textAlignCenter}>
            <Typography variant="h3" align="center">
               {t('request_user_interaction.title')}
            </Typography>
            <Typography variant="h6" align="center">
               {t('request_user_interaction.request')}
            </Typography>
         </div>

         <div className={classes.infoContainer}>
            <Typography variant="caption" color="textSecondary" align="center">
               {t('request_user_interaction.description')}
            </Typography>
         </div>
      </div>
   );
}
