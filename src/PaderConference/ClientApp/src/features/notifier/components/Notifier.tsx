import { useSnackbar } from 'notistack';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { removeMessage } from '../notifierSlice';

export default function Notifier() {
   const dispatch = useDispatch();
   const notifications = useSelector((state: RootState) => state.notifier.notifications);
   const { enqueueSnackbar, closeSnackbar } = useSnackbar();
   const displayed = React.useRef<string[]>([]);

   const storeDisplayed = (key: string) => {
      displayed.current = [...displayed.current, key];
   };

   const removeDisplayed = (key: string) => {
      displayed.current = displayed.current.filter((x) => x !== key);
   };

   useEffect(() => {
      notifications.forEach(({ key, message, variant, dismissed = false }) => {
         if (dismissed) {
            // dismiss snackbar using notistack
            closeSnackbar(key);
            return;
         }

         // do nothing if snackbar is already displayed
         if (displayed.current.includes(key)) return;

         // display snackbar using notistack
         enqueueSnackbar(message, {
            key,
            variant,
            onExited: (_, myKey) => {
               // remove this snackbar from redux store
               dispatch(removeMessage(myKey as string));
               removeDisplayed(myKey as string);
            },
         });

         // keep track of snackbars that we've displayed
         storeDisplayed(key);
      });
   }, [notifications, closeSnackbar, enqueueSnackbar, dispatch]);

   return null;
}
