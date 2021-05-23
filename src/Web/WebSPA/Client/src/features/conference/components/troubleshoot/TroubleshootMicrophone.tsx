import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import useDeviceManagement from 'src/features/media/useDeviceManagement';
import { RootState } from 'src/store';
import useMicrophone from 'src/store/webrtc/hooks/useMicrophone';
import StatusChip from './StatusChip';
import ActiveMicrophoneChip from './troubleshoot-microphone/ActiveMicrophoneChip';
import DetailedStatus from './troubleshoot-microphone/DetailedStatus';
import TroubleshootAccordion from './TroubleshootAccordion';

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
};

export default function TroubleshootMicrophone({ expanded, onChange }: Props) {
   const { t } = useTranslation();
   const gain = useSelector((state: RootState) => state.settings.obj.mic.audioGain);

   const localMic = useMicrophone(gain, true);
   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device);
   const micController = useDeviceManagement('loopback-mic', localMic, audioDevice);

   const [error, setError] = useState<string | null>(null);

   const handleEnableMic = async () => {
      try {
         await micController.enable();
      } catch (error) {
         console.log(error);

         if (error && error.toString().startsWith('NotAllowedError')) {
            setError(t('conference.settings.audio.permission_denied'));
         } else {
            setError(error?.message ?? error?.toString() ?? 'Unknown error');
         }
      }
   };

   useEffect(() => {
      if (!micController.enabled) {
         // the mic is automatically disabled on component unmount
         handleEnableMic();
      }
   }, [micController.enabled, audioDevice]);

   return (
      <TroubleshootAccordion
         title={t('common:Microphone')}
         expanded={expanded}
         onChange={onChange}
         renderStatus={() =>
            error ? <StatusChip status="error" label={error} size="small" /> : <ActiveMicrophoneChip />
         }
      >
         <DetailedStatus enableError={error} />
      </TroubleshootAccordion>
   );
}
