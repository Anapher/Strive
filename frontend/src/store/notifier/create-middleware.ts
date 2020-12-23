import { PayloadAction } from '@reduxjs/toolkit';
import toast from 'react-hot-toast';
import { Middleware } from 'redux';
import { showMessage, ShowMessageDto } from './actions';

type ActiveToast = {
   toastId: string;
   actionType: string;
   dependingActionTypes?: string[];
   successMessage?: string;

   updateHandler?: {
      success: boolean;
      message: string;
   };
};

const middleware: Middleware = () => {
   const activeToasts = new Map<string, ActiveToast[]>();

   const addToastHandler = (info: ActiveToast) => {
      const list = activeToasts.get(info.actionType) ?? [];
      list.push(info);

      activeToasts.set(info.actionType, list);
   };

   const removeToastHandler = (actionType: string, toastId: string) => {
      let list = activeToasts.get(actionType);
      if (list) {
         list = list.filter((x) => x.toastId !== toastId);
         if (list.length === 0) {
            activeToasts.delete(actionType);
         } else {
            activeToasts.set(actionType, list);
         }
      }
   };

   return (next) => (action: PayloadAction<any>) => {
      const { type: actionType } = action;

      if (actionType === showMessage.type) {
         const messageDto = action.payload as ShowMessageDto;
         const { message } = messageDto;

         if (messageDto.type === 'action') {
            const toastId = toast.loading(message);

            addToastHandler({
               toastId,
               actionType: messageDto.succeededOn.type,
               dependingActionTypes: [messageDto.failedOn.type],
               updateHandler: { success: true, message: messageDto.succeededOn.message },
            });
            addToastHandler({
               toastId,
               actionType: messageDto.failedOn.type,
               dependingActionTypes: [messageDto.succeededOn.type],
               updateHandler: { success: false, message: messageDto.failedOn.message },
            });
         } else {
            let toastId: string | undefined;
            switch (messageDto.type) {
               case 'success':
                  toastId = toast.success(message);
                  break;
               case 'error':
                  toastId = toast.error(message);
                  break;
               case 'info':
                  toastId = toast(message, { icon: messageDto.icon });
                  break;
               case 'loading':
                  toastId = toast.loading(message);
                  break;
               default:
                  console.error('Invalid toast message type', messageDto);
                  return;
            }

            if (messageDto.dismissOn) {
               addToastHandler({
                  toastId,
                  actionType: messageDto.dismissOn.type,
                  successMessage: (messageDto.dismissOn as any).successMessage,
               });
            }
         }

         return;
      }

      const handlers = activeToasts.get(actionType);
      if (handlers) {
         activeToasts.delete(actionType);

         for (const handler of handlers) {
            if (handler.updateHandler) {
               if (handler.updateHandler.success) {
                  toast.success(handler.updateHandler.message, { id: handler.toastId });
               } else {
                  toast.error(handler.updateHandler.message, { id: handler.toastId });
               }
            } else {
               if (handler.successMessage) {
                  if (action.payload.success === true) {
                     toast.success(handler.successMessage, { id: handler.toastId });
                  } else if (action.payload.success === false) {
                     toast.error(action.payload.error.message, { id: handler.toastId });
                  } else {
                     toast.dismiss(handler.toastId);
                  }
               } else {
                  toast.dismiss(handler.toastId);
               }
            }

            if (handler.dependingActionTypes)
               for (const dependingType of handler.dependingActionTypes) {
                  removeToastHandler(dependingType, handler.toastId);
               }
         }
      }

      return next(action);
   };
};

export default middleware;
