import { Box, Typography } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import React from 'react';
import { Roles } from 'src/consts';
import { ParticipantDto } from 'src/store/conference-signal/types';

type Props = {
   participant?: ParticipantDto;
};

export default function ParticipantItem({ participant }: Props) {
   return (
      <Box mb={1}>
         <Typography color={participant?.role === Roles.Moderator ? 'secondary' : undefined}>
            {participant ? participant?.displayName : <Skeleton />}
         </Typography>
      </Box>
   );
}
