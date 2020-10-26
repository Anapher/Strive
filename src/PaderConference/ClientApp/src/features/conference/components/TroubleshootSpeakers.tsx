import { Accordion, AccordionDetails, AccordionSummary, Button, makeStyles, Typography } from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import StopIcon from '@material-ui/icons/Stop';
import React from 'react';
import testAudioFile from 'src/assets/audio/test_audio_file.mp3';
import useSound from 'use-sound';

const useStyles = makeStyles((theme) => ({
   heading: {
      fontSize: theme.typography.pxToRem(15),
      flex: 1,
   },
   playButton: {
      width: 160,
      marginRight: theme.spacing(2),
   },
}));

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
};

export default function TroubleshootSpeakers({ expanded, onChange }: Props) {
   const classes = useStyles();
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
            <Typography className={classes.heading}>Speakers</Typography>
            <Button
               size="small"
               variant="contained"
               onClick={handleToggleAudio}
               onFocus={(event) => event.stopPropagation()}
               className={classes.playButton}
            >
               {isPlaying ? <StopIcon fontSize="small" /> : <PlayArrowIcon fontSize="small" />}
               {isPlaying ? 'Stop' : 'Play test audio'}
            </Button>
         </AccordionSummary>
         <AccordionDetails>
            <Typography gutterBottom>
               To check if your speakers are working, please play the test sound. If you cannot hear anything while the
               sound is playing, please check your sound output in the settings of your operating system.
            </Typography>
         </AccordionDetails>
      </Accordion>
   );
}
