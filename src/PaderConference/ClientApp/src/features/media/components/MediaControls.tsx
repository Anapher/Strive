import { Fab } from '@material-ui/core';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import { createSelector } from '@reduxjs/toolkit';
import React from 'react';
import { useSelector } from 'react-redux';
import { Roles } from 'src/consts';
import { RootState } from 'src/store';
import { parseJwt } from 'src/utils/token-helpers';
import AccessibilityIcon from '@material-ui/icons/Accessibility';

const selectUserRole = createSelector(
   (state: RootState) => state.auth.token?.accessToken,
   (token) => (token ? parseJwt(token).role : undefined),
);

type Props = {
   startDesktopRecording: () => void;
   getScreen: () => void;
};

export default function MediaControls({ startDesktopRecording, getScreen }: Props) {
   const role = useSelector(selectUserRole);

   return (
      <div>
         {role === Roles.Moderator && (
            <>
               <Fab color="primary" aria-label="share screen" onClick={startDesktopRecording}>
                  <DesktopWindowsIcon />
               </Fab>
            </>
         )}
         <Fab color="primary" aria-label="share screen" onClick={getScreen}>
            <AccessibilityIcon />
         </Fab>
      </div>
   );
}
