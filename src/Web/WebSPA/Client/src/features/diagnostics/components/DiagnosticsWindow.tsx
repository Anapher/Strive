import { useReactOidc } from '@axa-fr/react-oidc-context';
import React from 'react';
import NewWindow from 'react-new-window';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { setOpen } from '../reducer';
import DiagnosticsView from './DiagnosticsView';

export default function DiagnosticsWindow() {
   const open = useSelector((state: RootState) => state.diagnostics.open);
   const { oidcUser } = useReactOidc();

   const dispatch = useDispatch();

   const handleUnload = () => {
      dispatch(setOpen(false));
   };

   return (
      <>
         {open && (
            <NewWindow
               center="screen"
               title={`Diagnostics of Strive (${oidcUser.profile.name})`}
               onUnload={handleUnload}
               features={{ width: 1000, height: 500 }}
            >
               <DiagnosticsView />
            </NewWindow>
         )}
      </>
   );
}
