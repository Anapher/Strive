import {
   Box,
   Button,
   DialogActions,
   Fab,
   LinearProgress,
   makeStyles,
   Paper,
   Tab,
   Tabs,
   TextField,
   useTheme,
   Zoom,
} from '@material-ui/core';
import { TabContext, TabPanel } from '@material-ui/lab';
import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { ConferenceDataForm } from '../form';
import TabCommon from './TabCommon';
import TabModerators from './TabModerators';
import TabPermissions from './TabPermissions';
import AddIcon from '@material-ui/icons/Add';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import { mergeDeep } from 'src/utils/object-merge';

const useStyles = makeStyles((theme) => ({
   header: {
      backgroundColor: theme.palette.grey[800],
   },
   fab: {
      position: 'absolute',
      bottom: theme.spacing(2),
      right: theme.spacing(2),
   },
   tabPanel: {
      height: '100%',
      padding: 0,
   },
}));

type Props = {
   defaultValues: ConferenceDataForm;
   onSubmit: (data: ConferenceDataForm) => void;
   isSubmitting: boolean;
   onClose: () => void;
};

const patchConferenceData = (callback: (data: ConferenceDataForm) => void, defaultValues: ConferenceDataForm) => (
   data: ConferenceDataForm,
) => callback(mergeDeep(data, { configuration: defaultValues.configuration }));

export default function CreateConferenceForm({ defaultValues, onSubmit, isSubmitting, onClose }: Props) {
   const form = useForm<ConferenceDataForm>({
      mode: 'onChange',
      defaultValues,
   });

   const [currentTab, setCurrentTab] = useState('1');

   const handleChangeTab = (_: unknown, newValue: string) => {
      setCurrentTab(newValue);
   };

   const { register, formState, handleSubmit } = form;
   const classes = useStyles();
   const theme = useTheme();

   const transitionDuration = {
      enter: theme.transitions.duration.enteringScreen,
      exit: theme.transitions.duration.leavingScreen,
   };

   const handleAddModerator = () => {
      const id = prompt('Enter the id of the user to add to moderators');
      if (id) {
         const currentMods = form.getValues('configuration.moderators') as string[];
         if (!currentMods.includes(id)) form.setValue('configuration.moderators', [...currentMods, id]);
      }
   };

   return (
      <form onSubmit={handleSubmit(patchConferenceData(onSubmit, defaultValues))}>
         <Box display="flex" flexDirection="column">
            <Box mb={2} px={3}>
               <TextField
                  fullWidth
                  label="Name"
                  {...wrapForInputRef(register('configuration.name'))}
                  placeholder="Unnamed conference"
               />
            </Box>
            <TabContext value={currentTab}>
               <Paper square className={classes.header}>
                  <Tabs
                     value={currentTab}
                     indicatorColor="primary"
                     textColor="primary"
                     onChange={handleChangeTab}
                     aria-label="options tabs"
                  >
                     <Tab label="Common" value="1" />
                     <Tab label="Moderators" value="2" />
                     <Tab label="Permissions" value="3" />
                  </Tabs>
               </Paper>
               <Box height={400} position="relative">
                  <TabPanel value="1" className={classes.tabPanel}>
                     <TabCommon form={form} />
                  </TabPanel>
                  <TabPanel value="2" className={classes.tabPanel}>
                     <TabModerators form={form} />
                  </TabPanel>
                  <TabPanel value="3" className={classes.tabPanel}>
                     <TabPermissions form={form} />
                  </TabPanel>
                  <Zoom
                     in={currentTab === '2'}
                     timeout={transitionDuration}
                     style={{
                        transitionDelay: `${currentTab === '2' ? transitionDuration.exit : 0}ms`,
                     }}
                  >
                     <Fab
                        aria-label="Add moderator"
                        className={classes.fab}
                        color="primary"
                        onClick={handleAddModerator}
                     >
                        <AddIcon />
                     </Fab>
                  </Zoom>
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
