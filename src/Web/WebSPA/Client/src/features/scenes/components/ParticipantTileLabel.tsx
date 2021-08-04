import { fade, makeStyles, Typography, useTheme } from '@material-ui/core';
import clsx from 'classnames';
import React, { RefObject } from 'react';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';

const INFO_BOX_MARGIN = 16;

const useStyles = makeStyles((theme) => ({
   infoBox: {
      position: 'absolute',
      display: 'flex',
      alignItems: 'center',
      flexDirection: 'row',
      transition: theme.transitions.create('padding', {
         duration: theme.transitions.duration.shorter,
         easing: theme.transitions.easing.easeOut,
      }),

      left: INFO_BOX_MARGIN,
      bottom: INFO_BOX_MARGIN,
   },
   infoBoxWebcamDisabled: {
      alignItems: 'flex-end',
   },
   infoBoxWebcamEnabled: {
      backgroundColor: fade(theme.palette.background.paper, 0.75),
      borderRadius: theme.shape.borderRadius,
      paddingLeft: theme.spacing(1),
      alignItems: 'center',
   },
   infoBoxEnabledSmall: {
      left: theme.spacing(1),
      bottom: theme.spacing(1),
   },
   label: {
      transition: theme.transitions.create('transform', {
         duration: theme.transitions.duration.standard,
         easing: theme.transitions.easing.easeOut,
      }),
   },
}));

type Props = {
   micActivated: boolean;
   webcamActivated: boolean;
   label: string;
   dense?: boolean;
   containerRef: RefObject<any> | null | undefined;
};

export default function ParticipantTileLabel({ micActivated, webcamActivated, containerRef, label, dense }: Props) {
   const classes = useStyles();
   const theme = useTheme();
   const [containerSize] = useThrottledResizeObserver(100, { ref: containerRef });

   if (!containerSize) return null;

   return (
      <div
         className={clsx(classes.infoBox, {
            [classes.infoBoxWebcamEnabled]: webcamActivated,
            [classes.infoBoxWebcamDisabled]: !webcamActivated,
            [classes.infoBoxEnabledSmall]: webcamActivated && dense,
         })}
      >
         <AnimatedMicIcon activated={micActivated} disabledColor={theme.palette.error.main} />
         <Typography
            variant="h4"
            className={clsx(classes.label)}
            style={{
               fontSize: dense ? 16 : 24,
               transform: webcamActivated
                  ? 'translate(0px, 0px) scale(0.75)'
                  : `translate(calc(${(containerSize?.width ?? 0) / 2}px - ${INFO_BOX_MARGIN}px - 18px - 50%), calc(-${
                       (containerSize?.height ?? 0) / 2
                    }px + 50% + ${INFO_BOX_MARGIN}px))`,
            }}
         >
            {label}
         </Typography>
      </div>
   );
}
