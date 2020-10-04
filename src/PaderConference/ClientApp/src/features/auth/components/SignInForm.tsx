import { Box, Button, Checkbox, FormControlLabel, Grid, LinearProgress, TextField } from '@material-ui/core';
import { SignInRequest } from 'MyModels';
import React from 'react';
import { useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { signInAsync } from '../signInReducer';

export default function SignInForm() {
   const dispatch = useDispatch();

   const isLoading = useSelector<RootState>((state) => state.signIn.isLoading);
   const { register, handleSubmit, watch, errors, formState } = useForm({ mode: 'onChange' });

   const handleSignIn = (dto: SignInRequest) => {
      dispatch(signInAsync(dto));
   };

   // const signInCallback = useCallback(
   //    async (values: SignInRequest, formikActions: FormikHelpers<SignInRequest>) => {
   //       const { setSubmitting } = formikActions;
   //       try {
   //          await signInAction!(values);
   //          // the view will automatically change when the user is authenticated
   //       } catch (error) {
   //          applyError(error, formikActions);
   //       } finally {
   //          setSubmitting(false);
   //       }
   //    },
   //    [signInAction],
   // );

   return (
      <form noValidate onSubmit={handleSubmit(handleSignIn)}>
         <Grid container spacing={2}>
            <Grid item xs={12}>
               <TextField fullWidth required label="Username" name="userName" inputRef={register({ required: true })} />
            </Grid>
            <Grid item xs={12}>
               <TextField
                  label="Password"
                  name="password"
                  fullWidth
                  type="password"
                  autoComplete="current-password"
                  required
                  inputRef={register({ required: true })}
               />
            </Grid>
            <Grid item>
               <FormControlLabel
                  label="Remember me"
                  control={
                     <Checkbox
                        inputRef={register}
                        name="rememberMe"
                        value="remember"
                        color="primary"
                        defaultValue={false as any}
                     />
                  }
               />
            </Grid>
            <Grid item xs={12} style={{ padding: 4 }}>
               {!isLoading ? (
                  <Button
                     type="submit"
                     fullWidth
                     variant="contained"
                     disabled={!formState.isValid || !formState.isDirty}
                     color="primary"
                  >
                     Sign in
                  </Button>
               ) : (
                  <Box mt={1} width="100%">
                     <LinearProgress />
                  </Box>
               )}
            </Grid>
            {/* {status && <Typography color="error">{status}</Typography>} */}
         </Grid>
      </form>
   );
}
