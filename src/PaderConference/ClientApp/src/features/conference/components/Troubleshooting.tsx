import React, { useState } from 'react';
import TroubleshootConnection from './TroubleshootConnection';
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
      </div>
   );
}
