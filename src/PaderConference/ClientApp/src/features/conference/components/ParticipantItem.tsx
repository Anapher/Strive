import { makeStyles, Typography } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import React from 'react';
import { Roles } from 'src/consts';
import { ParticipantDto } from 'src/store/conference-signal/types';

const useStyles = makeStyles({
   root: {
      marginLeft: 16,
   },
});

type Props = {
   participant?: ParticipantDto;
};

export default function ParticipantItem({ participant }: Props) {
   const classes = useStyles();

   return (
      <div className={classes.root}>
         <Typography color={participant?.role === Roles.Moderator ? 'secondary' : undefined} variant="subtitle1">
            {participant ? participant?.displayName : <Skeleton />}
         </Typography>
      </div>
   );
}
