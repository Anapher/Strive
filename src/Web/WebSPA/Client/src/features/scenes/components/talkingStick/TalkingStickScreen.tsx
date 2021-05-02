import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import clsx from 'classnames';
import { useTranslation } from 'react-i18next';
import { motion } from 'framer-motion';
import { TalkingStickMode } from '../../types';
import ActiveChipsLayout from '../ActiveChipsLayout';

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
};

export default function TalkingStickScreen({ children, className, mode, footerChildren }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   return (
      <ActiveChipsLayout className={className} contentClassName={classes.root}>
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
      </ActiveChipsLayout>
   );
}
