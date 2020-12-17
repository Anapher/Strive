import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { showMessage } from './actions';
import { ShowMessageData } from './types';

export type SnackNotification = ShowMessageData & {
   key: any;
   message: string;
   dismissed?: boolean;
};

export type NotifierState = Readonly<{
   notifications: SnackNotification[];
}>;

const initialState: NotifierState = {
   notifications: [],
};

const notifierSlice = createSlice({
   name: 'notifier',
   initialState,
   reducers: {
      closeMessage(state, action: PayloadAction<string>) {
         const notification = state.notifications.find((x) => x.key === action.payload);
         if (notification) notification.dismissed = true;
      },
      removeMessage(state, action: PayloadAction<string>) {
         state.notifications = state.notifications.filter((x) => x.key !== action.payload);
      },
   },
   extraReducers: {
      [showMessage.type]: (state, action: PayloadAction<SnackNotification>) => {
         state.notifications.push(action.payload);
      },
   },
});

export const { closeMessage, removeMessage } = notifierSlice.actions;
export default notifierSlice.reducer;
