import { Checkbox, FormControlLabel, Typography } from '@material-ui/core';
import debug from 'debug';
import _ from 'lodash';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import TroubleshootAccordion from './TroubleshootAccordion';

type DebugFilterDescription = {
   label: string;
   namespace: string;
};

const debugFilters: DebugFilterDescription[] = [
   {
      label: 'WebRTC Connection',
      namespace: 'webrtc:*',
   },
   { label: 'Mediasoup', namespace: 'mediasoup-client:*' },
   { label: 'SignalR Library', namespace: 'signalr:library' },
   { label: 'SignalR Middleware', namespace: 'signalr:middleware' },
];

function filterIncludes(filter: string | null | undefined, namespace: string) {
   if (!filter) return false;
   return filter.split(',').includes(namespace);
}

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
   className?: string;
};

export default function TroubleshootLogging({ expanded, onChange, className }: Props) {
   const { t } = useTranslation();
   const [currentFilter, setCurrentFilter] = useState<string | null>();

   useEffect(() => {
      setCurrentFilter(localStorage.getItem('debug'));
   }, []);

   const handleSetNamespaceEnabled = (namespace: string, enabled: boolean) => {
      const currentNamespaces = currentFilter?.split(',') ?? [];
      if (enabled) currentNamespaces.push(namespace);
      else _.remove(currentNamespaces, (x) => x === namespace);

      const newNamespaces = currentNamespaces.join(',');
      setCurrentFilter(newNamespaces);
      localStorage.setItem('debug', newNamespaces);
      debug.enable(newNamespaces);
   };

   return (
      <TroubleshootAccordion
         className={className}
         title={t('conference.troubleshooting.logging.title')}
         expanded={expanded}
         onChange={onChange}
      >
         <div>
            <div>
               {debugFilters.map((x) => (
                  <FormControlLabel
                     key={x.namespace}
                     control={
                        <Checkbox
                           checked={filterIncludes(currentFilter, x.namespace)}
                           onChange={(_, checked) => handleSetNamespaceEnabled(x.namespace, checked)}
                        />
                     }
                     label={x.label}
                  />
               ))}
            </div>
            <Typography variant="caption">{t('conference.troubleshooting.logging.refresh_notice')}</Typography>
         </div>
      </TroubleshootAccordion>
   );
}
