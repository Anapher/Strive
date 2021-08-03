import { fade, makeStyles, Paper } from '@material-ui/core';
import React, { useState } from 'react';
import TroubleshootConnection from './TroubleshootConnection';
import TroubleshootDiagnostics from './TroubleshootDiagnostics';
import TroubleshootLogging from './TroubleshootLogging';
import TroubleshootMicrophone from './TroubleshootMicrophone';
import TroubleshootSpeakers from './TroubleshootSpeakers';

const useStyles = makeStyles((theme) => ({
   root: {
      backgroundColor: fade(theme.palette.text.primary, 0.04),
   },
}));

export default function Troubleshooting() {
   const classes = useStyles();
   const [expanded, setExpanded] = useState<string | null>(null);

   const handleChangeExpanded = (name: string) => () => {
      if (expanded === name) setExpanded(null);
      else setExpanded(name);
   };

   return (
      <Paper elevation={1}>
         <TroubleshootConnection
            className={classes.root}
            expanded={expanded === 'connection'}
            onChange={handleChangeExpanded('connection')}
         />
         <TroubleshootSpeakers
            className={classes.root}
            expanded={expanded === 'speakers'}
            onChange={handleChangeExpanded('speakers')}
         />
         <TroubleshootMicrophone
            className={classes.root}
            expanded={expanded === 'mic'}
            onChange={handleChangeExpanded('mic')}
         />
         <TroubleshootDiagnostics
            className={classes.root}
            expanded={expanded === 'diagnostics'}
            onChange={handleChangeExpanded('diagnostics')}
         />
         <TroubleshootLogging
            className={classes.root}
            expanded={expanded === 'logging'}
            onChange={handleChangeExpanded('logging')}
         />
      </Paper>
   );
}
