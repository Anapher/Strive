import { Box, Button } from '@material-ui/core';
import React from 'react';
import BaseAuthComponent from './BaseAuthComponent';

export default function SessionLostComponent() {
   return (
      <BaseAuthComponent title="Session Lost" text="You are not authenticated">
         <Box mt={2}>
            <Button href="/" variant="contained">
               Back to start
            </Button>
         </Box>
      </BaseAuthComponent>
   );
}
