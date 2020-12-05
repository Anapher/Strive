import {
   Accordion,
   AccordionDetails,
   AccordionSummary,
   Button,
   makeStyles,
   Typography,
   useTheme,
} from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { fetchDevices } from 'src/features/settings/thunks';
import { DeviceGroup, selectAvailableInputDevices } from 'src/features/settings/selectors';
import { EquipmentInputDevice, LocalInputDevice } from 'src/features/settings/types';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import { selectMyParticipantId } from 'src/features/auth/selectors';
import hark from 'hark';
import { motion, MotionValue, useMotionTemplate, useMotionValue, useTransform } from 'framer-motion';
import useMicrophone from 'src/store/webrtc/hooks/useMicrophone';
import useDeviceManagement from 'src/features/media/useDeviceManagement';

const useStyles = makeStyles((theme) => ({
   heading: {
      fontSize: theme.typography.pxToRem(15),
      flex: 1,
   },
   playButton: {
      width: 160,
      marginRight: theme.spacing(2),
   },
   accordionSummary: {
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'center',
      flex: 1,
   },
}));

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
};

export default function TroubleshootMicrophone({ expanded, onChange }: Props) {
   const classes = useStyles();
   const handleChange = (_: React.ChangeEvent<unknown>, isExpanded: boolean) => {
      onChange(isExpanded);
   };
   const dispatch = useDispatch();

   const handleUpdateMics = () => dispatch(fetchDevices());

   return (
      <Accordion expanded={expanded} onChange={handleChange}>
         <AccordionSummary
            expandIcon={<ExpandMoreIcon />}
            aria-controls="troubleshoot-microphone-content"
            id="troubleshoot-microphone-header"
         >
            <div className={classes.accordionSummary}>
               <Typography className={classes.heading}>Microphone</Typography>
            </div>
         </AccordionSummary>
         <AccordionDetails>
            <div>
               <MicrophoneView />
               <Button onClick={handleUpdateMics} variant="contained" color="primary">
                  Update connected microphones
               </Button>
            </div>
         </AccordionDetails>
      </Accordion>
   );
}

function MicrophoneView() {
   const audioDevices = useSelector((state: RootState) => selectAvailableInputDevices(state, 'mic'));
   const selected = useSelector((state: RootState) => state.settings.obj.mic.device);
   const theme = useTheme();

   if (audioDevices.length === 0) {
      return (
         <div>
            <Typography gutterBottom color="error">
               {"It seems like you don't have any microphones connected. Please connect a microphone to your pc."}
            </Typography>
         </div>
      );
   }

   const deviceName = getDeviceName(selected ?? audioDevices[0]?.devices[0]?.device, audioDevices);

   return (
      <div>
         <Typography gutterBottom>
            You have selected <span style={{ color: theme.palette.secondary.main }}>{deviceName}</span> as your
            microphone.
         </Typography>
         <MicrophoneLoopbackView />
      </div>
   );
}

function MicrophoneLoopbackView() {
   const myId = useSelector(selectMyParticipantId);
   const consumer = useConsumer(myId, 'loopback-mic');
   const audioLevel = useMotionValue(0);

   console.log('loopback consumer', consumer);

   const gain = useSelector((state: RootState) => state.settings.obj.mic.audioGain);

   const localMic = useMicrophone(gain, true);
   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device);
   const micController = useDeviceManagement('loopback-mic', localMic, audioDevice);

   useEffect(() => {
      // the mic is automatically disabled on component unmount
      micController.enable();
   }, []);

   const audioRef = useRef<HTMLAudioElement>(null);

   useEffect(() => {
      if (consumer) {
         const stream = new MediaStream();
         stream.addTrack(consumer.track);

         const analyser = hark(stream, { play: false });
         analyser.on('volume_change', (dBs) => {
            // The exact formula to convert from dBs (-100..0) to linear (0..1) is:
            //   Math.pow(10, dBs / 20)
            // However it does not produce a visually useful output, so let exagerate
            // it a bit. Also, let convert it from 0..1 to 0..10 and avoid value 1 to
            // minimize component renderings.
            let audioVolume = Math.round(Math.pow(10, dBs / 85) * 10);

            if (audioVolume === 1) audioVolume = 0;

            console.log(audioVolume);

            audioLevel.set(audioVolume / 10);
         });

         if (audioRef.current) {
            audioRef.current.srcObject = stream;
            audioRef.current.play();
         }

         return () => analyser.stop();
      }
   }, [consumer]);

   const transform = useTransform(audioLevel, [0, 1], [0, 2]);
   const audioBorder = useMotionTemplate`rgba(39, 174, 96, ${transform})`;

   return (
      <div>
         <audio ref={audioRef} muted />
         <motion.div style={{ width: '100%', height: 30, backgroundColor: audioBorder }} />
      </div>
   );
}

function getDeviceName(device: LocalInputDevice | EquipmentInputDevice, allDevices: DeviceGroup[]): string | undefined {
   if (device.type === 'local') {
      return allDevices.find((x) => x.type === 'local')?.devices.find((x) => x.device.deviceId === device.deviceId)
         ?.label;
   }

   return allDevices
      .find((x) => x.type === 'equipment' && x.equipmentId === device.equipmentId)
      ?.devices.find((x) => x.device.deviceId === device.deviceId)?.label;
}
