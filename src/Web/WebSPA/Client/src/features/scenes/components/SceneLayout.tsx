import { Portal } from '@material-ui/core';
import React, { useContext, useMemo } from 'react';
import ConferenceLayoutContext from 'src/features/conference/conference-layout-context';
import { Participant } from 'src/features/conference/types';
import { Size } from 'src/types';
import LayoutChildSizeContext from '../layout-child-size-context';
import useSomeParticipants from '../useSomeParticipants';
import ActiveChipsLayout, { ACTIVE_CHIPS_LAYOUT_HEIGHT } from './ActiveChipsLayout';
import ParticipantTile from './ParticipantTile';
import TilesBarLayout from './TilesBarLayout';

type SceneLayoutType = 'tiles-bar' | 'presentator' | 'chips';

type Props = {
   children?: React.ReactNode;
   className?: string;
   type: SceneLayoutType;
   participant?: Participant;
   width: number;
   height: number;
};

export default function SceneLayout({ type, ...props }: Props) {
   switch (type) {
      case 'chips':
         return <SceneLayoutChips {...props} />;
      case 'presentator':
         return <SceneLayoutPresentator {...props} />;
      default:
         return <SceneLayoutTiles {...props} />;
   }
}

function SceneLayoutChips({ className, children, width, height }: Omit<Props, 'type'>) {
   const size = useMemo(() => ({ width, height: height - ACTIVE_CHIPS_LAYOUT_HEIGHT }), [width, height]);

   return (
      <LayoutChildSizeContext.Provider value={size}>
         <ActiveChipsLayout className={className}>{children}</ActiveChipsLayout>
      </LayoutChildSizeContext.Provider>
   );
}

function SceneLayoutPresentator(props: Omit<Props, 'type'>) {
   return (
      <>
         <PortalWithParticipant participant={props.participant} />
         <SceneLayoutChips {...props} />
      </>
   );
}

function SceneLayoutTiles({ participant, width, height, className, children }: Omit<Props, 'type'>) {
   const participants = useSomeParticipants(1000, {
      includedParticipants: participant ? [participant] : undefined,
   });

   return (
      <TilesBarLayout participants={participants} sceneSize={{ width, height }} className={className}>
         {children}
      </TilesBarLayout>
   );
}

export const ACTIVE_PARTICIPANTS_WEBCAM_RATIO = 9 / 16;
type PortalWithParticipantProps = {
   participant?: Participant;
};
function PortalWithParticipant({ participant }: PortalWithParticipantProps) {
   const context = useContext(ConferenceLayoutContext);
   const participants = useSomeParticipants(1, {
      includedParticipants: participant ? [participant] : undefined,
      webcamOnly: true,
   });

   const width = context.chatWidth - 8;
   const size: Size = { width, height: width * ACTIVE_PARTICIPANTS_WEBCAM_RATIO };

   return (
      <Portal container={context.chatContainer}>
         {participants.length > 0 && (
            <div style={{ ...size, marginBottom: 8, marginRight: 8 }} key={participants[0].id}>
               <ParticipantTile {...size} participant={participants[0]} />
            </div>
         )}
      </Portal>
   );
}
