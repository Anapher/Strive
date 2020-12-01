import {
   Box,
   Button,
   Dialog,
   DialogActions,
   DialogContent,
   DialogContentText,
   DialogTitle,
   TextField,
} from '@material-ui/core';
import { Duration } from 'luxon';
import React from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { OpenBreakoutRoomsDto } from 'src/core-hub.types';
import { RootState } from 'src/store';
import { setCreationDialogOpen } from '../breakoutRoomsSlice';
import OpenBreakoutRoomsAssignments from './OpenBreakoutRoomsAssignments';

const validateNumberInteger = (val: unknown) => {
   const x = Number(val);
   if (Number.isInteger(x)) return true;

   return 'The amount must be an integer.';
};

export default function OpenBreakoutRoomsDialog() {
   const dispatch = useDispatch();
   const participants = useSelector((state: RootState) => state.conference.participants);
   const open = useSelector((state: RootState) => state.breakoutRooms.creationDialogOpen);

   const handleClose = () => dispatch(setCreationDialogOpen(false));

   const { register, errors, formState, handleSubmit, watch, control } = useForm<OpenBreakoutRoomsDto>({
      defaultValues: {
         amount: participants != undefined ? Math.ceil(participants.length / 3) : 4,
         description: '',
         duration: '15',
         assignedRooms: [],
      },
      mode: 'onChange',
   });

   const amount = watch('amount');

   const handleApplyForm = (dto: OpenBreakoutRoomsDto) => {
      const action = coreHub.openBreakoutRooms({
         ...dto,
         amount: Number(dto.amount),
         duration: dto.duration ? Duration.fromObject({ minutes: Number(dto.duration) }).toString() : undefined,
      });

      dispatch(action);
      handleClose();
   };

   // participants = [
   //    ...participants,
   //    ...Array.from({ length: 15 }).map((_, i) => ({
   //       participantId: 'test-' + i,
   //       displayName: 'random ' + i,
   //       role: 'mod',
   //    })),
   // ];

   return (
      <Dialog
         open={open}
         onClose={handleClose}
         aria-labelledby="form-dialog-title"
         fullWidth
         maxWidth="md"
         scroll="paper"
      >
         <form onSubmit={handleSubmit(handleApplyForm)}>
            <DialogTitle id="form-dialog-title">Create breakout rooms</DialogTitle>
            <DialogContent>
               <DialogContentText>
                  Create breakout rooms to let participants work together in smaller groups. Here you can define the
                  amount of rooms to create. You can assign a duration for the breakout phase, it will be displayed to
                  all participants. After creation the participants can join them on their own.
               </DialogContentText>
               <Box mt={4}>
                  <Box display="flex">
                     <TextField
                        required
                        label="Amount"
                        inputRef={register({
                           min: { value: 1, message: 'Must at least create one breakout room.' },
                           required: true,
                           validate: validateNumberInteger,
                        })}
                        autoFocus
                        inputProps={{ min: 1, step: 1 }}
                        name="amount"
                        type="number"
                        error={!!errors.amount}
                        style={{ maxWidth: 80 }}
                        helperText={errors.amount?.message}
                        fullWidth
                     />
                     <TextField
                        label="Duration in minutes"
                        inputRef={register({
                           min: { value: 1, message: 'Must at least give one minute.' },
                           validate: validateNumberInteger,
                        })}
                        inputProps={{ min: 1, step: 1 }}
                        name="duration"
                        type="number"
                        error={!!errors.duration}
                        style={{ maxWidth: 160, marginLeft: 16 }}
                        helperText={errors.duration?.message}
                        fullWidth
                     />
                     <TextField
                        label="Description"
                        name="description"
                        inputRef={register}
                        placeholder="Optionally define the task the participants should do."
                        fullWidth
                        style={{ marginLeft: 16 }}
                     />
                  </Box>
               </Box>
               <Box mt={3} height={300}>
                  <Controller
                     control={control}
                     name="assignedRooms"
                     render={({ value, onChange }) => (
                        <OpenBreakoutRoomsAssignments
                           data={value ?? []}
                           participants={participants ?? []}
                           createdRooms={amount}
                           onChange={(data) => onChange(data)}
                        />
                     )}
                  />
               </Box>
            </DialogContent>
            <DialogActions>
               <Button color="primary" onClick={handleClose}>
                  Cancel
               </Button>
               <Button color="primary" disabled={!formState.isValid} type="submit">
                  Open Breakout Rooms
               </Button>
            </DialogActions>
         </form>
      </Dialog>
   );
}
