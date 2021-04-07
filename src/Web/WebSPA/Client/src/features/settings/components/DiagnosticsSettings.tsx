import { Box, Checkbox, FormControlLabel, makeStyles, Switch, Typography } from '@material-ui/core';
import debug from 'debug';
import _ from 'lodash';
import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { setEnableVideoOverlay } from '../reducer';
import { selectEnableVideoOverlay } from '../selectors';

const useStyles = makeStyles((theme) => ({
   root: {
      margin: theme.spacing(3),
   },
}));

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

export default function DiagnosticsSettings() {
   const classes = useStyles();
   const dispatch = useDispatch();

   const [currentFilter, setCurrentFilter] = useState<string | null>();
   useEffect(() => {
      setCurrentFilter(localStorage.getItem('debug'));
   }, []);

   const enableVideoOverlay = useSelector(selectEnableVideoOverlay);
   const handleSetEnableVideoOverlay = (newState: boolean) => dispatch(setEnableVideoOverlay(newState));

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
      <div className={classes.root}>
         <Typography variant="h6">Logging</Typography>
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
         <Typography variant="caption">
            Please note: you may have to refresh this page in order to apply the changes
         </Typography>

         <Box mt={2}>
            <FormControlLabel
               control={
                  <Switch
                     checked={enableVideoOverlay}
                     onChange={(_, checked) => handleSetEnableVideoOverlay(checked)}
                     color="primary"
                  />
               }
               label="Enable video overlay"
            />
         </Box>
      </div>
   );
}
