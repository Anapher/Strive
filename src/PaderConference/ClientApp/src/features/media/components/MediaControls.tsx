import { Fab } from '@material-ui/core';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import React from 'react';
import { useSelector } from 'react-redux';
import { Roles } from 'src/consts';
import { selectAccessToken } from 'src/features/auth/selectors';

type Props = {
   startDesktopRecording: () => void;
};

export default function MediaControls({ startDesktopRecording }: Props) {
   const role = useSelector(selectAccessToken)?.role;

   return (
      <div>
         {role === Roles.Moderator && (
            <>
               <Fab color="primary" aria-label="share screen" onClick={startDesktopRecording}>
                  <DesktopWindowsIcon />
               </Fab>
            </>
         )}
      </div>
   );
}
