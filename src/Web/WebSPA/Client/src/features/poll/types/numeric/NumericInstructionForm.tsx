import { Grid, TextField, Typography } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import { InstructionFormProps } from '../types';

export default function NumericInstructionForm({ form: { register } }: InstructionFormProps) {
   const { t } = useTranslation();

   return (
      <div>
         <Typography variant="caption">{t('conference.poll.create_dialog.numeric_description')}</Typography>
         <Grid container spacing={2} style={{ marginTop: 8 }}>
            <Grid item xs={4}>
               <TextField
                  type="number"
                  label={t('conference.poll.create_dialog.min')}
                  {...wrapForInputRef(
                     register('instruction.min', {
                        valueAsNumber: true,
                        shouldUnregister: true,
                     }),
                  )}
               />
            </Grid>
            <Grid item xs={4}>
               <TextField
                  type="number"
                  label={t('conference.poll.create_dialog.max')}
                  {...wrapForInputRef(
                     register('instruction.max', {
                        valueAsNumber: true,
                        shouldUnregister: true,
                     }),
                  )}
               />
            </Grid>
            <Grid item xs={4}>
               <TextField
                  type="number"
                  label={t('conference.poll.create_dialog.step')}
                  {...wrapForInputRef(
                     register('instruction.step', {
                        valueAsNumber: true,
                        shouldUnregister: true,
                     }),
                  )}
               />
            </Grid>
         </Grid>
      </div>
   );
}
