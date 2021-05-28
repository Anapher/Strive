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
import AddIcon from '@material-ui/icons/Add';
import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { mergeDeep } from 'src/utils/object-merge';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import { ConferenceDataForm } from '../form';
import TabCommon from './TabCommon';
import TabModerators from './TabModerators';
import TabPermissions from './TabPermissions';

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

interface TabPanelProps {
   children?: React.ReactNode;
   index: number;
   value: number;
   className: string;
}

function TabPanel(props: TabPanelProps) {
   const { children, value, index, ...other } = props;

   return (
      <div
         role="tabpanel"
         hidden={value !== index}
         id={`create-conference-tabpanel-${index}`}
         aria-labelledby={`create-conference-tab-${index}`}
         {...other}
      >
         {value === index && children}
      </div>
   );
}

function a11yProps(index: number) {
   return {
      id: `create-conference-tab-${index}`,
      'aria-controls': `create-conference-tabpanel-${index}`,
   };
}

type Props = {
   defaultValues: ConferenceDataForm;
   onSubmit: (data: ConferenceDataForm) => void;
   isSubmitting: boolean;
   onClose: () => void;
   conferenceId: string | null;

   mode: 'create' | 'patch';
};

const patchConferenceData =
   (callback: (data: ConferenceDataForm) => void, defaultValues: ConferenceDataForm) => (data: ConferenceDataForm) =>
      callback(mergeDeep(data, { configuration: defaultValues.configuration }));

export default function CreateConferenceForm({
   defaultValues,
   onSubmit,
   isSubmitting,
   onClose,
   mode,
   conferenceId,
}: Props) {
   const classes = useStyles();
   const theme = useTheme();
   const { t } = useTranslation();

   const form = useForm<ConferenceDataForm>({
      mode: 'onChange',
      defaultValues,
   });
   const { register, formState, handleSubmit } = form;
   const [currentTab, setCurrentTab] = useState(0);

   const handleChangeTab = (_: unknown, newValue: number) => {
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
            <Paper square className={classes.header}>
               <Tabs
                  value={currentTab}
                  indicatorColor="primary"
                  textColor="primary"
                  onChange={handleChangeTab}
                  aria-label="options tabs"
               >
                  <Tab label={t('common:common')} {...a11yProps(0)} />
                  <Tab label={t('common:moderator_plural')} {...a11yProps(1)} />
                  <Tab label={t('common:permissions')} {...a11yProps(2)} />
               </Tabs>
            </Paper>
            <Box position="relative" flex={1} minHeight={0}>
               <TabPanel value={currentTab} index={0} className={classes.tabPanel}>
                  <TabCommon form={form} />
               </TabPanel>
               <TabPanel value={currentTab} index={1} className={classes.tabPanel}>
                  <TabModerators form={form} conferenceId={conferenceId} />
               </TabPanel>
               <TabPanel value={currentTab} index={2} className={classes.tabPanel}>
                  <TabPermissions form={form} />
               </TabPanel>
               <Zoom
                  in={currentTab === 1}
                  timeout={transitionDuration}
                  style={{
                     transitionDelay: `${currentTab === 1 ? transitionDuration.exit : 0}ms`,
                  }}
               >
                  <Fab aria-label="Add moderator" className={classes.fab} color="primary" onClick={handleAddModerator}>
                     <AddIcon />
                  </Fab>
               </Zoom>
            </Box>
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
