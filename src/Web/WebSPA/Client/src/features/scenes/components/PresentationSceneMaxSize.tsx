import { Portal } from '@material-ui/core';
import React, { useContext, useState } from 'react';
import ConferenceLayoutContext from 'src/features/conference/conference-layout-context';
import ActiveParticipantsGrid from './ActiveParticipantsGrid';
import PresentationScene, { PresentationSceneProps } from './PresentationScene';

type Props = Omit<PresentationSceneProps, 'showParticipants'>;

export default function PresentationSceneMaxSize({
   canShowParticipantsWithoutResize,
   fixedParticipants,
   ...props
}: Props) {
   const handleCanShowParticipantsWithoutResize = (canShow: boolean) => {
      setShowParticipantOverlay(canShow);
      if (canShowParticipantsWithoutResize) canShowParticipantsWithoutResize(canShow);
   };

   const [showParticipantOverlay, setShowParticipantOverlay] = useState(false);
   const context = useContext(ConferenceLayoutContext);

   return (
      <>
         <PresentationScene
            {...props}
            fixedParticipants={fixedParticipants}
            showParticipants={showParticipantOverlay}
            canShowParticipantsWithoutResize={handleCanShowParticipantsWithoutResize}
         />
         {!showParticipantOverlay && (
            <Portal container={context.chatContainer}>
               <ActiveParticipantsGrid width={context.chatWidth} fixedParticipants={fixedParticipants} />
            </Portal>
         )}
      </>
   );
}
