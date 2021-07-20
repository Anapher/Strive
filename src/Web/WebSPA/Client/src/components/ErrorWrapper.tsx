import { Button, makeStyles, Typography } from '@material-ui/core';
import { Alert } from '@material-ui/lab';
import React from 'react';
import { useTranslation } from 'react-i18next';

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
   const { t } = useTranslation();

   return failed ? (
      <div>
         {type === 'alert' ? (
            <Alert
               severity="error"
               action={
                  onRetry && (
                     <Button color="inherit" size="small" onClick={onRetry}>
                        {t('common:try_again')}
                     </Button>
                  )
               }
            >
               {error ?? t('error_occurred')}
            </Alert>
         ) : (
            <>
               <Typography color="error">{error}</Typography>
               {onRetry && (
                  <Button variant="contained" onClick={onRetry} className={classes.retryButton}>
                     {t('common:try_again')}
                  </Button>
               )}
            </>
         )}
      </div>
   ) : (
      <>{children}</>
   );
}
