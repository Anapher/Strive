import React from 'react';
import NewWindow from 'react-new-window';
import { useDispatch, useSelector } from 'react-redux';
import { selectAccessToken } from 'src/features/auth/selectors';
import { RootState } from 'src/store';
import { setOpen } from '../reducer';
import DiagnosticsView from './DiagnosticsView';

export default function DiagnosticsWindow() {
   const open = useSelector((state: RootState) => state.diagnostics.open);
   const token = useSelector(selectAccessToken);

   const dispatch = useDispatch();

   const handleUnload = () => {
      dispatch(setOpen(false));
   };

   return (
      <>
         {open && (
            <NewWindow
               center="screen"
               title={`Diagnostics of PaderConference (${token?.unique_name})`}
               onUnload={handleUnload}
               features={{ width: 1000, height: 500 }}
            >
               <DiagnosticsView />
            </NewWindow>
         )}
      </>
   );
}
