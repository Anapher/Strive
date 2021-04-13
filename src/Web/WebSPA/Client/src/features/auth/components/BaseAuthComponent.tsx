import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles({
   root: {
      height: '100%',
      width: '100%',
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
   },
   content: {
      display: 'flex',
      alignItems: 'center',
      flexDirection: 'column',
   },
});

type Props = {
   componentName: string;
   children?: React.ReactNode;
};

export default function BaseAuthComponent({ componentName, children }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   return (
      <div className={classes.root}>
         <div className={classes.content}>
            <Typography variant="h3" gutterBottom>
               {t(`auth.${componentName}.title`)}
            </Typography>
            <Typography>{t(`auth.${componentName}.text`)}</Typography>
            {children}
         </div>
      </div>
   );
}
