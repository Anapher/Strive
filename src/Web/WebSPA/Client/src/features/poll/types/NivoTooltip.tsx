import { makeStyles, Paper, Typography } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { RootState } from 'src/store';
import { selectPollResults } from '../selectors';

const useStyles = makeStyles({
   paper: {
      padding: 8,
      maxWidth: 400,
   },
});

type Props = {
   header: React.ReactChild;
   participantTokens?: string[];
   pollId: string;
};

export default function NivoTooltip(props: Props) {
   const classes = useStyles();

   return (
      <Paper elevation={10} square className={classes.paper}>
         <NivoTooltipContent {...props} />
      </Paper>
   );
}

export function NivoTooltipContent({ header, pollId, participantTokens }: Props) {
   const participants = useSelector(selectParticipants);
   const results = useSelector((state: RootState) => selectPollResults(state, pollId));
   const translationTable = results?.tokenIdToParticipant;

   return (
      <>
         <Typography>{header}</Typography>
         {translationTable && participantTokens && (
            <Typography variant="caption">
               {participantTokens.map((x) => participants[translationTable[x]]?.displayName).join(', ')}
            </Typography>
         )}
      </>
   );
}
