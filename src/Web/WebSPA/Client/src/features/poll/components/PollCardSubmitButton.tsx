import { Button, makeStyles } from '@material-ui/core';
import React from 'react';
import clsx from 'classnames';
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles((theme) => ({
   pulsing: {
      color: theme.palette.secondary.main,
      animationName: '$pulsing',
      animationDuration: '1.5s',
      animationIterationCount: 'infinite',
   },
   '@keyframes pulsing': {
      '0%': {
         color: theme.palette.secondary.dark,
      },
      '50%': {
         color: theme.palette.secondary.light,
      },
      '100%': {
         color: theme.palette.secondary.dark,
      },
   },
}));

type Props = React.ComponentProps<typeof Button>;

export default function PollCardSubmitButton({ className, ...props }: Props) {
   const { t } = useTranslation();
   const classes = useStyles();

   return (
      <Button className={clsx(className, !props.disabled && classes.pulsing)} size="small" color="secondary" {...props}>
         {t('conference.poll.submit_answer')}
      </Button>
   );
}
