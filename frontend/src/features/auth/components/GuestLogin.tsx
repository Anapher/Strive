import { Box, Button, Grid, LinearProgress, makeStyles, Paper, TextField, Typography } from '@material-ui/core';
import { GuestSignInRequest } from 'MyModels';
import React from 'react';
import { useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { signInGuestAsync } from '../sign-in-reducer';

const useStyles = makeStyles((theme) => ({
   paper: {
      padding: theme.spacing(2, 3, 3),
   },
}));

export default function GuestLogin() {
   const classes = useStyles();
   const dispatch = useDispatch();

   const isLoading = useSelector((state: RootState) => state.signIn.isSigningIn);
   const isGuestLoading = useSelector((state: RootState) => state.signIn.isSigningInGuest);

   const { register, handleSubmit, formState } = useForm({ mode: 'onChange' });

   const handleContinue = (dto: GuestSignInRequest) => {
      dispatch(signInGuestAsync(dto));
   };

   return (
      <Paper className={classes.paper}>
         <Typography component="h1" variant="h5" align="center">
            {"I'm a Guest"}
         </Typography>
         <form noValidate onSubmit={handleSubmit(handleContinue)}>
            <Grid container spacing={2}>
               <Grid item xs={12}>
                  <TextField
                     fullWidth
                     required
                     label="Your name"
                     name="displayName"
                     inputRef={register({ required: true })}
                  />
               </Grid>
               <Grid item xs={12}>
                  {!isGuestLoading ? (
                     <Button
                        color="primary"
                        type="submit"
                        fullWidth
                        variant="contained"
                        disabled={!formState.isValid || !formState.isDirty || isLoading}
                     >
                        Continue
                     </Button>
                  ) : (
                     <Box mt={1} width="100%">
                        <LinearProgress />
                     </Box>
                  )}
               </Grid>
            </Grid>
         </form>
      </Paper>
   );
}
