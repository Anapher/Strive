import { Box, Button, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import { DateTime } from 'luxon';
import React from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { OpenBreakoutRoomsDto } from 'src/core-hub.types';
import { selectParticipantList } from 'src/features/conference/selectors';
import BreakoutRoomsForm from './BreakoutRoomsForm';

type Props = {
   onClose: () => void;
};

export default function CreateBreakoutRoomsDialog({ onClose }: Props) {
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const participants = useSelector(selectParticipantList);
   // participants = [
   //    ...participants,
   //    ...Array.from({ length: 15 }).map((_, i) => ({
   //       participantId: 'test-' + i,
   //       displayName: 'random ' + i,
   //       role: 'mod',
   //    })),
   // ];

   const form = useForm<OpenBreakoutRoomsDto>({
      defaultValues: {
         amount: participants != undefined ? Math.ceil(participants.length / 3) : 4,
         description: '',
         deadline: '15',
         assignedRooms: [],
      },
      mode: 'onChange',
   });

   const { formState, handleSubmit } = form;

   const handleApplyForm = (dto: OpenBreakoutRoomsDto) => {
      const action = coreHub.openBreakoutRooms({
         ...dto,
         amount: Number(dto.amount),
         deadline: dto.deadline
            ? DateTime.now()
                 .plus({ minutes: Number(dto.deadline) })
                 .toISO()
            : undefined,
      });

      dispatch(action);
      onClose();
   };

   return (
      <form onSubmit={handleSubmit(handleApplyForm)}>
         <DialogTitle id="breakout-room-dialog-title">{t('conference.dialog_breakout_rooms.create_title')}</DialogTitle>
         <DialogContent>
            <DialogContentText>{t('conference.dialog_breakout_rooms.create_description')}</DialogContentText>
            <Box mt={4}>
               <BreakoutRoomsForm form={form} participants={participants} />
            </Box>
         </DialogContent>
         <DialogActions>
            <Button color="primary" onClick={onClose}>
               {t('common:cancel')}
            </Button>
            <Button color="primary" disabled={!formState.isValid} type="submit">
               {t('conference.dialog_breakout_rooms.open_breakout_rooms')}
            </Button>
         </DialogActions>
      </form>
   );
}
