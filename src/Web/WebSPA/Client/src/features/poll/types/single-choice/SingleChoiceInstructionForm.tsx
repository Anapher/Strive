import { TextField } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import { InstructionFormProps } from '../types';

export default function SingleChoiceInstructionForm({
   form: {
      register,
      formState: { errors },
      watch,
   },
}: InstructionFormProps) {
   const { t } = useTranslation();

   const validateOptionsText = (s: string) => {
      if (!s) return false;
      return s.split(/\r?\n/).filter((x) => x.length > 0).length > 1;
   };

   const options = watch('instruction.options');

   return (
      <TextField
         label={t('conference.poll.create_dialog.options_label')}
         fullWidth
         {...wrapForInputRef(register('instruction.options', { validate: validateOptionsText }))}
         rows={4}
         InputLabelProps={{ shrink: Boolean(options) }}
         multiline
         error={Boolean((errors.instruction as any)?.options)}
         helperText={
            (errors.instruction as any)?.options
               ? t('conference.poll.create_dialog.options_error_at_least_two')
               : t('conference.poll.create_dialog.options_helper_text')
         }
      />
   );
}
