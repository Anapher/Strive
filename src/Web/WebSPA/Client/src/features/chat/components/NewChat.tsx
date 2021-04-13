import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles({
   root: {
      flex: 1,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
   },
});

export default function NewChat() {
   const classes = useStyles();
   const { t } = useTranslation();

   return (
      <div className={classes.root}>
         <Typography color="textSecondary">{t('conference.chat.new_chat_info')}</Typography>
      </div>
   );
}
