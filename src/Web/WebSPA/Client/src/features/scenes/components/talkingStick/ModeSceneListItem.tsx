import { Collapse, List, ListItem, ListItemIcon, ListItemText, makeStyles } from '@material-ui/core';
import ExpandLess from '@material-ui/icons/ExpandLess';
import ExpandMore from '@material-ui/icons/ExpandMore';
import { AccountArrowRight, AccountConvert, AutoFix, HumanQueue, RunFast } from 'mdi-material-ui';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { ModeSceneListItemProps, TalkingStickMode } from '../../types';

const useStyles = makeStyles((theme) => ({
   nested: {
      paddingLeft: theme.spacing(4),
   },
}));

export default function ModeSceneListItem({ selectedScene, onChangeScene }: ModeSceneListItemProps) {
   const classes = useStyles();
   const { t } = useTranslation();

   const isSelected = selectedScene.type === 'talkingStick';
   const handleSetScene = (mode: TalkingStickMode) => () => onChangeScene({ type: 'talkingStick', mode });

   const [open, setOpen] = React.useState(false);

   const handleClick = () => {
      setOpen(!open);
   };

   return (
      <>
         <ListItem button selected={isSelected} onClick={handleClick}>
            <ListItemIcon>
               <AccountConvert />
            </ListItemIcon>
            <ListItemText primary={t('conference.scenes.talking_stick')} />
            {open ? <ExpandLess /> : <ExpandMore />}
         </ListItem>
         <Collapse in={open} timeout="auto" unmountOnExit>
            <List component="div" disablePadding>
               <ListItem button className={classes.nested} onClick={handleSetScene('queue')}>
                  <ListItemIcon>
                     <HumanQueue />
                  </ListItemIcon>
                  <ListItemText
                     primary={t('conference.scenes.talking_stick_modes.queue')}
                     secondary={t('conference.scenes.talking_stick_modes.queue_description')}
                  />
               </ListItem>
               <ListItem button className={classes.nested} onClick={handleSetScene('race')}>
                  <ListItemIcon>
                     <RunFast />
                  </ListItemIcon>
                  <ListItemText
                     primary={t('conference.scenes.talking_stick_modes.race')}
                     secondary={t('conference.scenes.talking_stick_modes.race_description')}
                  />
               </ListItem>
               <ListItem button className={classes.nested} onClick={handleSetScene('moderated')}>
                  <ListItemIcon>
                     <AutoFix />
                  </ListItemIcon>
                  <ListItemText
                     primary={t('conference.scenes.talking_stick_modes.moderated')}
                     secondary={t('conference.scenes.talking_stick_modes.moderated_description')}
                  />
               </ListItem>
               <ListItem button className={classes.nested} onClick={handleSetScene('speakerPassStick')}>
                  <ListItemIcon>
                     <AccountArrowRight />
                  </ListItemIcon>
                  <ListItemText
                     primary={t('conference.scenes.talking_stick_modes.speakerPassStick')}
                     secondary={t('conference.scenes.talking_stick_modes.speakerPassStick_description')}
                  />
               </ListItem>
            </List>
         </Collapse>
      </>
   );
}
