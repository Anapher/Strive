import {
   Box,
   Button,
   Dialog,
   DialogActions,
   DialogContent,
   DialogContentText,
   DialogTitle,
   Fade,
   Input,
   Typography,
} from '@material-ui/core';
import _ from 'lodash';
import React from 'react';
import { useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { selectRoomViewModels } from '../selectors';
import * as coreHub from 'src/core-hub';

const NATO_ALPHA = [
   'Alfa',
   'Bravo',
   'Charlie',
   'Delta',
   'Echo',
   'Foxtrot',
   'Golf',
   'Hotel',
   'India',
   'Juliett',
   'Kilo',
   'Lima',
   'Mika',
   'November',
   'Oscar',
   'Papa',
   'Quebec',
   'Romeo',
   'Sierra',
   'Tango',
   'Uniform',
   'Victor',
   'Whiskey',
   'Xray',
   'Yankee',
   'Zulu',
];

type CreateBreakoutRoomsForm = {
   amount: number;
};

type Props = {
   open: boolean;
   onClose: () => void;
};

const validateNumberInteger = (val: unknown) => {
   const x = Number(val);
   if (Number.isInteger(x)) return true;

   return 'The amount must be an integer.';
};

const getBreakoutRoomName = (i: number) => NATO_ALPHA[i % 26] + (i > 25 ? ` #${Math.floor(i / 26)}` : '');

export default function BreakoutRoomDialog({ open, onClose }: Props) {
   const dispatch = useDispatch();

   const participants = useSelector((state: RootState) => state.conference.participants);
   const rooms = useSelector(selectRoomViewModels) ?? [];

   const hasBreakoutRooms = rooms.length > 1;
   const openedBreakoutRoomsLen = Math.max(rooms.length - 1, 0);

   const { register, errors, formState, handleSubmit } = useForm<CreateBreakoutRoomsForm>({
      defaultValues: {
         amount: hasBreakoutRooms
            ? openedBreakoutRoomsLen
            : participants != undefined
            ? Math.ceil(participants.length / 3)
            : 4,
      },
      mode: 'onChange',
   });

   const handleCreateRooms = (amount: number) => {
      const names = new Array<string>();
      let nameIndex = 0;

      for (let i = 0; i < amount; i++) {
         let name = getBreakoutRoomName(nameIndex++);
         while (rooms?.find((x) => x.displayName === name)) {
            name = getBreakoutRoomName(nameIndex++);
         }

         names.push(name);
      }

      dispatch(coreHub.createRooms(names.map((x) => ({ displayName: x }))));
   };

   const handleRemoveRooms = (amount: number) => {
      const targetRooms = _(rooms)
         .filter((x) => !x.isDefaultRoom) // don't remove default room
         .orderBy([(x) => x.participants.length, (x) => x.displayName], ['asc', 'desc']) // delete rooms with no participants first
         .take(amount)
         .map((x) => x.roomId)
         .value();

      dispatch(coreHub.removeRooms(targetRooms));
   };

   const handleApplyForm = ({ amount }: CreateBreakoutRoomsForm) => {
      if (amount > openedBreakoutRoomsLen) {
         handleCreateRooms(amount - openedBreakoutRoomsLen);
      } else if (amount < openedBreakoutRoomsLen) {
         handleRemoveRooms(openedBreakoutRoomsLen - amount);
      }

      onClose();
   };

   return (
      <Dialog open={open} onClose={onClose} aria-labelledby="form-dialog-title" fullWidth maxWidth="sm">
         <form onSubmit={handleSubmit(handleApplyForm)}>
            <DialogTitle id="form-dialog-title">Create breakout rooms</DialogTitle>
            <DialogContent>
               <DialogContentText>
                  Create breakout rooms to let participants work together in smaller groups. Here you can define the
                  amount of rooms to create. After creation the participants can join them on their own.
               </DialogContentText>
               {hasBreakoutRooms && (
                  <DialogContentText>
                     You already have {openedBreakoutRoomsLen} breakout rooms opened. The breakout rooms will be
                     adjusted to the new amount.
                  </DialogContentText>
               )}
               <Box display="flex" flexDirection="row" alignItems="center" mt={4}>
                  <Typography style={{ marginRight: 16 }}>Amount:</Typography>
                  <Input
                     inputRef={register({
                        min: { value: 1, message: 'Must at least create one breakout room.' },
                        required: true,
                        validate: validateNumberInteger,
                     })}
                     autoFocus
                     inputProps={{ min: 1, step: 1 }}
                     margin="dense"
                     name="amount"
                     type="number"
                     error={!!errors.amount}
                     style={{ maxWidth: 80 }}
                     fullWidth
                  />
                  <Fade in={!!errors.amount} style={{ marginLeft: 16 }}>
                     <Typography color="error" variant="caption">
                        {errors.amount?.message}
                     </Typography>
                  </Fade>
               </Box>
            </DialogContent>
            <DialogActions>
               <Button color="primary" onClick={onClose}>
                  Cancel
               </Button>
               <Button color="primary" disabled={!formState.isValid} type="submit">
                  {hasBreakoutRooms ? 'Adjust Breakout Rooms' : 'Open Breakout Rooms'}
               </Button>
            </DialogActions>
         </form>
      </Dialog>
   );
}
