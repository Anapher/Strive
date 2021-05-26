import React, { useState } from 'react';
import TroubleshootConnection from './TroubleshootConnection';
import TroubleshootDiagnostics from './TroubleshootDiagnostics';
import TroubleshootLogging from './TroubleshootLogging';
import TroubleshootMicrophone from './TroubleshootMicrophone';
import TroubleshootSpeakers from './TroubleshootSpeakers';

export default function Troubleshooting() {
   const [expanded, setExpanded] = useState<string | null>(null);

   const handleChangeExpanded = (name: string) => () => {
      if (expanded === name) setExpanded(null);
      else setExpanded(name);
   };

   return (
      <div>
         <TroubleshootConnection expanded={expanded === 'connection'} onChange={handleChangeExpanded('connection')} />
         <TroubleshootSpeakers expanded={expanded === 'speakers'} onChange={handleChangeExpanded('speakers')} />
         <TroubleshootMicrophone expanded={expanded === 'mic'} onChange={handleChangeExpanded('mic')} />
         <TroubleshootDiagnostics
            expanded={expanded === 'diagnostics'}
            onChange={handleChangeExpanded('diagnostics')}
         />
         <TroubleshootLogging expanded={expanded === 'logging'} onChange={handleChangeExpanded('logging')} />
      </div>
   );
}
