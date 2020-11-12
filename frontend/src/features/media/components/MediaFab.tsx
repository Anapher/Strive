import { Fab, useTheme } from '@material-ui/core';
import React from 'react';
import { useDispatch } from 'react-redux';
import { AnimatedIconProps } from 'src/assets/animated-icons/AnimatedIconBase';
import { showMessage } from 'src/features/notifier/actions';
import { UseMediaState } from 'src/store/webrtc/hooks/useMedia';

type Props = {
   className?: string;
   control: UseMediaState;

   Icon: React.ComponentType<AnimatedIconProps>;
} & Omit<React.ComponentProps<typeof Fab>, 'children'>;

export default function MediaFab({ Icon, control: { enable, pause, resume, enabled, paused }, ...fabProps }: Props) {
   const dispatch = useDispatch();
   const theme = useTheme();

   const handleClick = async () => {
      if (!enabled) {
         try {
            await enable();
         } catch (error) {
            const { message } = error as DOMException;
            dispatch(showMessage({ message, variant: 'error' }));
         }
      } else {
         if (paused) {
            resume();
         } else {
            pause();
         }
      }
   };

   return (
      <Fab color={enabled ? 'primary' : 'default'} onClick={handleClick} {...fabProps}>
         <Icon
            activated={!paused}
            color={enabled ? theme.palette.primary.contrastText : theme.palette.background.default}
            width={24}
            height={24}
         />
      </Fab>
   );
}
