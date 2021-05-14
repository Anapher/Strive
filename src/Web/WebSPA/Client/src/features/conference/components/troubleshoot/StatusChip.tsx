import { Chip, makeStyles } from '@material-ui/core';
import React from 'react';
import { HealthStatus } from './useWebRtcHealth';
import clsx from 'classnames';

const useStyles = makeStyles((theme) => ({
   statusChipOk: {
      backgroundColor: '#27ae60',
   },
   statusChipError: {
      backgroundColor: theme.palette.error.main,
   },
}));

type Props = React.ComponentProps<typeof Chip> & {
   status: HealthStatus;
};

export default function StatusChip({ status, className, ...props }: Props) {
   const classes = useStyles();

   return (
      <Chip
         className={clsx(className, {
            [classes.statusChipOk]: status === 'ok',
            [classes.statusChipError]: status === 'error',
         })}
         {...props}
      />
   );
}
