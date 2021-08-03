import { Accordion, AccordionDetails, AccordionSummary, Button, makeStyles, Typography } from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import StopIcon from '@material-ui/icons/Stop';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import useStriveSound from 'src/hooks/useStriveSound';

const useStyles = makeStyles((theme) => ({
   heading: {
      fontSize: theme.typography.pxToRem(15),
      flex: 1,
      marginRight: theme.spacing(1),
   },
   playButton: {
      [theme.breakpoints.up('md')]: {
         minWidth: 160,
      },
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
   className?: string;
};

export default function TroubleshootSpeakers({ expanded, onChange, className }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   const [isPlaying, setIsPlaying] = useState(false);
   const [play, { stop }] = useStriveSound('testAudioFile', {
      onplay: () => setIsPlaying(true),
      onend: () => setIsPlaying(false),
   });

   const handleChange = (_: React.ChangeEvent<unknown>, isExpanded: boolean) => {
      onChange(isExpanded);
   };

   const handleToggleAudio = (event: React.MouseEvent) => {
      if (isPlaying) {
         stop();
         setIsPlaying(false);
      } else {
         play();
      }

      event.stopPropagation();
   };

   useEffect(() => {
      return () => {
         stop();
      };
   }, [stop]);

   return (
      <Accordion expanded={expanded} onChange={handleChange} className={className}>
         <AccordionSummary
            expandIcon={<ExpandMoreIcon />}
            aria-controls="troubleshoot-speakers-content"
            id="troubleshoot-speakers-header"
         >
            <div className={classes.accordionSummary}>
               <Typography className={classes.heading}>{t('conference.troubleshooting.speakers.title')}</Typography>
               <Button
                  size="small"
                  variant="contained"
                  onClick={handleToggleAudio}
                  onFocus={(event) => event.stopPropagation()}
                  className={classes.playButton}
               >
                  {isPlaying ? <StopIcon fontSize="small" /> : <PlayArrowIcon fontSize="small" />}
                  {isPlaying
                     ? t('conference.troubleshooting.speakers.button_stop')
                     : t('conference.troubleshooting.speakers.button_play')}
               </Button>
            </div>
         </AccordionSummary>
         <AccordionDetails>
            <Typography gutterBottom>{t('conference.troubleshooting.speakers.desc')}</Typography>
         </AccordionDetails>
      </Accordion>
   );
}
