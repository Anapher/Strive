import { Box, Button, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import { compare } from 'fast-json-patch';
import { DateTime, Duration } from 'luxon';
import React, { useRef } from 'react';
import { useForm } from 'react-hook-form';
import { useDispatch } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { OpenBreakoutRoomsDto } from 'src/core-hub.types';
import { ActiveBreakoutRoomState } from '../types';
import BreakoutRoomsForm from './BreakoutRoomsForm';

type Props = {
   onClose: () => void;
   active: ActiveBreakoutRoomState;
};

export default function UpdateBreakoutRoomsDialog({ active, onClose }: Props) {
   const dispatch = useDispatch();

   // the duration changes every second, so remmeber the initial duration to create a meaningful diff
   const initialDuration = useRef(
      active.deadline ? Math.ceil(DateTime.fromISO(active.deadline).diffNow().as('minutes')).toString() : undefined,
   );

   const form = useForm<OpenBreakoutRoomsDto>({
      defaultValues: {
         amount: active.amount,
         description: active.description,
         duration: initialDuration.current,
      },
      mode: 'onChange',
   });

   const { formState, handleSubmit } = form;

   const handleApplyForm = (dto: OpenBreakoutRoomsDto) => {
      const newData: OpenBreakoutRoomsDto = {
         ...dto,
         amount: Number(dto.amount),
         duration: dto.duration ? Duration.fromObject({ minutes: Number(dto.duration) }).toString() : undefined,
      };

      const currentData: OpenBreakoutRoomsDto = {
         amount: active.amount,
         duration: initialDuration.current
            ? Duration.fromObject({ minutes: Number(initialDuration.current) }).toString()
            : undefined,
         description: active.description,
      };

      const operations = compare(currentData, newData);
      dispatch(coreHub.changeBreakoutRooms(operations));
      onClose();
   };

   return (
      <form onSubmit={handleSubmit(handleApplyForm)}>
         <DialogTitle id="form-dialog-title">Update breakout rooms</DialogTitle>
         <DialogContent>
            <DialogContentText>Here you can update the current state of the breakout rooms.</DialogContentText>
            <Box mt={4}>
               <BreakoutRoomsForm form={form} participants={null} />
            </Box>
         </DialogContent>
         <DialogActions>
            <Button color="primary" onClick={onClose}>
               Cancel
            </Button>
            <Button color="primary" disabled={!formState.isValid} type="submit">
               Update Breakout Rooms
            </Button>
         </DialogActions>
      </form>
   );
}
