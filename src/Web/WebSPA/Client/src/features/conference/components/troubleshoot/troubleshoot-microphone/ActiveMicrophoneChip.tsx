import { Chip } from '@material-ui/core';
import { motion, useMotionTemplate } from 'framer-motion';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectAvailableInputDevicesFactory } from 'src/features/settings/selectors';
import useConsumerMediaStream from 'src/hooks/useConsumerMediaStream';
import useMediaStreamMotionAudioLevel from 'src/hooks/useMediaStreamMotionAudioLevel';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import useSelectorFactory from 'src/hooks/useSelectorFactory';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import { findMicrophoneLabel, getDefaultDevice } from './utilts';

export default function ActiveMicrophoneChip() {
   const mics = useSelectorFactory(selectAvailableInputDevicesFactory, (state: RootState, selector) =>
      selector(state, 'mic'),
   );

   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device) ?? getDefaultDevice(mics);

   const myId = useMyParticipantId();
   const consumer = useConsumer(myId, 'loopback-mic');

   const stream = useConsumerMediaStream(consumer);
   const currentAudioLevel = useMediaStreamMotionAudioLevel(stream);
   const audioColor = useMotionTemplate`rgba(39, 174, 96, ${currentAudioLevel})`;

   return (
      <Chip
         component={motion.span}
         style={{ backgroundColor: audioColor as any, cursor: 'pointer', textOverflow: 'ellipsis', width: '100%' }}
         size="small"
         label={audioDevice && findMicrophoneLabel(audioDevice, mics)}
      />
   );
}
