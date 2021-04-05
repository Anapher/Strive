import { makeStyles } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { selectActiveParticipants } from '../../selectors';
import { RenderSceneProps } from '../../types';
import ParticipantTile from '../ParticipantTile';
import clsx from 'classnames';

const useStyles = makeStyles({
   root: {
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
   },
});

export default function ActiveSpeakerScene({ className, dimensions }: RenderSceneProps) {
   const classes = useStyles();

   const activeParticipants = useSelector(selectActiveParticipants);
   const participants = useSelector(selectParticipants);

   if (participants.length === 0) return null;

   return (
      <div className={clsx(className, classes.root)}>
         <div style={{ width: 300, height: 300 }}>
            <ParticipantTile size={{ width: 32, height: 32 }} participant={participants[0]} />
         </div>
      </div>
   );
}
