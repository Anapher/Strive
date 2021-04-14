import { Accordion, AccordionDetails, AccordionSummary, Button, makeStyles, Typography } from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import StopIcon from '@material-ui/icons/Stop';
import React from 'react';
import { useTranslation } from 'react-i18next';
import testAudioFile from 'src/assets/audio/test_audio_file.mp3';
import useSound from 'use-sound';

const useStyles = makeStyles((theme) => ({
   heading: {
      fontSize: theme.typography.pxToRem(15),
      flex: 1,
   },
   playButton: {
      minWidth: 160,
      marginRight: theme.spacing(2),
   },
   accordionSummary: {
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'center',
      flex: 1,
   },
}));

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
};

export default function TroubleshootSpeakers({ expanded, onChange }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();
   const [play, { isPlaying, stop }] = useSound(testAudioFile, { volume: 0.75 });

   const handleChange = (_: React.ChangeEvent<unknown>, isExpanded: boolean) => {
      onChange(isExpanded);
   };

   const handleToggleAudio = (event: React.MouseEvent) => {
      if (isPlaying) stop();
      else play();

      event.stopPropagation();
   };

   return (
      <Accordion expanded={expanded} onChange={handleChange}>
         <AccordionSummary
            expandIcon={<ExpandMoreIcon />}
            aria-controls="troubleshoot-speakers-content"
            id="troubleshoot-speakers-header"
         >
            <div className={classes.accordionSummary}>
               <Typography className={classes.heading}>
                  {t('conference.media.troubleshooting.speakers.title')}
               </Typography>
               <Button
                  size="small"
                  variant="contained"
                  onClick={handleToggleAudio}
                  onFocus={(event) => event.stopPropagation()}
                  className={classes.playButton}
               >
                  {isPlaying ? <StopIcon fontSize="small" /> : <PlayArrowIcon fontSize="small" />}
                  {isPlaying
                     ? t('conference.media.troubleshooting.speakers.button_stop')
                     : t('conference.media.troubleshooting.speakers.button_play')}
               </Button>
            </div>
         </AccordionSummary>
         <AccordionDetails>
            <Typography gutterBottom>{t('conference.media.troubleshooting.speakers.desc')}</Typography>
         </AccordionDetails>
      </Accordion>
   );
}
