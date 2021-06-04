import { ClickAwayListener, Grow, Paper, Popper, PopperProps } from '@material-ui/core';
import React from 'react';

type Props = {
   open: boolean;
   anchorEl: PopperProps['anchorEl'];
   onClose: () => void;
   children?: React.ReactNode;

   placement: PopperProps['placement'];
   transformOrigin: string;
};

export default function PopperWrapper({ open, anchorEl, onClose, children, placement, transformOrigin }: Props) {
   return (
      <Popper open={open} anchorEl={anchorEl} transition placement={placement}>
         {({ TransitionProps }) => (
            <Grow {...TransitionProps} style={{ transformOrigin }}>
               <Paper>
                  <ClickAwayListener onClickAway={onClose}>{children}</ClickAwayListener>
               </Paper>
            </Grow>
         )}
      </Popper>
   );
}
