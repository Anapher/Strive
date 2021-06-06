import { Box, Collapse, Input } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import SingleChoiceInstructionForm from '../single-choice/SingleChoiceInstructionForm';
import { InstructionFormProps } from '../types';

export default function MultipleChoiceInstructionForm(props: InstructionFormProps) {
   const { t } = useTranslation();
   const {
      form: {
         register,
         formState: { errors },
      },
      showAdvanced,
   } = props;

   return (
      <>
         <SingleChoiceInstructionForm {...props} />
         <Collapse in={showAdvanced}>
            <Box mt={1}>
               {t('conference.poll.create_dialog.multiple_choice_limit_choices') + ' '}
               <Input
                  {...wrapForInputRef(
                     register('instruction.maxSelections', {
                        min: 1,
                        valueAsNumber: true,
                        shouldUnregister: true,
                     }),
                  )}
                  inputProps={{ min: 1, step: 1 }}
                  type="number"
                  error={Boolean((errors.instruction as any)?.maxSelections)}
                  style={{ maxWidth: 100, marginLeft: 8 }}
                  placeholder={t('conference.poll.create_dialog.no_limit')}
                  fullWidth
               />
            </Box>
         </Collapse>
      </>
   );
}
