import { makeStyles, Typography } from '@material-ui/core';
import { AnimatePresence, motion } from 'framer-motion';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { selectParticipant } from 'src/features/conference/selectors';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { RootState } from 'src/store';
import { PresenterScene, RenderSceneProps } from '../../types';

const useStyles = makeStyles((theme) => ({
   overlay: {
      position: 'absolute',
      top: 0,
      left: 0,
      bottom: 0,
      right: 0,
      backgroundColor: theme.palette.background.default,
      height: '100%',
      overflow: 'hidden',
   },
   presenterTextContainer: {
      width: '100%',
      height: '100%',

      display: 'flex',
      flexDirection: 'column',
      justifyContent: 'center',
   },
}));

export default function RenderPresenter({ next, scene, className }: RenderSceneProps<PresenterScene>) {
   const classes = useStyles();
   const { t } = useTranslation();

   const myId = useMyParticipantId();
   const [showOverlay, setShowOverlay] = useState(true);

   const isPresenter = scene.presenterParticipantId === myId;

   const presenter = useSelector((state: RootState) => selectParticipant(state, scene.presenterParticipantId));

   useEffect(() => {
      setShowOverlay(true);
      const timeout = setTimeout(() => setShowOverlay(false), 2500);
      return () => clearTimeout(timeout);
   }, [scene.presenterParticipantId]);

   return (
      <div className={className}>
         {next({ appliedShowMediaControls: isPresenter })}
         <AnimatePresence>
            {showOverlay && (
               <motion.div
                  initial={{ opacity: 1 }}
                  animate={{ opacity: 1 }}
                  exit={{ opacity: 0 }}
                  className={classes.overlay}
               >
                  <motion.div
                     initial={{ translateY: 400, opacity: 0 }}
                     animate={{ translateY: 0, opacity: 1 }}
                     className={classes.presenterTextContainer}
                     transition={{
                        type: 'spring',
                        damping: 15,
                        stiffness: 60,
                     }}
                  >
                     <Typography variant="h5" align="center">
                        {t('conference.scenes.presenter_text', { name: presenter?.displayName })}
                     </Typography>
                  </motion.div>
               </motion.div>
            )}
         </AnimatePresence>
      </div>
   );
}
