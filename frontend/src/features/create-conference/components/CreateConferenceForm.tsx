import { Box, Button, DialogActions, LinearProgress, makeStyles, Paper, Tab, Tabs, TextField } from '@material-ui/core';
import { TabContext, TabPanel } from '@material-ui/lab';
import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { ConferenceDataForm } from '../form';
import TabCommon from './TabCommon';
import TabPermissions from './TabPermissions';

const useStyles = makeStyles((theme) => ({
   header: {
      backgroundColor: theme.palette.grey[800],
   },
   fab: {
      position: 'absolute',
      bottom: theme.spacing(2),
      right: theme.spacing(2),
   },
}));

type Props = {
   defaultValues: ConferenceDataForm;
   onSubmit: (data: ConferenceDataForm) => void;
   isSubmitting: boolean;
   onClose: () => void;
};

export default function CreateConferenceForm({ defaultValues, onSubmit, isSubmitting, onClose }: Props) {
   const form = useForm<ConferenceDataForm>({
      mode: 'onChange',
      shouldUnregister: false,
      defaultValues: defaultValues,
   });

   const [currentTab, setCurrentTab] = useState('1');

   const handleChange = (_: unknown, newValue: string) => {
      setCurrentTab(newValue);
   };

   const { register, formState, handleSubmit } = form;
   const classes = useStyles();

   return (
      <form onSubmit={handleSubmit(onSubmit)}>
         <Box display="flex" flexDirection="column">
            <Box mb={2} px={3}>
               <TextField fullWidth label="Name" name="name" inputRef={register} placeholder="Unnamed conference" />
            </Box>
            <TabContext value={currentTab}>
               <Paper square className={classes.header}>
                  <Tabs
                     value={currentTab}
                     indicatorColor="primary"
                     textColor="primary"
                     onChange={handleChange}
                     aria-label="options tabs"
                  >
                     <Tab label="Common" value="1" />
                     <Tab label="Moderators" value="2" />
                     <Tab label="Permissions" value="3" />
                  </Tabs>
               </Paper>
               <Box height={300} position="relative">
                  <TabPanel value="1">
                     <TabCommon form={form} />
                  </TabPanel>
                  <TabPanel value="1">
                     <TabPermissions form={form} />
                  </TabPanel>
               </Box>
            </TabContext>
         </Box>
         <DialogActions>
            <Button autoFocus onClick={onClose} color="primary" disabled={isSubmitting}>
               Cancel
            </Button>
            <Button type="submit" color="primary" autoFocus disabled={isSubmitting || !formState.isValid}>
               Create
            </Button>
         </DialogActions>
         {isSubmitting && <LinearProgress />}
      </form>
   );
}
