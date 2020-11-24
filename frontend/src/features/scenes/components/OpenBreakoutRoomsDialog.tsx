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
import React from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { OpenBreakoutRoomsDto } from 'src/core-hub.types';
import { RootState } from 'src/store';
import OpenBreakoutRoomsAssignments from './OpenBreakoutRoomsAssignments';

const validateNumberInteger = (val: unknown) => {
   const x = Number(val);
   if (Number.isInteger(x)) return true;

   return 'The amount must be an integer.';
};

type Props = {
   open: boolean;
   onClose: () => void;
};

export default function OpenBreakoutRoomsDialog({ open, onClose }: Props) {
   const dispatch = useDispatch();
   const participants = useSelector((state: RootState) => state.conference.participants);

   const { register, errors, formState, handleSubmit, watch, control } = useForm<OpenBreakoutRoomsDto>({
      defaultValues: {
         amount: participants != undefined ? Math.ceil(participants.length / 3) : 4,
         description: '',
         assignedRooms: [],
      },
      mode: 'onChange',
   });

   const amount = watch('amount');

   const handleApplyForm = (dto: OpenBreakoutRoomsDto) => {
      dispatch(coreHub.openBreakoutRooms(dto));
      onClose();
   };

   return (
      <Dialog open={open} onClose={onClose} aria-labelledby="form-dialog-title" fullWidth maxWidth="md" scroll="paper">
         <form onSubmit={handleSubmit(handleApplyForm)}>
            <DialogTitle id="form-dialog-title">Create breakout rooms</DialogTitle>
            <DialogContent>
               <DialogContentText>
                  Create breakout rooms to let participants work together in smaller groups. Here you can define the
                  amount of rooms to create. After creation the participants can join them on their own.
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
                  ></Controller>
               </Box>
            </DialogContent>
            <DialogActions>
               <Button color="primary" onClick={onClose}>
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
