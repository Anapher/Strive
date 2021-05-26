import { Box, Link, makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { Trans, useTranslation } from 'react-i18next';
import appSettings from 'src/config';

const gitInfo = appSettings.gitInfo;

const useStyles = makeStyles((theme) => ({
   root: {
      width: '100%',
      padding: theme.spacing(3),
      paddingTop: 0,
   },
}));

export default function About() {
   const { t } = useTranslation();
   const classes = useStyles();

   return (
      <div className={classes.root}>
         <Typography variant="h6" gutterBottom>
            {t('conference.settings.about.about_strive')}
         </Typography>
         <Typography>{t('conference.settings.about.strive_creator')}</Typography>
         <Typography>
            {t('conference.settings.about.strive_commit')}: {gitInfo.commit}{' '}
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
               <Link target="_blank" href="https://www.openstrive.org/">
                  Website
               </Link>
            </Box>
         </Box>
         <Box mt={4}>
            <Typography gutterBottom>
               {t('conference.settings.about.sound_effects_obtained_from')}{' '}
               <Link target="_blank" href="https://www.zapsplat.com">
                  zapsplat.com
               </Link>
            </Typography>
            <Typography>
               <Trans t={t} i18nKey="conference.settings.about.open_source_acknowledgements">
                  This project would not have been possible without all these awesome open source libraries. You can
                  find a list
                  <Link target="_blank" href="https://github.com/Anapher/Strive#acknowledgements">
                     here
                  </Link>
                  .
               </Trans>
            </Typography>
         </Box>
      </div>
   );
}
