import { Chip } from '@material-ui/core';
import { DateTime } from 'luxon';
import React from 'react';
import Countdown from 'react-countdown';
import { useTranslation } from 'react-i18next';
import CountdownRenderer from 'src/components/CountdownRenderer';
import { BreakoutRoomsConfig } from 'src/core-hub.types';

type Props = {
   state: BreakoutRoomsConfig;
   className?: string;
};

export default function BreakoutRoomChip({ state, className }: Props) {
   const deadline = state.deadline ? DateTime.fromISO(state.deadline) : undefined;
   const { t } = useTranslation();

   return (
      <Chip
         id="appbar-status-chip-breakoutrooms"
         className={className}
         label={
            <>
               <span>
                  {state.description && `${state.description} | `}
                  {t('conference.appbar.breakout_room_chip')}
                  {deadline && (
                     <span>
                        {' '}
                        {t('conference.appbar.breakout_room_chip_deadline', {
                           deadline: deadline.toLocaleString(DateTime.TIME_24_SIMPLE),
                        }) + ' ('}
                        <Countdown date={deadline.toJSDate()} renderer={CountdownRenderer} />
                        {')'}
                     </span>
                  )}
               </span>
            </>
         }
         color="primary"
         size="small"
      />
   );
}
