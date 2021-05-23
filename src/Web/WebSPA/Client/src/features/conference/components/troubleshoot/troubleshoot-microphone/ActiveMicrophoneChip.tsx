import { Chip } from '@material-ui/core';
import { motion, useMotionTemplate } from 'framer-motion';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectAvailableInputDevices } from 'src/features/settings/selectors';
import useConsumerMediaStream from 'src/hooks/useConsumerMediaStream';
import useMediaStreamMotionAudioLevel from 'src/hooks/useMediaStreamMotionAudioLevel';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import { findMicrophoneLabel, getDefaultDevice } from './utilts';

export default function ActiveMicrophoneChip() {
   const mics = useSelector((state: RootState) => selectAvailableInputDevices(state, 'mic'));
   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device) ?? getDefaultDevice(mics);

   const myId = useMyParticipantId();
   const consumer = useConsumer(myId, 'loopback-mic');

   const stream = useConsumerMediaStream(consumer);
   const currentAudioLevel = useMediaStreamMotionAudioLevel(stream);
   const audioColor = useMotionTemplate`rgba(39, 174, 96, ${currentAudioLevel})`;

   return (
      <Chip
         component={motion.div}
         style={{ backgroundColor: audioColor as any, cursor: 'pointer' }}
         size="small"
         label={audioDevice && findMicrophoneLabel(audioDevice, mics)}
      />
   );
}
