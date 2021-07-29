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
   TextField,
   Typography,
} from '@material-ui/core';
import ScheduleIcon from '@material-ui/icons/Schedule';
import cronstrue from 'cronstrue/i18n';
import { DateTime } from 'luxon';
import React, { useEffect, useRef, useState } from 'react';
import { Controller, UseFormReturn } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import MobileAwareSelect from 'src/components/MobileAwareSelect';
import NativeIsoDateInput from 'src/components/NativeIsoDateInput';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import { ConferenceDataForm } from '../form';
import SceneLayoutSelect from './SceneLayoutSelect';

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
      maxWidth: 248,
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
                        <MobileAwareSelect
                           labelId="default-scene-select-label"
                           id="default-scene-select"
                           value={value}
                           onChange={onChange}
                        >
                           {[
                              { value: 'grid', label: t<string>('conference.scenes.grid') },
                              { value: 'activeSpeaker', label: t<string>('conference.scenes.active_speaker') },
                           ]}
                        </MobileAwareSelect>
                     )}
                     control={control}
                     name="configuration.scenes.defaultScene"
                  />
               </FormControl>
            </div>
         </Grid>
         <Grid item xs={12} md={6}>
            <div className={classes.selectFormControl}>
               <FormControl fullWidth>
                  <InputLabel id="scene-layout-select-label">
                     {t('dialog_create_conference.tabs.common.scene_layout')}
                  </InputLabel>
                  <Controller
                     render={({ field: { onChange, value } }) => (
                        <SceneLayoutSelect
                           labelId="scene-layout-select-label"
                           id="scene-layout-select"
                           value={value}
                           onChange={onChange}
                        />
                     )}
                     control={control}
                     name="configuration.scenes.sceneLayout"
                  />
               </FormControl>
            </div>
         </Grid>
         <Grid item xs={12} md={6}>
            <div className={classes.selectFormControl}>
               <FormControl fullWidth>
                  <InputLabel id="scene-layout-screenshare-select-label">
                     {t('dialog_create_conference.tabs.common.scene_layout_screen_share')}
                  </InputLabel>
                  <Controller
                     render={({ field: { onChange, value } }) => (
                        <SceneLayoutSelect
                           labelId="scene-layout-screenshare-select-label"
                           id="scene-layout-screenshare-select"
                           value={value}
                           onChange={onChange}
                        />
                     )}
                     control={control}
                     name="configuration.scenes.screenShareLayout"
                  />
               </FormControl>
            </div>
         </Grid>
         <Grid item xs={12}>
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
         </Grid>
      </Grid>
   );
}
