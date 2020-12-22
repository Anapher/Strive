import { makeStyles, Menu } from '@material-ui/core';
import React from 'react';
import { ParticipantDto } from '../types';
import ParticipantContextMenu from './ParticipantContextMenu';

const useStyles = makeStyles(() => ({
   menu: {
      width: 240,
   },
}));

type Props = Omit<React.ComponentProps<typeof Menu>, 'children'> & {
   onClose: () => void;
   participant: ParticipantDto;
};

export default function ParticipantContextMenuPopper({ participant, onClose, ...props }: Props) {
   const classes = useStyles();

   return (
      <Menu {...props} onClose={onClose} MenuListProps={{ className: classes.menu }}>
         <ParticipantContextMenu participant={participant} onClose={onClose} />
      </Menu>
   );
}
