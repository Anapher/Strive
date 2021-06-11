import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import useSelectorFactory from 'src/hooks/useSelectorFactory';
import { RootState } from 'src/store';
import { setCurrentDevice } from '../reducer';
import { selectAvailableInputDevicesFactory, selectIsDeviceAvailableFactory } from '../selectors';
import DeviceSelector from './DeviceSelector';

const useStyles = makeStyles((theme) => ({
   root: {
      width: '100%',
      padding: theme.spacing(3),
      paddingTop: 0,
   },
}));

export default function ScreenSettings() {
   const classes = useStyles();
   const { t } = useTranslation();
   const dispatch = useDispatch();

   const devices = useSelectorFactory(selectAvailableInputDevicesFactory, (state: RootState, selector) =>
      selector(state, 'screen'),
   );
   const isAvailable = useSelectorFactory(selectIsDeviceAvailableFactory, (state: RootState, selector) =>
      selector(state, 'screen'),
   );
   const device = useSelector((state: RootState) => state.settings.obj.screen.device);

   if (!isAvailable) {
      return (
         <div className={classes.root}>
            <Typography color="error">{t('conference.settings.screen.error_no_devices')}</Typography>
         </div>
      );
   }

   return (
      <div className={classes.root}>
         <DeviceSelector
            devices={devices}
            label={t('common:screen')}
            defaultName={t('common:screen')}
            selectedDevice={device}
            onChange={(device) => dispatch(setCurrentDevice({ device, source: 'screen' }))}
         />
      </div>
   );
}
