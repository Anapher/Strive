import { Chip, Grid, makeStyles, Portal } from '@material-ui/core';
import React, { useState } from 'react';
import PollCardSubmitButton from '../../components/PollCardSubmitButton';
import { MultipleChoiceAnswer } from '../../types';
import { PollAnswerFormProps } from '../types';

type SelectionChipProps = {
   className: string;
   option: string;
   applied: boolean;
   selected: boolean;
   disabled: boolean;

   onClick: () => void;
};

function SelectionChip({ className, option, selected, applied, disabled, onClick }: SelectionChipProps) {
   return (
      <Chip
         className={className}
         label={option}
         color={selected || applied ? 'primary' : undefined}
         disabled={disabled}
         clickable
         variant={selected && !applied ? 'outlined' : undefined}
         onClick={onClick}
         component="button"
         type="submit"
      />
   );
}

const useStyles = makeStyles({
   chip: {
      minWidth: 56,
   },
});

const createAnswerDto: (selected: string[]) => MultipleChoiceAnswer = (selected) => ({
   type: 'multipleChoice',
   selected,
});

export default function MultipleChoiceAnswerForm({
   onSubmit,
   onDelete,
   poll: { poll, answer },
   footerPortalRef,
}: PollAnswerFormProps<MultipleChoiceAnswer>) {
   if (poll.instruction.type !== 'multipleChoice') throw new Error('Multiple choice instruction required');

   const classes = useStyles();
   const [selected, setSelected] = useState(new Array<string>());

   const selectedMax = poll.instruction.maxSelections
      ? !!selected && selected.length >= poll.instruction.maxSelections
      : false;

   const handleSelectOption = (option: string) => {
      setSelected(
         selected.includes(option) ? selected.filter((x) => x !== option) : [...(selected || []), option].sort(),
      );

      if (!poll.config.isAnswerFinal) {
         if (selected.length === 0) {
            onDelete();
         } else {
            onSubmit(createAnswerDto(selected));
         }
      }
   };

   return (
      <>
         <Grid container spacing={1} justify="center">
            {poll.instruction.options.map((x) => (
               <Grid item key={x}>
                  <SelectionChip
                     option={x}
                     className={classes.chip}
                     applied={answer?.answer.type === 'multipleChoice' && answer.answer.selected.includes(x)}
                     selected={poll.config.isAnswerFinal && Boolean(selected?.includes(x))}
                     disabled={Boolean(answer && poll.config.isAnswerFinal) || (selectedMax && !selected.includes(x))}
                     onClick={() => handleSelectOption(x)}
                  />
               </Grid>
            ))}
         </Grid>
         {poll.config.isAnswerFinal && !answer && (
            <Portal container={footerPortalRef}>
               <PollCardSubmitButton
                  disabled={selected.length === 0}
                  onClick={() => onSubmit(createAnswerDto(selected))}
               />
            </Portal>
         )}
      </>
   );
}
