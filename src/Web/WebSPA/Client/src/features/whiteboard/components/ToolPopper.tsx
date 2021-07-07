import { ClickAwayListener, Grow, Paper, PopperProps } from '@material-ui/core';
import { Popper } from '@material-ui/core';
import React from 'react';

type Props = PopperProps & {
   onClose: () => void;
};

export default function ToolPopper({ children, onClose, ...props }: Props) {
   return (
      <Popper {...props} transition placement="right-start">
         {({ TransitionProps }) => (
            <Grow {...TransitionProps} style={{ transformOrigin: 'left top' }}>
               <Paper>
                  <ClickAwayListener onClickAway={onClose}>{children}</ClickAwayListener>
               </Paper>
            </Grow>
         )}
      </Popper>
   );
}
