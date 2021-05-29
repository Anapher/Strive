import { Box, Button, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import { compare } from 'fast-json-patch';
import { DateTime } from 'luxon';
import React, { useRef } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { BreakoutRoomsConfig, OpenBreakoutRoomsDto } from 'src/core-hub.types';
import BreakoutRoomsForm from './BreakoutRoomsForm';

type Props = {
   onClose: () => void;
   active: BreakoutRoomsConfig;
};

export default function UpdateBreakoutRoomsDialog({ active, onClose }: Props) {
   const dispatch = useDispatch();
   const { t } = useTranslation();

   // the duration changes every second, so remmeber the initial duration to create a meaningful diff
   const initialDuration = useRef(
      active.deadline ? Math.ceil(DateTime.fromISO(active.deadline).diffNow().as('minutes')) : undefined,
   );

   const form = useForm<OpenBreakoutRoomsDto>({
      defaultValues: {
         amount: active.amount,
         description: active.description,
         deadline: initialDuration.current?.toString(),
      },
      mode: 'onChange',
   });

   const { formState, handleSubmit } = form;

   const handleApplyForm = (dto: OpenBreakoutRoomsDto) => {
      let deadline: string | undefined;
      if (dto.deadline) {
         const diff = initialDuration.current ? Number(dto.deadline) - initialDuration.current : Number(dto.deadline);
         const baseDate = active.deadline ? DateTime.fromISO(active.deadline) : DateTime.now();
         deadline = baseDate.plus({ minutes: diff }).toISO();
      }

      const newData: OpenBreakoutRoomsDto = {
         ...dto,
         amount: Number(dto.amount),
         deadline,
      };

      const currentData: OpenBreakoutRoomsDto = {
         amount: active.amount,
         deadline: active.deadline,
         description: active.description,
      };

      const operations = compare(currentData, newData);
      dispatch(coreHub.changeBreakoutRooms(operations));
      onClose();
   };

   return (
      <>
         <DialogTitle id="breakout-room-dialog-title">{t('conference.dialog_breakout_rooms.update_title')}</DialogTitle>
         <DialogContent>
            <DialogContentText>{t('conference.dialog_breakout_rooms.update_description')}</DialogContentText>
            <form onSubmit={handleSubmit(handleApplyForm)} id="breakout-room-dialog-form">
               <Box mt={4}>
                  <BreakoutRoomsForm form={form} participants={null} />
               </Box>
            </form>
         </DialogContent>
         <DialogActions>
            <Button color="primary" onClick={onClose}>
               {t('common:cancel')}
            </Button>
            <Button color="primary" disabled={!formState.isValid} type="submit" form="breakout-room-dialog-form">
               {t('conference.dialog_breakout_rooms.update_breakout_rooms')}
            </Button>
         </DialogActions>
      </>
   );
}
