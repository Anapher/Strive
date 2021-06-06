import { Menu, MenuItem, PopoverProps } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { PollState, PollViewModel } from '../types';

type PollContextMenuProps = {
   open: boolean;
   anchorEl: PopoverProps['anchorEl'];
   onClose: () => void;

   poll: PollViewModel;
};

export default function PollContextMenu({ open, onClose, anchorEl, poll: { poll } }: PollContextMenuProps) {
   const { t } = useTranslation();
   const dispatch = useDispatch();

   const handleDeletePoll = () => {
      dispatch(coreHub.deletePoll({ pollId: poll.id }));
      onClose();
   };

   const handleUpdatePollState = (update: Partial<PollState>) => () => {
      dispatch(coreHub.updatePollState({ pollId: poll.id, state: { ...poll.state, ...update } }));
      onClose();
   };

   return (
      <Menu onClose={onClose} open={open} anchorEl={anchorEl}>
         {poll.state.isOpen ? (
            <MenuItem onClick={handleUpdatePollState({ isOpen: false })}>{t('conference.poll.close_poll')}</MenuItem>
         ) : (
            <MenuItem onClick={handleUpdatePollState({ isOpen: true })}>{t('conference.poll.open_poll')}</MenuItem>
         )}

         {poll.state.resultsPublished ? (
            <MenuItem onClick={handleUpdatePollState({ resultsPublished: false })}>
               {t('conference.poll.hide_results')}
            </MenuItem>
         ) : (
            <MenuItem onClick={handleUpdatePollState({ resultsPublished: true })}>
               {t('conference.poll.publish_results')}
            </MenuItem>
         )}

         <MenuItem onClick={handleDeletePoll}>{t('common:delete')}</MenuItem>
      </Menu>
   );
}
