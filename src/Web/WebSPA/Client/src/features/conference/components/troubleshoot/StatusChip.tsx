import { Chip, makeStyles } from '@material-ui/core';
import React from 'react';
import clsx from 'classnames';
import { HealthStatus } from './utils';

const useStyles = makeStyles((theme) => ({
   statusChip: {
      cursor: 'pointer',
   },
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
         className={clsx(className, classes.statusChip, {
            [classes.statusChipOk]: status === 'ok',
            [classes.statusChipError]: status === 'error',
         })}
         {...props}
      />
   );
}
