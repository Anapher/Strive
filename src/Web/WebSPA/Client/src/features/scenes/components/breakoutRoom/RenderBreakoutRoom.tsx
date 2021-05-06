import { makeStyles, Typography } from '@material-ui/core';
import { DateTime } from 'luxon';
import React from 'react';
import Countdown from 'react-countdown';
import { useSelector } from 'react-redux';
import CountdownRenderer from 'src/components/CountdownRenderer';
import { selectBreakoutRoomState } from 'src/features/breakout-rooms/selectors';
import { BreakoutRoomScene, RenderSceneProps } from '../../types';
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

export default function RenderBreakoutRoom({ className, next }: RenderSceneProps<BreakoutRoomScene>) {
   const classes = useStyles();
   const state = useSelector(selectBreakoutRoomState);

   const overwrite = next();
   if (overwrite) return <>{overwrite}</>;

   if (!state) return null;

   return (
      <ActiveChipsLayout className={className} contentClassName={classes.root}>
         <div className={classes.content}>
            {state.deadline && (
               <Typography variant="h1">
                  <Countdown date={DateTime.fromISO(state.deadline).toJSDate()} renderer={CountdownRenderer} />
               </Typography>
            )}
            <Typography variant="h5">{state.description}</Typography>
         </div>
      </ActiveChipsLayout>
   );
}
