import { Portal, TextField } from '@material-ui/core';
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import PollCardSubmitButton from '../../components/PollCardSubmitButton';
import { NumericAnswer, NumericInstruction } from '../../types';
import { PollAnswerFormProps } from '../types';

const convertStringToNumber = (s: string) => {
   s = s.replace(',', '.'); // allow comma as decimal separator
   const n = Number(s);
   if (Number.isNaN(n)) return undefined;
   return n;
};

const validateNumber = (n: number | undefined, instruction: NumericInstruction) => {
   if (n === undefined) {
      return { valid: false, error: 'conference.poll.types.numeric.error_invalid_number' };
   }

   if (typeof instruction.min === 'number' && instruction.min > n) {
      return { valid: false, error: 'conference.poll.types.numeric.error_too_small' };
   }

   if (typeof instruction.max === 'number' && instruction.max < n) {
      return { valid: false, error: 'conference.poll.types.numeric.error_too_large' };
   }

   if (typeof instruction.max === 'number' && instruction.max < n) {
      return { valid: false, error: 'conference.poll.types.numeric.error_too_large' };
   }

   if (typeof instruction.step === 'number' && n % instruction.step !== 0) {
      return { valid: false, error: 'conference.poll.types.numeric.error_not_match_step' };
   }

   return { valid: true };
};

const createAnswerDto: (selected: number) => NumericAnswer = (selected) => ({
   type: 'numeric',
   selected,
});

export default function NumericAnswerForm({
   poll: { poll, answer },
   footerPortalRef,
   onSubmit,
}: PollAnswerFormProps<NumericAnswer>) {
   const { t } = useTranslation();

   if (poll.instruction.type !== 'numeric') throw new Error('Numeric instruction required');

   const [selected, setSelected] = useState<string>(
      ((answer?.answer as NumericAnswer)?.selected ?? poll.instruction.min ?? 0).toString(),
   );

   const handleChangeSelected = (event: React.ChangeEvent<HTMLInputElement>) => {
      setSelected(event.target.value);
   };

   const number = convertStringToNumber(selected);
   const numberValidation = validateNumber(number, poll.instruction);

   const canSubmitAnswer = poll.config.isAnswerFinal && Boolean(answer);

   return (
      <>
         <TextField
            value={selected}
            onChange={handleChangeSelected}
            fullWidth
            margin="dense"
            label={t('conference.poll.types.numeric.your_number')}
            variant="outlined"
            type="number"
            error={!numberValidation.valid}
            helperText={numberValidation.error ? t(numberValidation.error) : undefined}
            InputLabelProps={{
               shrink: true,
            }}
            InputProps={{
               inputProps: { min: poll.instruction.min, max: poll.instruction.max, step: poll.instruction.step },
            }}
            disabled={canSubmitAnswer}
         />
         {(!poll.config.isAnswerFinal || !answer) && (
            <Portal container={footerPortalRef}>
               <PollCardSubmitButton
                  disabled={
                     !numberValidation.valid ||
                     canSubmitAnswer ||
                     (answer?.answer.type === 'numeric' && answer.answer.selected === number)
                  }
                  onClick={() => onSubmit(createAnswerDto(number!))}
               />
            </Portal>
         )}
      </>
   );
}
