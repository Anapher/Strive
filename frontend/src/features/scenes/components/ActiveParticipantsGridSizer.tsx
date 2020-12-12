import { motion } from 'framer-motion';
import React, { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { selectActiveParticipantsWithWebcam } from '../selectors';
import ActiveParticipantsGrid, {
   ACTIVE_PARTICIPANTS_MARGIN,
   ACTIVE_PARTICIPANTS_SECONDARY_TILES_SPACE,
   ACTIVE_PARTICIPANTS_WEBCAM_RATIO,
} from './ActiveParticipantsGrid';

type Props = {
   width: number;
   show: boolean;
};

export default function ActiveParticipantsGridSizer({ width, show }: Props) {
   const activeParticipants = useSelector(selectActiveParticipantsWithWebcam);
   const [height, setHeight] = useState(0);

   useEffect(() => {
      let newHeight = 0;

      if (activeParticipants.length > 0) {
         newHeight += ACTIVE_PARTICIPANTS_MARGIN * 2; // margin vertical
         newHeight += (width - ACTIVE_PARTICIPANTS_MARGIN * 2) * ACTIVE_PARTICIPANTS_WEBCAM_RATIO; // one full width webcam
      }

      if (activeParticipants.length > 1) {
         newHeight += ACTIVE_PARTICIPANTS_MARGIN; // margin between full width webcam and two separate
         newHeight +=
            ((width -
               ACTIVE_PARTICIPANTS_MARGIN * 2 -
               ACTIVE_PARTICIPANTS_SECONDARY_TILES_SPACE) /** space left, between, right */ /
               2) *
            ACTIVE_PARTICIPANTS_WEBCAM_RATIO; /** height of two half width webcams */
      }

      setHeight(newHeight);
   }, [width, activeParticipants]);

   console.log('height', height);

   return (
      <motion.div
         animate={{
            height: show ? height : 0,
            marginBottom: show ? 8 : 0,
         }}
         transition={{ type: 'tween' }}
         initial={{ height: 0, marginBottom: 0 }}
      >
         {show && height > 0 && <ActiveParticipantsGrid width={width} />}
      </motion.div>
   );
}
