import { Box, Button, Checkbox, Container, FormControlLabel, IconButton, makeStyles } from '@material-ui/core';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import { openSettings } from 'src/features/settings/reducer';
import to from 'src/utils/to';

const useStyles = makeStyles((theme) => ({
   root: {
      height: '100%',
      position: 'relative',
      display: 'flex',
      flexDirection: 'column',
   },
   container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      flex: 1,
   },
   settingsContainer: {
      right: theme.spacing(4),
      top: theme.spacing(4),
      position: 'absolute',

      [theme.breakpoints.down('md')]: {
         right: theme.spacing(2),
         top: theme.spacing(2),
      },
   },
   backButtonContainer: {
      left: theme.spacing(4),
      top: theme.spacing(4),
      position: 'absolute',

      [theme.breakpoints.down('md')]: {
         left: theme.spacing(2),
         top: theme.spacing(2),
      },
   },
}));

type Props = {
   children: React.ReactNode;
};

export default function ConferenceNotOpenLayout({ children }: Props) {
   const classes = useStyles();
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const handleOpenSettings = () => dispatch(openSettings());

   return (
      <div className={classes.root}>
         <div className={classes.settingsContainer}>
            <Button onClick={handleOpenSettings}>{t('common:settings')}</Button>
         </div>
         <div className={classes.backButtonContainer}>
            <IconButton {...to('/')} aria-label={t('common:back_to_start')}>
               <ArrowBackIcon />
            </IconButton>
         </div>
         <Box display="flex" flexDirection="row" position="absolute" left={32} bottom={32}>
            <FormControlLabel
               control={<Checkbox checked={true} />}
               label={t('conference_not_open.play_sound_on_conference_open')}
            />
         </Box>
         <Container maxWidth="md" className={classes.container}>
            {children as any}
         </Container>
      </div>
   );
}
