import { useSelector } from 'react-redux';
import { selectParticipantAudio } from './selectors';

export default function useSpeakingParticipants() {
   const asd = useSelector(selectParticipantAudio);
}
