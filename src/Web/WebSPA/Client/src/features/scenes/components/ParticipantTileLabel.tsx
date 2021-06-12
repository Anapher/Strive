import { fade, makeStyles, Typography, useTheme } from '@material-ui/core';
import clsx from 'classnames';
import React from 'react';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';

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
   tileWidth: number;
   tileHeight: number;
   label: string;
};

export default function ParticipantTileLabel({ micActivated, webcamActivated, tileWidth, tileHeight, label }: Props) {
   const classes = useStyles();
   const theme = useTheme();

   const isSmall = tileWidth < 400;

   return (
      <div
         className={clsx(classes.infoBox, {
            [classes.infoBoxWebcamEnabled]: webcamActivated,
            [classes.infoBoxWebcamDisabled]: !webcamActivated,
            [classes.infoBoxEnabledSmall]: webcamActivated && isSmall,
         })}
      >
         <AnimatedMicIcon activated={micActivated} disabledColor={theme.palette.error.main} />
         <Typography
            variant="h4"
            className={clsx(classes.label)}
            style={{
               fontSize: isSmall ? 16 : 24,
               transform: webcamActivated
                  ? 'translate(0px, 0px) scale(0.75)'
                  : `translate(calc(${tileWidth / 2}px - ${INFO_BOX_MARGIN}px - 18px - 50%), calc(-${
                       tileHeight / 2
                    }px + 50% + ${INFO_BOX_MARGIN}px))`,
            }}
         >
            {label}
         </Typography>
      </div>
   );
}
