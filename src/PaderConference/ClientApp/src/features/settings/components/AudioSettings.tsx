import { makeStyles, Mark, Slider, Typography } from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { setAudioGain } from '../settingsSlice';

const useStyles = makeStyles((theme) => ({
   root: {
      width: '100%',
      padding: theme.spacing(3),
   },
   slider: {
      padding: theme.spacing(0, 2),
   },
}));

const marks: Mark[] = [
   { value: 1, label: '0%' },
   { value: 0, label: '-100%' },
   { value: 2, label: '100%' },
   { value: 3, label: '200%' },
];

export default function AudioSettings() {
   const classes = useStyles();
   const audioGain = useSelector((state: RootState) => state.settings.obj.audioGain);
   const dispatch = useDispatch();

   const handleChangeGain = (_: React.ChangeEvent<unknown>, value: number | number[]) => {
      dispatch(setAudioGain(value as number));
   };

   return (
      <div className={classes.root}>
         <Typography variant="h6" gutterBottom>
            Gain
         </Typography>
         <div className={classes.slider}>
            <Slider
               value={audioGain}
               min={0}
               max={3}
               onChange={handleChangeGain}
               step={0.2}
               valueLabelDisplay="auto"
               marks={marks}
               valueLabelFormat={(val) => `${Math.round((val - 1) * 100)}%`}
            />
         </div>
      </div>
   );
}