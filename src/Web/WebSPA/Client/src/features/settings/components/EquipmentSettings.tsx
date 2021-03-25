import { Box, Button, Chip, Grid, TextField, Typography } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import QRCode from 'qrcode.react';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useRouteMatch } from 'react-router-dom';
import { getEquipmentToken } from 'src/core-hub';
import { ConferenceRouteParams } from 'src/routes/types';
import { RootState } from 'src/store';
import CheckIcon from '@material-ui/icons/Check';
import { selectEquipmentConnections } from '../selectors';
import { selectMyParticipantId } from 'src/features/auth/selectors';

export default function EquipmentSettings() {
   const token = useSelector((state: RootState) => state.settings.equipmentToken);
   const error = useSelector((state: RootState) => state.settings.equipmentTokenError);
   const equipment = useSelector(selectEquipmentConnections);
   const participantId = useSelector(selectMyParticipantId);

   const {
      params: { id },
   } = useRouteMatch<ConferenceRouteParams>();
   const dispatch = useDispatch();

   const handleFetchEquipment = () => dispatch(getEquipmentToken());

   useEffect(() => {
      if (!token) {
         handleFetchEquipment();
      }
   }, [token, dispatch]);

   const url = new URL(`/c/${id}/as-equipment?participantId=${participantId}&token=${token}`, document.baseURI).href;

   return (
      <Box p={2} pt={0}>
         <Typography variant="subtitle1">{`Don't have a webcam but have a smartphone with a camera? Don't have a good microphone but a tablet that has one? No problem, you can use multiple external devices as your microphone or webcam or share their screen.`}</Typography>
         {error ? (
            <Box mt={2}>
               <Typography color="error" gutterBottom>
                  An error occurred when trying to fetch the equipment token: {error.message}
               </Typography>
               <Button variant="contained" onClick={handleFetchEquipment}>
                  Retry
               </Button>
            </Box>
         ) : (
            <Box display="flex" mt={4}>
               {token ? (
                  <QRCode value={url} size={200} renderAs="svg" />
               ) : (
                  <Skeleton variant="rect" width={200} height={200} />
               )}
               <Box flex={1} ml={3}>
                  <Typography>
                     {token ? '1. Scan the QR code or manually open this url on your device:' : <Skeleton />}
                  </Typography>
                  <Box mt={1} mb={1}>
                     {token ? (
                        <TextField fullWidth variant="outlined" InputProps={{ readOnly: true }} value={url} />
                     ) : (
                        <Skeleton height={50} />
                     )}
                  </Box>
                  <Typography gutterBottom>{token ? '2. Allow access on your device' : <Skeleton />}</Typography>
                  <Typography>{token ? '3. Change the default device here in settings' : <Skeleton />}</Typography>
               </Box>
            </Box>
         )}
         {equipment && (
            <Box mt={2}>
               <Grid container>
                  {Object.values(equipment).map((x) => (
                     <Grid item key={x.connectionId}>
                        <Chip color="secondary" label={x.name} icon={<CheckIcon />} />
                     </Grid>
                  ))}
               </Grid>
            </Box>
         )}
      </Box>
   );
}
