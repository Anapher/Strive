import { FormControl, Grid, InputLabel, TextField, Typography } from '@material-ui/core';
import React from 'react';
import { Controller } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import MobileAwareSelect from 'src/components/MobileAwareSelect';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import { InstructionFormProps } from '../types';

export default function TagCloudInstructionForm({
   form: {
      register,
      control,
      formState: { errors },
   },
   showAdvanced,
}: InstructionFormProps) {
   const { t } = useTranslation();

   return (
      <div>
         <Typography variant="caption">{t('conference.poll.create_dialog.tag_cloud_description')}</Typography>
         <Grid container spacing={2} style={{ marginTop: 8 }}>
            <Grid item xs={6}>
               <FormControl fullWidth>
                  <InputLabel id="poll-dialog-tag-cloud-mode-label">
                     {t('conference.poll.create_dialog.tag_cloud_mode')}
                  </InputLabel>
                  <Controller
                     render={({ field: { onChange, value } }) => (
                        <MobileAwareSelect
                           labelId="poll-dialog-tag-cloud-mode-label"
                           id="poll-dialog-tag-cloud-mode"
                           value={value}
                           onChange={onChange}
                        >
                           {[
                              {
                                 label: t('conference.poll.create_dialog.tag_cloud_mode_ignore_case'),
                                 value: 'caseInsensitive',
                              },
                              { label: t('conference.poll.create_dialog.tag_cloud_mode_ignore_typos'), value: 'fuzzy' },
                           ]}
                        </MobileAwareSelect>
                     )}
                     control={control}
                     name="instruction.mode"
                     defaultValue={'fuzzy' as any}
                     shouldUnregister
                  />
               </FormControl>
            </Grid>
            {showAdvanced && (
               <Grid item xs={6}>
                  <TextField
                     type="number"
                     label={t('conference.poll.create_dialog.max')}
                     {...wrapForInputRef(
                        register('instruction.maxTags', {
                           setValueAs: (value: string) => (value === '' ? undefined : +value),
                           valueAsNumber: true,
                           min: 1,
                           shouldUnregister: true,
                        }),
                     )}
                     helperText={t('conference.poll.create_dialog.tag_cloud_max_helper')}
                     inputProps={{ min: 1, step: 1 }}
                     error={Boolean((errors.instruction as any)?.maxTags)}
                  />
               </Grid>
            )}
         </Grid>
      </div>
   );
}
