import {
   Avatar,
   Box,
   Checkbox,
   Collapse,
   Fab,
   FormControlLabel,
   Grid,
   IconButton,
   InputAdornment,
   List,
   ListItem,
   ListItemAvatar,
   ListItemText,
   makeStyles,
   Paper,
   Tab,
   Tabs,
   TextField,
   Typography,
   useTheme,
   Zoom,
} from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import PersonIcon from '@material-ui/icons/Person';
import ScheduleIcon from '@material-ui/icons/Schedule';
import { TabContext, TabPanel, ToggleButton, ToggleButtonGroup } from '@material-ui/lab';
import { DateTimePicker } from '@material-ui/pickers';
import cronstrue from 'cronstrue';
import { DateTime } from 'luxon';
import React, { useState } from 'react';
import { Controller, UseFormMethods } from 'react-hook-form';
import { CreateConferenceFormState } from '../form';
import { ConferenceType } from '../types';
import ConferenceTypePreview from './ConferenceTypePreview';

const checkBoxWidth = 150;

const useStyles = makeStyles((theme) => ({
   checkBoxLabel: {
      margin: 0,
      width: 150,
   },
   fab: {
      position: 'absolute',
      bottom: theme.spacing(2),
      right: theme.spacing(2),
   },
}));

type Props = {
   form: UseFormMethods<CreateConferenceFormState>;
};

const availableConferenceTypes: ConferenceType[] = ['class', 'presentation'];

const conferenceTypeDescriptions: { [type in ConferenceType]: string } = {
   class:
      'The presenter has an overview about the entire class. Breakout rooms are available. Temporarily users can become the presenter to show off their work.',
   presentation:
      'The presenter in the foreground without much interaction with users. This mode is great for lectures.',
};

export default function CreateConferenceForm({ form: { control, register, watch, errors } }: Props) {
   const classes = useStyles();
   const state = watch(['scheduleCron', 'conferenceType', 'enableStartTime', 'schedule', 'moderators']);
   const theme = useTheme();

   const [cronDesc, setCronDesc] = useState<string | undefined>();

   const validateSchedulerCron = (s: string) => {
      if (!state.schedule) return true;

      try {
         const desc = cronstrue.toString(s);
         setCronDesc(desc);
         return true;
      } catch (error) {
         return error.toString();
      }
   };

   const [currentTab, setCurrentTab] = useState('1');

   const handleChange = (_: unknown, newValue: string) => {
      setCurrentTab(newValue);
   };

   const transitionDuration = {
      enter: theme.transitions.duration.enteringScreen,
      exit: theme.transitions.duration.leavingScreen,
   };

   return (
      <Box display="flex" flexDirection="column">
         <Box mb={2} px={3}>
            <TextField fullWidth label="Name" name="name" inputRef={register} placeholder="Unnamed conference" />
         </Box>
         <TabContext value={currentTab}>
            <Paper square>
               <Tabs
                  value={currentTab}
                  indicatorColor="primary"
                  textColor="primary"
                  onChange={handleChange}
                  aria-label="options tabs"
               >
                  <Tab label="Common" value="1" />
                  <Tab label="Schedule" value="2" />
                  <Tab label="Moderators" value="3" />
               </Tabs>
            </Paper>
            <Box height={300} position="relative">
               <TabPanel value="1">
                  <Grid container spacing={2}>
                     <Grid item xs={12}>
                        <Controller
                           name="conferenceType"
                           control={control}
                           defaultValue="class"
                           render={({ onChange, onBlur, value }) => (
                              <ToggleButtonGroup
                                 exclusive
                                 onChange={(_, val) => {
                                    if (val) onChange(val);
                                 }}
                                 onBlur={onBlur}
                                 value={value}
                              >
                                 {availableConferenceTypes.map((x) => (
                                    <ToggleButton value={x} aria-label={`use ${x}`} key={x}>
                                       <ConferenceTypePreview type={x} width={120} animate={value === x} />
                                    </ToggleButton>
                                 ))}
                              </ToggleButtonGroup>
                           )}
                        />
                     </Grid>
                     <Grid item xs={12}>
                        <Typography style={{ height: '4.5rem' }}>
                           {conferenceTypeDescriptions[state.conferenceType]}
                        </Typography>
                     </Grid>
                  </Grid>
               </TabPanel>
               <TabPanel value="2">
                  <Grid container>
                     <Grid item xs={12}>
                        <Box display="flex" flexDirection="row" alignItems="center">
                           <FormControlLabel
                              className={classes.checkBoxLabel}
                              control={
                                 <Controller
                                    render={({ onChange, value }) => (
                                       <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                                    )}
                                    control={control}
                                    name="enableStartTime"
                                 />
                              }
                              label="Start"
                           />
                           <Controller
                              name="startTime"
                              control={control}
                              rules={{ validate: (x: string) => DateTime.fromISO(x).isValid }}
                              render={({ onChange, value }) => (
                                 <DateTimePicker
                                    error={!!errors.startTime}
                                    disabled={!state.enableStartTime}
                                    disablePast
                                    onChange={(x) => onChange(x?.toISO())}
                                    value={value && DateTime.fromISO(value)}
                                    ampm={false}
                                 />
                              )}
                           />
                        </Box>
                     </Grid>
                     <Grid item xs={12}>
                        <div>
                           <Box display="flex" flexDirection="row" alignItems="center">
                              <FormControlLabel
                                 className={classes.checkBoxLabel}
                                 control={
                                    <Controller
                                       render={({ onChange, value }) => (
                                          <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                                       )}
                                       control={control}
                                       name="schedule"
                                    />
                                 }
                                 label="Schedule"
                              />
                              <TextField
                                 style={{ flex: 1 }}
                                 disabled={!state.schedule}
                                 required={state.schedule}
                                 placeholder="Cron"
                                 name="scheduleCron"
                                 error={!!errors.scheduleCron}
                                 inputRef={register({ required: state.schedule, validate: validateSchedulerCron })}
                                 InputProps={{
                                    endAdornment: (
                                       <InputAdornment position="end">
                                          <IconButton
                                             disabled={!state.schedule}
                                             aria-label="open cron expression generator"
                                             title="Cron Expression Generator"
                                             href="https://www.freeformatter.com/cron-expression-generator-quartz.html"
                                             target="_blank"
                                          >
                                             <ScheduleIcon />
                                          </IconButton>
                                       </InputAdornment>
                                    ),
                                 }}
                              />
                           </Box>
                           <Collapse in={state.schedule}>
                              <div style={{ marginLeft: checkBoxWidth }}>
                                 <Typography variant="caption" color={errors.scheduleCron ? 'error' : 'textSecondary'}>
                                    {errors.scheduleCron?.message || cronDesc}
                                 </Typography>
                              </div>
                           </Collapse>
                        </div>
                     </Grid>
                  </Grid>
               </TabPanel>
               <TabPanel value="3">
                  <List>
                     {state.moderators?.map((x) => (
                        <ListItem key={x.id}>
                           <ListItemAvatar>
                              <Avatar>
                                 <PersonIcon />
                              </Avatar>
                           </ListItemAvatar>
                           <ListItemText primary={x.name} />
                        </ListItem>
                     ))}
                  </List>
               </TabPanel>

               <Zoom
                  in={currentTab === '3'}
                  timeout={transitionDuration}
                  style={{
                     transitionDelay: `${currentTab === '3' ? transitionDuration.exit : 0}ms`,
                  }}
               >
                  <Fab aria-label="Add moderator" className={classes.fab} color="primary">
                     <AddIcon />
                  </Fab>
               </Zoom>
            </Box>
         </TabContext>
      </Box>
   );
}
