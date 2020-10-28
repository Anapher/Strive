import { createAction } from '@reduxjs/toolkit';
import { ShowMessageData } from './types';

export const showMessage = createAction('notifier/showMessage', (data: ShowMessageData) => ({
   payload: { ...data, key: new Date().getTime() + Math.random() },
}));
