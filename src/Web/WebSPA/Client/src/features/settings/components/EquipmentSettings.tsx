import { Box, Button, Chip, Grid, makeStyles, TextField, Typography } from '@material-ui/core';
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
import { useTranslation } from 'react-i18next';

const QR_CODE_PADDING = 8;

const useStyles = makeStyles((theme) => ({
   qrCode: {
      backgroundColor: 'white',
      padding: QR_CODE_PADDING,
      height: 200 + 2 * QR_CODE_PADDING,
      width: 200 + 2 * QR_CODE_PADDING,

      [theme.breakpoints.down('sm')]: {
         marginBottom: theme.spacing(4),
      },
   },
   qrCodeContainer: {
      display: 'flex',
      marginTop: theme.spacing(4),

      [theme.breakpoints.down('sm')]: {
         flexDirection: 'column',
         alignItems: 'center',
      },
   },
}));

export default function EquipmentSettings() {
   const dispatch = useDispatch();
   const { t } = useTranslation();
   const classes = useStyles();

   const token = useSelector((state: RootState) => state.settings.equipmentToken);
   const error = useSelector((state: RootState) => state.settings.equipmentTokenError);
   const equipment = useSelector(selectEquipmentConnections);
   const participantId = useSelector(selectMyParticipantId);

   const {
      params: { id },
   } = useRouteMatch<ConferenceRouteParams>();

   const handleFetchEquipment = () => dispatch(getEquipmentToken());

   useEffect(() => {
      if (!token) {
         handleFetchEquipment();
      }
   }, [token, dispatch]);

   const url = new URL(`/c/${id}/as-equipment?participantId=${participantId}&token=${token}`, document.baseURI).href;

   return (
      <Box p={2} pt={0}>
         <Typography variant="subtitle1">{t('conference.settings.equipment.description')}</Typography>
         {error ? (
            <Box mt={2}>
               <Typography color="error" gutterBottom>
                  {t('conference.settings.equipment.error_fetch_token', { error })}
               </Typography>
               <Button variant="contained" onClick={handleFetchEquipment}>
                  {t('common:retry')}
               </Button>
            </Box>
         ) : (
            <div className={classes.qrCodeContainer}>
               {token ? (
                  <div className={classes.qrCode}>
                     <QRCode value={url} size={200} renderAs="svg" />
                  </div>
               ) : (
                  <Skeleton variant="rect" width={200} height={200} />
               )}
               <Box flex={1} ml={3}>
                  <Typography>{token ? t('conference.settings.equipment.step_1') : <Skeleton />}</Typography>
                  <Box mt={1} mb={1}>
                     {token ? (
                        <TextField fullWidth variant="outlined" InputProps={{ readOnly: true }} value={url} />
                     ) : (
                        <Skeleton height={50} />
                     )}
                  </Box>
                  <Typography gutterBottom>
                     {token ? t('conference.settings.equipment.step_2') : <Skeleton />}
                  </Typography>
                  <Typography>{token ? t('conference.settings.equipment.step_3') : <Skeleton />}</Typography>
               </Box>
            </div>
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
