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
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles((theme) => ({
   form: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
   },
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

   mode: 'create' | 'patch';
};

const patchConferenceData = (callback: (data: ConferenceDataForm) => void, defaultValues: ConferenceDataForm) => (
   data: ConferenceDataForm,
) => callback(mergeDeep(data, { configuration: defaultValues.configuration }));

export default function CreateConferenceForm({ defaultValues, onSubmit, isSubmitting, onClose, mode }: Props) {
   const classes = useStyles();
   const theme = useTheme();
   const { t } = useTranslation();

   const form = useForm<ConferenceDataForm>({
      mode: 'onChange',
      defaultValues,
   });
   const { register, formState, handleSubmit } = form;
   const [currentTab, setCurrentTab] = useState('1');

   const handleChangeTab = (_: unknown, newValue: string) => {
      setCurrentTab(newValue);
   };

   const transitionDuration = {
      enter: theme.transitions.duration.enteringScreen,
      exit: theme.transitions.duration.leavingScreen,
   };

   const handleAddModerator = () => {
      const id = prompt(t('dialog_create_conference.enter_moderator_id'));
      if (id) {
         const currentMods = form.getValues('configuration.moderators') as string[];
         if (!currentMods.includes(id)) form.setValue('configuration.moderators', [...currentMods, id]);
      }
   };

   return (
      <form onSubmit={handleSubmit(patchConferenceData(onSubmit, defaultValues))} className={classes.form}>
         <Box display="flex" flexDirection="column" flex={1} minHeight={0}>
            <Box mb={2} px={3}>
               <TextField
                  fullWidth
                  label={t('common:name')}
                  {...wrapForInputRef(register('configuration.name'))}
                  placeholder={t('dialog_create_conference.unnamed_conference')}
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
                     <Tab label={t('common:common')} value="1" />
                     <Tab label={t('common:moderator_plural')} value="2" />
                     <Tab label={t('common:permissions')} value="3" />
                  </Tabs>
               </Paper>
               <Box position="relative" flex={1} minHeight={0}>
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
               {t('common:cancel')}
            </Button>
            <Button type="submit" color="primary" autoFocus disabled={isSubmitting || !formState.isValid}>
               {mode === 'patch' ? t('common:change') : t('common:create')}
            </Button>
         </DialogActions>
         {isSubmitting && <LinearProgress />}
      </form>
   );
}
