import { makeStyles, Typography } from '@material-ui/core';
import { DateTime } from 'luxon';
import React from 'react';
import Countdown from 'react-countdown';
import { useSelector } from 'react-redux';
import CountdownRenderer from 'src/components/CountdownRenderer';
import { selectBreakoutRoomState } from 'src/features/breakout-rooms/selectors';
import { selectRoomViewModels } from 'src/features/rooms/selectors';
import { RenderSceneProps } from '../../types';
import clsx from 'classnames';

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

export default function BreakoutRoomScene({ className }: RenderSceneProps) {
   const classes = useStyles();

   const state = useSelector(selectBreakoutRoomState);
   const rooms = useSelector(selectRoomViewModels);

   if (!state) return null;

   return (
      <div className={clsx(className, classes.root)}>
         <div className={classes.content}>
            {state.deadline && (
               <Typography variant="h1">
                  <Countdown date={DateTime.fromISO(state.deadline).toJSDate()} renderer={CountdownRenderer} />
               </Typography>
            )}
            <Typography variant="h5">{state.description}</Typography>
         </div>
      </div>
   );
}
