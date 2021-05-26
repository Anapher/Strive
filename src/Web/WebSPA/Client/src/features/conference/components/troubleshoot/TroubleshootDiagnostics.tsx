import { Box, Button, FormControlLabel, Switch } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { setOpen } from 'src/features/diagnostics/reducer';
import { setEnableVideoOverlay } from 'src/features/settings/reducer';
import { selectEnableVideoOverlay } from 'src/features/settings/selectors';
import { RootState } from 'src/store';
import TroubleshootAccordion from './TroubleshootAccordion';

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
};

export default function TroubleshootDiagnostics({ expanded, onChange }: Props) {
   const { t } = useTranslation();
   const dispatch = useDispatch();

   const enableVideoOverlay = useSelector(selectEnableVideoOverlay);
   const handleSetEnableVideoOverlay = (newState: boolean) => dispatch(setEnableVideoOverlay(newState));

   const diagnosticsOpen = useSelector((state: RootState) => state.diagnostics.open);
   const handleShowDiagnostics = () => {
      dispatch(setOpen(true));
   };

   return (
      <TroubleshootAccordion
         title={t('conference.troubleshooting.diagnostics.title')}
         expanded={expanded}
         onChange={onChange}
      >
         <div>
            <FormControlLabel
               control={
                  <Switch
                     checked={enableVideoOverlay}
                     onChange={(_, checked) => handleSetEnableVideoOverlay(checked)}
                     color="primary"
                  />
               }
               label={t('conference.troubleshooting.diagnostics.enable_video_overlay')}
            />
            <Box mt={2}>
               <Button onClick={handleShowDiagnostics} disabled={diagnosticsOpen} variant="contained" color="secondary">
                  {t('conference.troubleshooting.diagnostics.open_media_diagnostics')}
               </Button>
            </Box>
         </div>
      </TroubleshootAccordion>
   );
}
