import { createAction } from '@reduxjs/toolkit';

export const showMessage = createAction<ShowMessageDto>('notifier/showMessage');

export type ShowMessageDto = ShowMessageFireAndForget | ShowMessageInfo | ShowMessageAction;

export type ShowMessageFireAndForget = {
   type: 'success' | 'error';
   message: string;

   /** dismiss the notification if the action with the type is dispatched */
   dismissOn?: ActionEventHandler;
};

export type ShowMessageInfo = {
   type: 'info';
   message: string;

   /** dismiss the notification if the action with the type is dispatched */
   dismissOn?: ActionEventHandler;

   /** the icon of the toast, you may use emojis */
   icon?: string;
};

export type ShowMessageAction = {
   type: 'action';
   message: string;
   succeededOn: ActionEventUpdateHandler;
   failedOn: ActionEventUpdateHandler;
};

export type ActionEventHandler = {
   /** the action type */
   type: string;
};

export type ActionEventUpdateHandler = ActionEventHandler & {
   /** the message that should be shown */
   message: string;
};
