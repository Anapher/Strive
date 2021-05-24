import { useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import useStriveSound from 'src/hooks/useStriveSound';
import { RootState } from 'src/store';

export default function ConferenceOpenSound() {
   const [play] = useStriveSound('striveConferenceOpened');
   const playConferenceOpenSound = useSelector((state: RootState) => state.settings.obj.conference.playSoundOnOpen);

   const refs = useRef({ play, playConferenceOpenSound });

   useEffect(() => {
      refs.current.play = play;
      refs.current.playConferenceOpenSound = playConferenceOpenSound;
   }, [play, playConferenceOpenSound]);

   useEffect(() => {
      return () => {
         if (refs.current.playConferenceOpenSound) {
            refs.current.play();
         }
      };
   }, [refs.current]);

   return null;
}
