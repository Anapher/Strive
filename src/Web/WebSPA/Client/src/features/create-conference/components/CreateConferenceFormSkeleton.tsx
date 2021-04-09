import { Box, makeStyles } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import React from 'react';

const useStyles = makeStyles((theme) => ({
   controlSkeleton: {
      marginTop: theme.spacing(2),
      marginLeft: theme.spacing(2),
      marginRight: theme.spacing(2),
      height: 32,
   },
}));

export default function CreateConferenceFormSkeleton() {
   const classes = useStyles();
   return (
      <div>
         <Box mb={2} px={3} mt={2}>
            <Skeleton variant="rect" height={40} />
         </Box>
         <Skeleton variant="rect" height={45} />

         <Box mt={4}>
            {Array.from({ length: 5 }).map((_, i) => (
               <Skeleton key={i} className={classes.controlSkeleton} variant="rect" />
            ))}
         </Box>
      </div>
   );
}
