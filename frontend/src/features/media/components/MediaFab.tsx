import { Fab, SvgIconTypeMap } from '@material-ui/core';
import { OverridableComponent } from '@material-ui/core/OverridableComponent';
import React from 'react';
import { UseMediaState } from 'src/store/webrtc/hooks/useMedia';

type Props = {
   className?: string;
   control: UseMediaState;

   IconEnable: OverridableComponent<SvgIconTypeMap>;
   IconDisable: OverridableComponent<SvgIconTypeMap>;
} & Omit<React.ComponentProps<typeof Fab>, 'children'>;

export default function MediaFab({
   IconDisable,
   IconEnable,
   control: { enable, pause, resume, enabled, paused },
   ...fabProps
}: Props) {
   const handleClick = () => {
      if (!enabled) {
         enable();
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
         {paused ? <IconEnable /> : <IconDisable />}
      </Fab>
   );
}
