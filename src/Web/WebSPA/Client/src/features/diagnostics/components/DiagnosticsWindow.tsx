import { useReactOidc } from '@axa-fr/react-oidc-context';
import React from 'react';
import { useTranslation } from 'react-i18next';
import NewWindow from 'react-new-window';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { setOpen } from '../reducer';
import DiagnosticsView from './DiagnosticsView';

export default function DiagnosticsWindow() {
   const dispatch = useDispatch();
   const { t } = useTranslation();
   const open = useSelector((state: RootState) => state.diagnostics.open);
   const { oidcUser } = useReactOidc();

   const handleUnload = () => {
      dispatch(setOpen(false));
   };

   return (
      <>
         {open && (
            <NewWindow
               center="screen"
               title={t('conference.diagnostics.title', { name: oidcUser.profile.name })}
               onUnload={handleUnload}
               features={{ width: 1000, height: 500 }}
            >
               <DiagnosticsView />
            </NewWindow>
         )}
      </>
   );
}
