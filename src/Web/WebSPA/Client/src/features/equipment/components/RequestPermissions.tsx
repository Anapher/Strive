import { Button, makeStyles, Typography } from '@material-ui/core';
import React, { useEffect, useState } from 'react';

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
               Use this device as equipment
            </Typography>
            <Button variant="contained" color="primary" onClick={onRequestPermissions}>
               Grant Permissions
            </Button>
            {error && <Typography color="error">Please accept the permission request for media.</Typography>}
         </div>
      </div>
   );
}
