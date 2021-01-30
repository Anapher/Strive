import { Box, Button } from '@material-ui/core';
import React from 'react';
import BaseAuthComponent from './BaseAuthComponent';

export default function NotAuthenticated() {
   return (
      <BaseAuthComponent title="Authentication failed" text="You are not authenticated">
         <Box mt={2}>
            <Button href="/" variant="contained">
               Back to start
            </Button>
         </Box>
      </BaseAuthComponent>
   );
}
