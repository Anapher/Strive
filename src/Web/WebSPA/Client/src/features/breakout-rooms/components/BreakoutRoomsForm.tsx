import { Box, TextField } from '@material-ui/core';
import React from 'react';
import { Controller, UseFormMethods } from 'react-hook-form';
import { OpenBreakoutRoomsDto } from 'src/core-hub.types';
import { Participant } from 'src/features/conference/types';
import OpenBreakoutRoomsAssignments from './BreakoutRoomsAssignments';

const validateNumberInteger = (val: unknown) => {
   const x = Number(val);
   if (Number.isInteger(x)) return true;

   return 'The amount must be an integer.';
};

type Props = {
   form: UseFormMethods<OpenBreakoutRoomsDto>;
   participants: Participant[] | null;
};

export default function BreakoutRoomsForm({ form: { register, errors, watch, control }, participants }: Props) {
   const amount = watch('amount');

   return (
      <div>
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
                  name="deadline"
                  type="number"
                  error={!!errors.deadline}
                  style={{ maxWidth: 160, marginLeft: 16 }}
                  helperText={errors.deadline?.message}
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
         {participants && (
            <Box mt={3} height={300}>
               <Controller
                  control={control}
                  name="assignedRooms"
                  render={({ value, onChange }) => (
                     <OpenBreakoutRoomsAssignments
                        data={value ?? []}
                        participants={participants}
                        createdRooms={amount}
                        onChange={(data) => onChange(data)}
                     />
                  )}
               />
            </Box>
         )}
      </div>
   );
}
