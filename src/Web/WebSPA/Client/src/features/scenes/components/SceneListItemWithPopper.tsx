import { IconButton, ListItemSecondaryAction, PopperProps } from '@material-ui/core';
import SettingsIcon from '@material-ui/icons/Settings';
import React, { useRef, useState } from 'react';
import { Scene } from '../types';
import SceneListItem, { Props as SceneListItemProps } from './SceneListItem';

export type ListItemPopperProps = {
   open: boolean;
   anchorEl: PopperProps['anchorEl'];
   onClose: () => void;

   scene: Scene;
};

type Props = SceneListItemProps & {
   PopperComponent: React.ComponentType<ListItemPopperProps>;
   listItemIcon: React.ReactNode;
};

export default function SceneListItemWithPopper({ PopperComponent, listItemIcon, ...props }: Props) {
   const [open, setOpen] = useState(false);
   const listItemRef = useRef(null);

   const handleOpen = () => setOpen(true);
   const handleClose = () => setOpen(false);

   return (
      <>
         <SceneListItem {...props}>
            <ListItemSecondaryAction>
               <IconButton edge="end" aria-label="options" onClick={handleOpen} ref={listItemRef}>
                  {listItemIcon}
               </IconButton>
            </ListItemSecondaryAction>
         </SceneListItem>

         <PopperComponent open={open} anchorEl={listItemRef.current} onClose={handleClose} scene={props.scene} />
      </>
   );
}
