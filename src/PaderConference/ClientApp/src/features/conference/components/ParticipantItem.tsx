import { Typography } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import React from 'react';
import { ParticipantDto } from 'src/store/conference-signal/types';

type Props = {
   participant?: ParticipantDto;
};

export default function ParticipantItem({ participant }: Props) {
   return (
      <div>
         <Typography>{participant ? participant?.displayName : <Skeleton />}</Typography>
      </div>
   );
}
