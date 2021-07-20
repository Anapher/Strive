import React, { useMemo } from 'react';
import { Size } from 'src/types';
import { computeGridTop } from '../../tile-frame-calculations';
import { ActiveSpeakerScene, RenderSceneProps } from '../../types';
import useSomeParticipants from '../../useSomeParticipants';
import ActiveChipsLayout, { ACTIVE_CHIPS_LAYOUT_HEIGHT } from '../ActiveChipsLayout';
import ParticipantTile from '../ParticipantTile';
import TileFrameGridTop from '../TileFrameGridTop';

const getListeningParticipantsWidth = (width: number) => {
   if (width <= 400) return 100;
   if (width <= 800) return 180;
   if (width <= 1200) return 260;

   return 340;
};

const participantTileRatio: Size = { width: 16, height: 9 };

export default function RenderActiveSpeaker({ className, dimensions }: RenderSceneProps<ActiveSpeakerScene>) {
   const instructions = useMemo(
      () =>
         computeGridTop({
            contentRatio: participantTileRatio,
            dimensions: { width: dimensions.width - 16, height: dimensions.height - ACTIVE_CHIPS_LAYOUT_HEIGHT - 8 },
            tileMinWidth: 260,
            tileSize: participantTileRatio,
            tileSpaceBetween: 8,
            tilesMargin: { left: 8, right: 8, bottom: 8, top: 0 },
            contentMargin: { left: 8, right: 8, bottom: 48, top: 0 },
         }),
      [dimensions.width, dimensions.height],
   );

   const activeParticipants = useSomeParticipants(instructions.tileAmount);
   if (activeParticipants.length === 0) return null;

   return (
      <ActiveChipsLayout className={className}>
         <TileFrameGridTop
            instructions={instructions}
            participants={activeParticipants.slice(1)}
            render={(size) => (
               <div
                  style={{
                     width: '100%',
                     height: '100%',
                     display: 'flex',
                     flexDirection: 'column',
                     alignItems: 'center',
                     justifyContent: 'center',
                  }}
               >
                  <div style={{ ...size }}>
                     <ParticipantTile key={activeParticipants[0].id} {...size} participant={activeParticipants[0]} />
                  </div>
               </div>
            )}
         />
      </ActiveChipsLayout>
   );
}
