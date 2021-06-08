import { Chip, Grid, makeStyles } from '@material-ui/core';
import React from 'react';
import { SingleChoiceAnswer } from '../../types';
import { PollAnswerFormProps } from '../types';

const useStyles = makeStyles({
   chip: {
      minWidth: 56,
   },
});

export default function SingleChoiceAnswerForm({
   onSubmit,
   onDelete,
   poll: { poll, answer },
}: PollAnswerFormProps<SingleChoiceAnswer>) {
   const classes = useStyles();

   if (poll.instruction.type !== 'singleChoice') throw new Error('Single choice instruction required');

   const applied = answer?.answer.type === 'singleChoice' ? answer.answer.selected : undefined;

   const handleSubmitOption = (option: string) => {
      if (applied === option) {
         onDelete();
      } else {
         onSubmit({ type: 'singleChoice', selected: option });
      }
   };

   return (
      <Grid container spacing={1} justify="center">
         {poll.instruction.options.map((x) => (
            <Grid item key={x}>
               <Chip
                  className={classes.chip}
                  label={x}
                  color={applied === x ? 'primary' : undefined}
                  disabled={answer && poll.config.isAnswerFinal}
                  onClick={() => handleSubmitOption(x)}
                  clickable
                  component="button"
               />
            </Grid>
         ))}
      </Grid>
   );
}
