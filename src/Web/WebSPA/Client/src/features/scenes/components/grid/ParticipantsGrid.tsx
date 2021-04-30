import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { Participant } from 'src/features/conference/types';
import { selectParticipantsOfRoomWebcamAvailable } from 'src/features/media/selectors';
import { selectParticipantsOfCurrentRoom } from 'src/features/rooms/selectors';
import { selectSceneOptions } from '../../selectors';
import { GridScene, RenderSceneProps } from '../../types';
import ActiveChipsLayout from '../ActiveChipsLayout';
import RenderGrid from './RenderGrid';

export default function ParticipantsGrid({ dimensions, className, next }: RenderSceneProps<GridScene>) {
   const participants = useSelector(selectParticipants);
   let visibleParticipants = useSelector(selectParticipantsOfCurrentRoom)
      .map((id) => participants[id])
      .filter((x): x is Participant => Boolean(x));
   const options = useSelector(selectSceneOptions);
   const participantsWithWebcam = useSelector(selectParticipantsOfRoomWebcamAvailable);

   const overwrite = next();
   if (overwrite) return <>{overwrite}</>;

   if (options?.hideParticipantsWithoutWebcam) {
      visibleParticipants = visibleParticipants.filter((x) => participantsWithWebcam.includes(x.id));

      return (
         <ActiveChipsLayout className={className}>
            <RenderGrid width={dimensions.width} height={dimensions.height - 32} participants={visibleParticipants} />
         </ActiveChipsLayout>
      );
   }

   return <RenderGrid {...dimensions} participants={visibleParticipants} />;
}
