import { Fab, Tooltip, useTheme } from '@material-ui/core';
import React from 'react';
import { AnimatedIconProps } from 'src/assets/animated-icons/AnimatedIconBase';
import { UseMediaState } from 'src/store/webrtc/hooks/useMedia';
import useMediaButton from '../useMediaButton';

type Props = {
   className?: string;
   control: UseMediaState;
   pauseOnToggle?: boolean;
   translationKey: 'screen' | 'webcam' | 'mic';

   Icon: React.ComponentType<AnimatedIconProps>;
   [x: string]: any;
} & Omit<React.ComponentProps<typeof Fab>, 'children'>;

export default function MediaFab({ Icon, pauseOnToggle, control, translationKey, ...fabProps }: Props) {
   const theme = useTheme();
   const { enabled } = control;

   const { handleClick, title, label, id, activated } = useMediaButton(Boolean(pauseOnToggle), control, translationKey);

   return (
      <Tooltip title={title} aria-label={label} arrow>
         <Fab id={id} color={enabled ? 'primary' : 'default'} onClick={handleClick} {...fabProps}>
            <Icon
               activated={activated}
               color={enabled ? theme.palette.primary.contrastText : theme.palette.background.default}
               width={24}
               height={24}
            />
         </Fab>
      </Tooltip>
   );
}
