import {
   BottomNavigation,
   BottomNavigationAction,
   ButtonBase,
   Divider,
   makeStyles,
   Tooltip,
   useTheme,
} from '@material-ui/core';
import Crop32Icon from '@material-ui/icons/Crop32';
import GroupWorkIcon from '@material-ui/icons/GroupWork';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import QuestionAnswerIcon from '@material-ui/icons/QuestionAnswer';
import React, { useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import { useMicrophoneControl } from 'src/features/media/useDeviceControl';
import useMediaButton from 'src/features/media/useMediaButton';
import usePermission from 'src/hooks/usePermission';
import { MEDIA_CAN_SHARE_AUDIO } from 'src/permissions';
import { showMessage } from 'src/store/notifier/actions';
import MobileLayoutActionsMenu from './MobileLayoutActionsMenu';

const useStyles = makeStyles((theme) => ({
   root: {
      backgroundColor: theme.palette.background.paper,
      display: 'flex',
      flexDirection: 'row',
      width: '100%',
   },
   primaryControl: {
      borderRadius: '16px 0px 0px 16px',
      padding: theme.spacing(0, 3),
   },
   secondaryControls: {
      display: 'flex',
      flexDirection: 'row',
   },
   primaryControlsContainer: {
      display: 'flex',
      flexDirection: 'row',
      borderRadius: '16px 0px 0px 16px',
      backgroundColor: theme.palette.grey[800],
   },
   moreControl: {
      padding: theme.spacing(0, 1),
   },
}));

type Props = {
   selectedTab: number;
   setSelectedTab: (value: number) => void;
};

export default function MobileLayoutActions({ selectedTab, setSelectedTab }: Props) {
   const classes = useStyles();
   const theme = useTheme();
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const moreIconButtonRef = useRef<HTMLButtonElement>(null);
   const [isMenuOpen, setIsMenuOpen] = useState(false);

   const handleOpenMenu = () => setIsMenuOpen(true);
   const handleCloseMenu = () => setIsMenuOpen(false);

   const { available, controller } = useMicrophoneControl();
   const { activated, id, handleClick, label, title } = useMediaButton(true, controller, 'mic');
   const canShareAudio = usePermission(MEDIA_CAN_SHARE_AUDIO);

   const audioDisabled = !available || !canShareAudio;

   const handleClickDisabled = () => {
      if (!available) {
         dispatch(showMessage({ message: t('conference.media.mic_not_available'), type: 'error' }));
      } else if (!canShareAudio) {
         dispatch(showMessage({ message: t('conference.media.mic_permission_denied'), type: 'error' }));
      }
   };

   return (
      <div className={classes.root}>
         <BottomNavigation
            value={selectedTab}
            onChange={(_, newValue) => {
               setSelectedTab(newValue);
            }}
            style={{ flex: 1 }}
            showLabels
         >
            <BottomNavigationAction label={t('conference.appbar.conference')} icon={<Crop32Icon />} />
            <BottomNavigationAction label={t('conference.appbar.chat')} icon={<QuestionAnswerIcon />} />
            <BottomNavigationAction label={t('conference.appbar.rooms')} icon={<GroupWorkIcon />} />
         </BottomNavigation>
         <div className={classes.primaryControlsContainer}>
            <Tooltip title={title} aria-label={label} arrow>
               <ButtonBase
                  className={classes.primaryControl}
                  onClick={audioDisabled ? handleClickDisabled : handleClick}
                  id={id}
               >
                  <AnimatedMicIcon
                     width={24}
                     height={24}
                     color={audioDisabled ? theme.palette.text.disabled : theme.palette.text.primary}
                     activated={activated}
                  />
               </ButtonBase>
            </Tooltip>
            <Divider orientation="vertical" />
            <Tooltip title={t<string>('conference.appbar.menu_more_options')}>
               <ButtonBase className={classes.moreControl} ref={moreIconButtonRef} onClick={handleOpenMenu}>
                  <MoreVertIcon />
               </ButtonBase>
            </Tooltip>

            <MobileLayoutActionsMenu open={isMenuOpen} onClose={handleCloseMenu} anchorEl={moreIconButtonRef.current} />
         </div>
      </div>
   );
}
