import { makeStyles, Portal } from '@material-ui/core';
import clsx from 'classnames';
import React, { useContext, useMemo } from 'react';
import ConferenceLayoutContext from 'src/features/conference/conference-layout-context';
import { Participant } from 'src/features/conference/types';
import { Size } from 'src/types';
import LayoutChildSizeContext from '../layout-child-size-context';
import { computeTilesSceneBar } from '../tile-frame-calculations';
import TilesBarLayoutRow from './TilesBarLayoutRow';

const MARGIN_HORIZONTAL = 8;

const useStyles = makeStyles({
   root: {
      width: '100%',
      height: '100%',
      paddingTop: 0,
      display: 'flex',
      flexDirection: 'column',
      justifyContent: 'flex-start',
   },
   child: {
      flex: 1,
      minHeight: 0,
      marginTop: 8,
   },
});

type Props = {
   participants: Participant[];
   sceneSize: Size;
   className?: string;
   children?: React.ReactNode;
};

export default function TilesBarLayout({ participants, sceneSize, children, className }: Props) {
   const context = useContext(ConferenceLayoutContext);
   const classes = useStyles();

   const sceneBarWidth = context.sceneBarWidth;

   const [instructions, usePortal] = useMemo(() => {
      if (!sceneBarWidth) return [undefined, false];

      let instructions = computeTilesSceneBar({
         tileMinWidth: 260,
         tileSize: { width: 16, height: 9 },
         tileSpaceBetween: 8,
         width: sceneSize.width - MARGIN_HORIZONTAL * 2,
      });

      if (participants.length > instructions.tileAmount) {
         instructions = computeTilesSceneBar({
            tileMinWidth: 200,
            tileSize: { width: 16, height: 9 },
            tileSpaceBetween: 8,
            width: sceneBarWidth - MARGIN_HORIZONTAL * 2,
         });
         return [instructions, true];
      }

      return [instructions, false];
   }, [sceneBarWidth, sceneSize.width, participants.length]);

   const topOffset = (instructions?.tileSize.height ?? 0) + 8;
   const childSize = useMemo(
      () => ({ ...sceneSize, height: usePortal ? sceneSize.height : sceneSize.height - topOffset, topOffset }),
      [usePortal, sceneSize.height, sceneSize.width, topOffset],
   );

   if (usePortal) {
      return (
         <div className={classes.root}>
            {instructions && (
               <Portal container={context.sceneBarContainer}>
                  <TilesBarLayoutRow
                     marginHorizontal={MARGIN_HORIZONTAL}
                     instructions={instructions}
                     participants={participants}
                  />
               </Portal>
            )}
            <LayoutChildSizeContext.Provider value={childSize}>
               <div className={classes.child}>{children}</div>
            </LayoutChildSizeContext.Provider>
         </div>
      );
   }

   return (
      <div className={clsx(className, classes.root)}>
         {instructions && (
            <TilesBarLayoutRow
               marginHorizontal={MARGIN_HORIZONTAL}
               instructions={instructions}
               participants={participants}
            />
         )}
         <LayoutChildSizeContext.Provider value={childSize}>
            <div className={classes.child}>{children}</div>
         </LayoutChildSizeContext.Provider>
      </div>
   );
}
