import { makeStyles } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectBreakoutRoomState } from 'src/features/breakout-rooms/selectors';
import { PollScene, RenderSceneProps } from '../../types';
import ActiveChipsLayout from '../ActiveChipsLayout';

const useStyles = makeStyles({
   root: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
   },
   content: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
});

export default function RenderPollScene({ className, next }: RenderSceneProps<PollScene>) {
   const classes = useStyles();
   const state = useSelector(selectBreakoutRoomState);

   const overwrite = next();
   if (overwrite) return <>{overwrite}</>;

   if (!state) return null;

   return (
      <ActiveChipsLayout className={className} contentClassName={classes.root}>
         <div className={classes.content}>Poll</div>
      </ActiveChipsLayout>
   );
}
