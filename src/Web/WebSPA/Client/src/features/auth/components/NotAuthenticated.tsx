import { Box, Button } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import BaseAuthComponent from './BaseAuthComponent';

export default function NotAuthenticated() {
   const { t } = useTranslation();

   return (
      <BaseAuthComponent componentName="NotAuthenticated">
         <Box mt={2}>
            <Button href="/" variant="contained">
               {t('common:back_to_start')}
            </Button>
         </Box>
      </BaseAuthComponent>
   );
}
