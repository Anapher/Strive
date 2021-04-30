import {
   Box,
   Checkbox,
   Collapse,
   FormControl,
   FormControlLabel,
   Grid,
   IconButton,
   InputAdornment,
   InputLabel,
   makeStyles,
   MenuItem,
   Select,
   TextField,
   Typography,
} from '@material-ui/core';
import cronstrue from 'cronstrue/i18n';
import { DateTime } from 'luxon';
import React, { useEffect, useRef, useState } from 'react';
import { Controller, UseFormReturn } from 'react-hook-form';
import ScheduleIcon from '@material-ui/icons/Schedule';
import { ConferenceDataForm } from '../form';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import NativeIsoDateInput from 'src/components/NativeIsoDateInput';
import { useTranslation } from 'react-i18next';

const checkBoxWidth = 150;

const useStyles = makeStyles((theme) => ({
   root: {
      padding: theme.spacing(3),
      width: '100%',
      height: '100%',
      overflowY: 'auto',
   },
   checkBoxLabel: {
      width: checkBoxWidth,
   },
   sectionGrid: {
      paddingTop: theme.spacing(3),
   },
   selectFormControl: {
      maxWidth: 240,
      margin: theme.spacing(2, 0),
   },
}));

type Props = {
   form: UseFormReturn<ConferenceDataForm>;
};

export default function TabCommon({
   form: {
      control,
      watch,
      register,
      formState: { errors },
      getValues,
   },
}: Props) {
   const classes = useStyles();
   const startTime: boolean = watch('additionalFormData.enableStartTime');
   const scheduleCron: boolean = watch('additionalFormData.enableSchedule');
   const { t, i18n } = useTranslation();

   const [cronDesc, setCronDesc] = useState<string | undefined>();
   const currentTimeIso = useRef(DateTime.local().toISO());

   const validateSchedulerCron = (s: string | undefined | null) => {
      if (!scheduleCron) return true;
      if (!s) {
         return 'Cannot be empty.';
      }

      try {
         const desc = cronstrue.toString(s, { locale: i18n.language.split('-')[0] });
         setCronDesc(desc);
         return true;
      } catch (error) {
         return error.toString() as string;
      }
   };

   useEffect(() => {
      if (scheduleCron) validateSchedulerCron(getValues().configuration.scheduleCron ?? '');
   }, [scheduleCron]);

   return (
      <Grid container className={classes.root}>
         <Grid item xs={12}>
            <Box display="flex" flexDirection="row" alignItems="center">
               <FormControlLabel
                  className={classes.checkBoxLabel}
                  control={
                     <Controller
                        render={({ field: { onChange, value } }) => (
                           <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                        )}
                        control={control}
                        name="additionalFormData.enableStartTime"
                     />
                  }
                  label={t('dialog_create_conference.tabs.common.start')}
               />
               <Controller
                  control={control}
                  name="configuration.startTime"
                  render={({ field }) => (
                     <NativeIsoDateInput
                        disabled={!startTime}
                        id="meeting-time"
                        {...field}
                        min={currentTimeIso.current}
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
                           render={({ field: { onChange, value } }) => (
                              <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                           )}
                           control={control}
                           name="additionalFormData.enableSchedule"
                        />
                     }
                     label={t('dialog_create_conference.tabs.common.schedule')}
                  />
                  <TextField
                     style={{ flex: 1 }}
                     disabled={!scheduleCron}
                     placeholder="Cron"
                     error={!!errors.configuration?.scheduleCron}
                     {...wrapForInputRef(
                        register('configuration.scheduleCron', {
                           validate: validateSchedulerCron,
                        }),
                     )}
                     InputProps={{
                        endAdornment: (
                           <InputAdornment position="end">
                              <IconButton
                                 disabled={!scheduleCron}
                                 title={t('dialog_create_conference.tabs.common.open_cron_expression_generator')}
                                 href="https://www.freeformatter.com/cron-expression-generator-quartz.html"
                                 target="_blank"
                              >
                                 <ScheduleIcon />
                              </IconButton>
                           </InputAdornment>
                        ),
                     }}
                     aria-describedby="schedule-error-text"
                  />
               </Box>
               <Collapse in={scheduleCron}>
                  <div style={{ marginLeft: checkBoxWidth + 4 }}>
                     <Typography
                        id="schedule-error-text"
                        variant="caption"
                        color={errors.configuration?.scheduleCron ? 'error' : 'textSecondary'}
                     >
                        {errors.configuration?.scheduleCron?.message || cronDesc}
                     </Typography>
                  </div>
               </Collapse>
            </div>
         </Grid>
         <Grid item xs={12} className={classes.sectionGrid}>
            <Typography variant="h6">{t('glossary:chat')}</Typography>
            <Box>
               <FormControlLabel
                  control={
                     <Controller
                        render={({ field: { onChange, value } }) => (
                           <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                        )}
                        control={control}
                        name="configuration.chat.isGlobalChatEnabled"
                     />
                  }
                  label={t('glossary:all_chat')}
               />
               <FormControlLabel
                  control={
                     <Controller
                        render={({ field: { onChange, value } }) => (
                           <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                        )}
                        control={control}
                        name="configuration.chat.isRoomChatEnabled"
                     />
                  }
                  label={t('glossary:room_chat')}
               />
               <FormControlLabel
                  control={
                     <Controller
                        render={({ field: { onChange, value } }) => (
                           <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                        )}
                        control={control}
                        name="configuration.chat.isPrivateChatEnabled"
                     />
                  }
                  label={t('glossary:private_chat')}
               />
            </Box>
            <FormControlLabel
               control={
                  <Controller
                     render={({ field: { onChange, value } }) => (
                        <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                     )}
                     control={control}
                     name="configuration.chat.isDefaultRoomChatDisabled"
                  />
               }
               label={t('dialog_create_conference.tabs.common.disable_room_chat_master')}
            />
            <FormControlLabel
               control={
                  <Controller
                     render={({ field: { onChange, value } }) => (
                        <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                     )}
                     control={control}
                     name="configuration.chat.showTyping"
                  />
               }
               label={t('dialog_create_conference.tabs.common.show_participants_typing')}
            />
         </Grid>
         <Grid item xs={12} className={classes.sectionGrid}>
            <Typography variant="h6" gutterBottom>
               {t('glossary:scene_plural')}
            </Typography>
            <div className={classes.selectFormControl}>
               <FormControl fullWidth>
                  <InputLabel id="default-scene-select-label">
                     {t('dialog_create_conference.tabs.common.default_scene')}
                  </InputLabel>
                  <Controller
                     render={({ field: { onChange, value } }) => (
                        <Select
                           labelId="default-scene-select-label"
                           id="default-scene-select"
                           value={value}
                           onChange={onChange}
                        >
                           <MenuItem value="grid">{t('conference.scenes.grid')}</MenuItem>
                           <MenuItem value="activeSpeaker">{t('conference.scenes.active_speaker')}</MenuItem>
                        </Select>
                     )}
                     control={control}
                     name="configuration.scenes.defaultScene"
                  />
               </FormControl>
            </div>
            <FormControlLabel
               control={
                  <Controller
                     render={({ field: { onChange, value } }) => (
                        <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                     )}
                     control={control}
                     name="configuration.scenes.hideParticipantsWithoutWebcam"
                  />
               }
               label={t('dialog_create_conference.tabs.common.hide_participants_without_webcam')}
            />
            <FormControlLabel
               control={
                  <Controller
                     render={({ field: { onChange, value } }) => (
                        <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                     )}
                     control={control}
                     name="configuration.scenes.overlayScene"
                  />
               }
               label={t('dialog_create_conference.tabs.common.overlay_scene')}
            />
         </Grid>
      </Grid>
   );
}
