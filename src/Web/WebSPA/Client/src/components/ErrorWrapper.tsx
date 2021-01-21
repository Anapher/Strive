import { Button, makeStyles, Typography } from '@material-ui/core';
import { Alert } from '@material-ui/lab';
import React from 'react';

const useStyles = makeStyles((theme) => ({
   retryButton: {
      marginTop: theme.spacing(1),
   },
}));

type Props = {
   onRetry?: () => void;
   error?: string | null;
   failed: boolean;
   children: React.ReactNode;
   type?: 'alert' | 'typography';
};

export default function ErrorWrapper({ failed, children, error, onRetry, type = 'alert' }: Props) {
   const classes = useStyles();

   return failed ? (
      <div>
         {type === 'alert' ? (
            <Alert
               severity="error"
               action={
                  onRetry && (
                     <Button color="inherit" size="small" onClick={onRetry}>
                        Retry
                     </Button>
                  )
               }
            >
               {error ?? 'An error occcurred.'}
            </Alert>
         ) : (
            <>
               <Typography color="error">{error}</Typography>
               {onRetry && (
                  <Button variant="contained" onClick={onRetry} className={classes.retryButton}>
                     Retry
                  </Button>
               )}
            </>
         )}
      </div>
   ) : (
      <>{children}</>
   );
}
