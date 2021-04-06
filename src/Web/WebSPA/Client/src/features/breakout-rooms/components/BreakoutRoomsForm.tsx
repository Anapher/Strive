import { Box, TextField } from '@material-ui/core';
import React from 'react';
import { Controller, UseFormReturn } from 'react-hook-form';
import { OpenBreakoutRoomsDto } from 'src/core-hub.types';
import { Participant } from 'src/features/conference/types';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import OpenBreakoutRoomsAssignments from './BreakoutRoomsAssignments';

const validateNumberInteger = (val: unknown) => {
   const x = Number(val);
   if (Number.isInteger(x)) return true;

   return 'The amount must be an integer.';
};

type Props = {
   form: UseFormReturn<OpenBreakoutRoomsDto>;
   participants: Participant[] | null;
};

export default function BreakoutRoomsForm({
   form: {
      register,
      formState: { errors },
      watch,
      control,
   },
   participants,
}: Props) {
   const amount = watch('amount');

   return (
      <div>
         <Box mt={4}>
            <Box display="flex">
               <TextField
                  {...wrapForInputRef(
                     register('amount', {
                        min: { value: 1, message: 'Must at least create one breakout room.' },
                        required: true,
                        validate: validateNumberInteger,
                     }),
                  )}
                  required
                  label="Amount"
                  autoFocus
                  inputProps={{ min: 1, step: 1 }}
                  type="number"
                  error={!!errors.amount}
                  style={{ maxWidth: 80 }}
                  helperText={errors.amount?.message}
                  fullWidth
               />
               <TextField
                  {...wrapForInputRef(
                     register('deadline', {
                        min: { value: 1, message: 'Must at least give one minute.' },
                        validate: validateNumberInteger,
                     }),
                  )}
                  label="Duration in minutes"
                  inputProps={{ min: 1, step: 1 }}
                  type="number"
                  error={!!errors.deadline}
                  style={{ maxWidth: 160, marginLeft: 16 }}
                  helperText={errors.deadline?.message}
                  fullWidth
               />
               <TextField
                  label="Description"
                  {...wrapForInputRef(register('description'))}
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
                  render={({ field: { value, onChange } }) => (
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
