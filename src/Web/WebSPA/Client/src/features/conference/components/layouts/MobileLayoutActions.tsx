import {
   BottomNavigation,
   BottomNavigationAction,
   ButtonBase,
   IconButton,
   makeStyles,
   Typography,
} from '@material-ui/core';
import React from 'react';
import AnimatedCamIcon from 'src/assets/animated-icons/AnimatedCamIcon';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import AnimatedScreenIcon from 'src/assets/animated-icons/AnimatedScreenIcon';
import QuestionAnswerIcon from '@material-ui/icons/QuestionAnswer';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import PollIcon from '@material-ui/icons/Poll';
import Crop32Icon from '@material-ui/icons/Crop32';
import GroupWorkIcon from '@material-ui/icons/GroupWork';

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
}));

type Props = {
   selectedTab: number;
   setSelectedTab: (value: number) => void;
};

export default function MobileLayoutActions({ selectedTab, setSelectedTab }: Props) {
   const classes = useStyles();

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
            <BottomNavigationAction label="Main" icon={<Crop32Icon />} />
            <BottomNavigationAction label="Chat" icon={<QuestionAnswerIcon />} />
            <BottomNavigationAction label="Rooms" icon={<GroupWorkIcon />} />
         </BottomNavigation>
         <div className={classes.primaryControlsContainer}>
            <ButtonBase className={classes.primaryControl}>
               <AnimatedMicIcon width={24} height={24} />
            </ButtonBase>
            <IconButton>
               <MoreVertIcon />
            </IconButton>
         </div>
      </div>
   );
}
