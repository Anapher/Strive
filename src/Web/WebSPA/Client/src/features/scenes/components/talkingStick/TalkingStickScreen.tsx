import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import clsx from 'classnames';
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
   headerContainer: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      justifyContent: 'flex-end',
      paddingBottom: theme.spacing(2),
   },
   footer: {
      flex: 1,
   },
}));

type Props = {
   children: React.ReactNode;
   className?: string;
};

export default function TalkingStickScreen({ children, className }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   return (
      <div className={clsx(className, classes.root)}>
         <div className={classes.headerContainer}>
            <Typography variant="h2">{t('conference.scenes.talking_stick')}</Typography>
         </div>
         {children}
         <div className={classes.footer} />
      </div>
   );
}
