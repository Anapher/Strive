import { makeStyles } from '@material-ui/core';
import { Trans, useTranslation } from 'react-i18next';
import { Participant } from 'src/features/conference/types';

const useStyles = makeStyles((theme) => ({
   participantText: {
      color: theme.palette.secondary.main,
   },
}));

type Props = {
   participants: Participant[];
   participantColors: { [id: string]: string };
};

export default function ParticipantsTypingText({ participants, participantColors }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   const beginning = participants.slice(0, 2);
   const tail = participants.slice(2);

   const showAndForLastParticipant = tail.length === 0;

   return (
      <>
         {beginning.map((x, i) => (
            <span key={x.id}>
               {i !== 0 && (
                  <span>
                     {i === participants.length - 1 && showAndForLastParticipant
                        ? ' ' + t('conference.chat.typing.and') + ' '
                        : ', '}
                  </span>
               )}
               <span className={classes.participantText} style={{ color: participantColors[x.id] }}>
                  {x.displayName}
               </span>{' '}
            </span>
         ))}
         {tail.length > 0 ? (
            <span>
               <Trans i18nKey="conference.chat.typing.tail_more_typing" count={tail.length}>
                  and <span className={classes.participantText}>{{ count: tail.length }}</span> more are typing
               </Trans>
            </span>
         ) : (
            <span>{t('conference.chat.typing.tail_typing', { count: beginning.length })}</span>
         )}
      </>
   );
}
