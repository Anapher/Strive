import { makeStyles, Menu } from '@material-ui/core';
import React from 'react';
import { Participant } from '../types';
import ParticipantContextMenu from './ParticipantContextMenu';

const useStyles = makeStyles(() => ({
   menu: {
      minWidth: 256,
   },
}));

type Props = Omit<React.ComponentProps<typeof Menu>, 'children'> & {
   onClose: () => void;
   participant: Participant;
};

export default function ParticipantContextMenuPopper({ participant, onClose, ...props }: Props) {
   const classes = useStyles();

   return (
      <Menu {...props} onClose={onClose} MenuListProps={{ className: classes.menu }}>
         <ParticipantContextMenu participant={participant} onClose={onClose} />
      </Menu>
   );
}
