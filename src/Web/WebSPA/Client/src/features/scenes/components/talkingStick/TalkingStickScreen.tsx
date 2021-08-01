import { makeStyles, Typography } from '@material-ui/core';
import { motion } from 'framer-motion';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Size } from 'src/types';
import { TalkingStickMode } from '../../types';
import AutoSceneLayout from '../AutoSceneLayout';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      flexDirection: 'column',
   },
   headerContainer: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      justifyContent: 'flex-end',
      paddingBottom: theme.spacing(3),
   },
   centeredContent: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
   footer: {
      flex: 1,
   },
}));

type Props = {
   mode: TalkingStickMode;
   children: React.ReactNode;
   footerChildren?: React.ReactNode;
   className?: string;
   dimensions: Size;
};

export default function TalkingStickScreen({ children, className, mode, footerChildren, dimensions }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   return (
      <AutoSceneLayout className={className} {...dimensions} center>
         <div className={classes.root}>
            <motion.div initial={{ translateY: -48 }} animate={{ translateY: 0 }} className={classes.headerContainer}>
               <Typography variant="h2" align="center">
                  {t('conference.scenes.talking_stick')}
               </Typography>
               <Typography variant="subtitle1" align="center">
                  {t('conference.scenes.talking_stick_modes.mode_desc', {
                     name: t(`conference.scenes.talking_stick_modes.${mode}`),
                  })}
               </Typography>
            </motion.div>
            <motion.div initial={{ translateY: 48 }} animate={{ translateY: 0 }} className={classes.centeredContent}>
               {children}
            </motion.div>
            <div className={classes.footer}>{footerChildren}</div>
         </div>
      </AutoSceneLayout>
   );
}
