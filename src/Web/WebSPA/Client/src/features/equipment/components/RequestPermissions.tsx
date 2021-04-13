import { Button, makeStyles, Typography } from '@material-ui/core';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles({
   root: {
      width: '100%',
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
   },
   container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
});

type Props = {
   onPermissionsGranted: () => void;
};

export default function RequestPermissions({ onPermissionsGranted }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();
   const [error, setError] = useState(false);

   const onRequestPermissions = async () => {
      try {
         await navigator.mediaDevices.getUserMedia({ audio: true });
         await navigator.mediaDevices.getUserMedia({ video: true });
         onPermissionsGranted();
      } catch (error) {
         setError(true);
      }
   };

   useEffect(() => {
      navigator.mediaDevices.enumerateDevices().then((devices) => {
         if (!devices.find((x) => !x.label)) {
            onPermissionsGranted();
         }
      });
   }, []);

   return (
      <div className={classes.root}>
         <div className={classes.container}>
            <Typography variant="h4" gutterBottom>
               {t('conference.equipment.request_permissions.title')}
            </Typography>
            <Button variant="contained" color="primary" onClick={onRequestPermissions}>
               {t('conference.equipment.request_permissions.grant_permissions')}
            </Button>
            {error && <Typography color="error">{t('conference.equipment.request_permissions.error')}</Typography>}
         </div>
      </div>
   );
}
