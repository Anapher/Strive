import { FormControl, InputLabel, makeStyles, MenuItem, Select } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
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
            <Select
               labelId="language-select-label"
               id="language-select"
               value={i18n.language}
               onChange={handleChangeLanguage}
            >
               {supportedLanguages.map(({ id, name }) => (
                  <MenuItem value={id} key={id}>
                     {name}
                  </MenuItem>
               ))}
            </Select>
         </FormControl>
      </div>
   );
}
