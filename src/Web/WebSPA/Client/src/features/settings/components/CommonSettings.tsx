import { Box, FormControl, InputLabel, Link, makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import MobileAwareSelect from 'src/components/MobileAwareSelect';
import appSettings from 'src/config';
import { supportedLanguages } from 'src/services/i18n';

const useStyles = makeStyles((theme) => ({
   root: {
      width: '100%',
      padding: theme.spacing(3),
      paddingTop: 0,
   },
   languageSelect: {
      maxWidth: 240,
   },
}));

const gitInfo = appSettings.gitInfo;

export default function CommonSettings() {
   const classes = useStyles();
   const { t, i18n } = useTranslation();

   const handleChangeLanguage = (event: React.ChangeEvent<{ value: unknown }>) => {
      const lang = event.target.value as string;
      i18n.changeLanguage(lang);
      localStorage.setItem('i18nextLng', lang);
   };

   return (
      <div className={classes.root}>
         <FormControl className={classes.languageSelect} fullWidth>
            <InputLabel id="language-select-label">{t('common:language')}</InputLabel>
            <MobileAwareSelect
               labelId="language-select-label"
               id="language-select"
               value={i18n.languages.find((x) => supportedLanguages.find((y) => y.id === x))}
               onChange={handleChangeLanguage}
            >
               {supportedLanguages.map(({ id, name }) => ({ value: id, label: name }))}
            </MobileAwareSelect>
         </FormControl>

         <Box mt={4}>
            <Typography variant="subtitle1">{t('conference.settings.common.strive_info')}</Typography>
            <Typography>{t('conference.settings.common.strive_creator')}</Typography>
            <Typography>
               {t('conference.settings.common.strive_commit')}: {gitInfo.commit}{' '}
               <Typography color="textSecondary" component="span">
                  ({gitInfo.ref} at {gitInfo.timestamp})
               </Typography>
            </Typography>
            <Box display="flex" mt={2}>
               <Box>
                  <Link target="_blank" href="https://github.com/Anapher/Strive">
                     GitHub
                  </Link>
               </Box>
               <Box ml={1}>
                  <Link target="_blank" href="http://openstrive.org/">
                     Website
                  </Link>
               </Box>
            </Box>
         </Box>
      </div>
   );
}
