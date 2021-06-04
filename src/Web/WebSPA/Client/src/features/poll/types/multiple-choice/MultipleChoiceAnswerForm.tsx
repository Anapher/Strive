import { Chip, Grid, makeStyles } from '@material-ui/core';
import React from 'react';
import { MultipleChoiceAnswer } from '../../types';
import { PollAnswerFormProps } from '../types';
import clsx from 'classnames';

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
         color={selected ? 'primary' : undefined}
         disabled={disabled}
         clickable
         variant={selected && !applied ? 'outlined' : undefined}
         onClick={onClick}
         component="button"
         type="submit"
      />
   );
}

const useStyles = makeStyles((theme) => ({
   chip: {
      minWidth: 56,
   },
   chipPreselected: {},
}));

export default function MultipleChoiceAnswerForm({
   onChangeCurrentAnswer,
   currentAnswer,
   onSubmit,
   onDelete,
   poll: { poll, answer },
}: PollAnswerFormProps<MultipleChoiceAnswer>) {
   if (poll.instruction.type !== 'multipleChoice') throw new Error('Multiple choice instruction required');

   const classes = useStyles();
   const selected = currentAnswer.selected;

   const handleSelectOption = (option: string) => {
      const answer: MultipleChoiceAnswer = {
         type: 'multipleChoice',
         selected: selected?.includes(option)
            ? selected.filter((x) => x !== option)
            : [...(selected || []), option].sort(),
      };
      onChangeCurrentAnswer(answer);

      if (!poll.config.isAnswerFinal) {
         if (answer.selected.length === 0) {
            onDelete();
         } else {
            onSubmit(answer);
         }
      }
   };

   return (
      <Grid container spacing={1} justify="center">
         {poll.instruction.options.map((x) => (
            <Grid item key={x}>
               <SelectionChip
                  option={x}
                  className={classes.chip}
                  applied={answer?.answer.type === 'multipleChoice' && answer.answer.selected.includes(x)}
                  selected={Boolean(selected?.includes(x))}
                  disabled={Boolean(answer && poll.config.isAnswerFinal)}
                  onClick={() => handleSelectOption(x)}
               />
            </Grid>
         ))}
      </Grid>
   );
}
