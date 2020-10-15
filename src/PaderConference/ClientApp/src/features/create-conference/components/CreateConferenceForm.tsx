import {
   Box,
   Checkbox,
   Collapse,
   FormControlLabel,
   Grid,
   IconButton,
   InputAdornment,
   makeStyles,
   Paper,
   Tab,
   Tabs,
   TextField,
   Typography,
} from '@material-ui/core';
import { ToggleButton, ToggleButtonGroup } from '@material-ui/lab';
import { DateTimePicker } from '@material-ui/pickers';
import React, { useMemo, useState } from 'react';
import { Controller, UseFormMethods } from 'react-hook-form';
import { ConferenceType } from '../types';
import ConferenceTypePreview from './ConferenceTypePreview';
import ScheduleIcon from '@material-ui/icons/Schedule';
import cronstrue from 'cronstrue';

const checkBoxWidth = 150;

const useStyles = makeStyles({
   checkBoxLabel: {
      margin: 0,
      width: 150,
   },
});

export type CreateConferenceFormState = {
   conferenceType: ConferenceType;
   scheduledDate: string;
   schedule: boolean;
   repeat: boolean;
   repeatCron: string;
};

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

export default function CreateConferenceForm({ form: { control, register, watch } }: Props) {
   const classes = useStyles();
   const state = watch();

   const cronDesc = useMemo(() => {
      try {
         return cronstrue.toString(state.repeatCron);
      } catch (error) {
         return error.toString();
      }
   }, [state.repeatCron]);

   const [currentTab, setCurrentTab] = useState(0);

   const handleChange = (event: React.ChangeEvent<{}>, newValue: number) => {
      setCurrentTab(newValue);
   };

   return (
      <Box display="flex" flexDirection="column">
         <Box mb={2}>
            <TextField fullWidth label="Name" name="name" inputRef={register} placeholder="Unnamed conference" />
         </Box>
         <Paper square>
            <Tabs
               value={currentTab}
               indicatorColor="primary"
               textColor="primary"
               onChange={handleChange}
               aria-label="options tabs"
            >
               <Tab label="common">
                  <Grid container>
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
               </Tab>
            </Tabs>
         </Paper>
      </Box>
   );

   return (
      <Grid container spacing={2}>
         <Grid item xs={12}></Grid>

         <Grid item xs={12}>
            <Box display="flex" flexDirection="row" alignItems="center">
               <FormControlLabel
                  className={classes.checkBoxLabel}
                  control={<Checkbox inputRef={register} name="schedule" />}
                  label="Schedule"
               />
               <Controller
                  name="scheduledDate"
                  control={control}
                  render={({ onChange, value }) => (
                     <DateTimePicker
                        disabled={!state.schedule}
                        disablePast
                        onChange={onChange}
                        value={value}
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
                     control={<Checkbox inputRef={register} name="repeat" />}
                     label="Repeat"
                  />
                  <TextField
                     style={{ flex: 1 }}
                     disabled={!state.repeat}
                     required={state.repeat}
                     placeholder="Cron"
                     name="repeatCron"
                     inputRef={register({ required: state.repeat })}
                     InputProps={{
                        endAdornment: (
                           <InputAdornment position="end">
                              <IconButton
                                 disabled={!state.repeat}
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
               <Collapse in={state.repeat}>
                  <div style={{ marginLeft: checkBoxWidth }}>
                     <Typography variant="caption" color="textSecondary">
                        {cronDesc}
                     </Typography>
                  </div>
               </Collapse>
            </div>
         </Grid>
      </Grid>
   );
}
