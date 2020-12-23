import React, { useContext, useEffect, useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { selectParticipantsOfCurrentRoom } from 'src/features/rooms/selectors';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import AudioManager from '../AudioManager';
import { patchParticipantAudio } from '../reducer';
import { DEFAULT_PARTICIPANT_AUDIO } from '../sagas';
import { ParticipantAudioElement } from '../types';

const Context = React.createContext<AudioManager | null>(null);

type ParticipantAudioProps = {
   participantId: string;
   audioManager: AudioManager;
   onRegistered: () => void;
   onUnregistered: () => void;
   onSpeaking: () => void;
   onSpeakingStopped: () => void;
   speakingThreshold: number;
};

function ParticipantAudio({
   participantId,
   audioManager,
   onRegistered,
   onUnregistered,
   ...props
}: ParticipantAudioProps) {
   const consumer = useConsumer(participantId, 'mic');
   const audioOptions =
      useSelector((state: RootState) => state.media.participantAudio[participantId]) ?? DEFAULT_PARTICIPANT_AUDIO;

   const audioElem = useRef<HTMLAudioElement>(null);

   useEffect(() => {
      if (!consumer || !audioElem.current) return;

      audioManager.register(
         audioElem.current,
         participantId,
         consumer.track,
         props.onSpeaking,
         props.onSpeakingStopped,
         audioOptions.volume,
         audioOptions.muted,
         props.speakingThreshold,
      );
      onRegistered();

      return () => {
         audioManager.unregister(participantId);
         onUnregistered();
      };
   }, [audioElem.current, consumer]);

   useEffect(() => {
      if (audioElem.current) {
         audioElem.current.volume = audioOptions.volume;
         audioElem.current.muted = audioOptions.muted;
      }
   }, [audioOptions.volume, audioOptions.muted, audioElem.current]);

   return <audio ref={audioElem} autoPlay playsInline controls={false} />;
}

type Props = {
   children?: React.ReactNode;
};

export default function ParticipantMicManager({ children }: Props) {
   const audioManager = useRef(new AudioManager());
   const participants = useSelector(selectParticipantsOfCurrentRoom);

   const dispatch = useDispatch();

   const handleOnRegister = (participantId: string, data: boolean) => () => {
      dispatch(patchParticipantAudio({ participantId, data: { registered: data } }));
   };

   const handleOnSpeaking = (participantId: string, data: boolean) => () => {
      dispatch(patchParticipantAudio({ participantId, data: { speaking: data } }));
   };

   return (
      <Context.Provider value={audioManager.current}>
         {participants.map((x) => (
            <ParticipantAudio
               key={x}
               participantId={x}
               audioManager={audioManager.current}
               onRegistered={handleOnRegister(x, true)}
               onUnregistered={handleOnRegister(x, false)}
               onSpeaking={handleOnSpeaking(x, true)}
               onSpeakingStopped={handleOnSpeaking(x, false)}
               speakingThreshold={2}
            />
         ))}
         {children}
      </Context.Provider>
   );
}

export const useParticipantAudio = (participantId: string) => {
   const context = useContext(Context);
   const [audioInfo, setAudioInfo] = useState<ParticipantAudioElement | undefined>(undefined);

   useEffect(() => {
      if (!context) return;

      const onUpdate = ({ participantId: eventId }: { participantId: string }) => {
         if (eventId === participantId) {
            setAudioInfo(context.audioElems.get(participantId));
         }
      };

      onUpdate({ participantId });
      context.on('update', onUpdate);
      return () => {
         context.off('update', onUpdate);
      };
   }, [context]);

   return audioInfo;
};
