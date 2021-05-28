import {
   Avatar,
   Divider,
   IconButton,
   List,
   ListItem,
   ListItemAvatar,
   ListItemIcon,
   ListItemSecondaryAction,
   ListItemText,
   ListSubheader,
   makeStyles,
} from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import DeleteIcon from '@material-ui/icons/Delete';
import PersonIcon from '@material-ui/icons/Person';
import { Skeleton } from '@material-ui/lab';
import React, { useEffect } from 'react';
import { Controller, ControllerRenderProps, UseFormReturn } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { useParams } from 'react-router-dom';
import { selectParticipantList } from 'src/features/conference/selectors';
import { ConferenceRouteParams } from 'src/routes/types';
import { RootState } from 'src/store';
import { ConferenceDataForm } from '../form';
import { loadUserInfo } from '../reducer';

const useStyles = makeStyles({
   list: {
      height: '100%',
      minHeight: 0,
      overflowY: 'auto',
   },
});

type Props = {
   form: UseFormReturn<ConferenceDataForm>;
   conferenceId: string | null;
};

export default function TabModerators({ form: { control }, conferenceId }: Props) {
   return (
      <Controller
         control={control}
         name="configuration.moderators"
         render={({ field }) => <ModeratorList field={field} conferenceId={conferenceId} />}
      />
   );
}

type ModeratorListProps = {
   field: ControllerRenderProps<ConferenceDataForm, 'configuration.moderators'>;
   conferenceId: string | null;
};

function ModeratorList({ field: { value, onChange }, conferenceId }: ModeratorListProps) {
   const { t } = useTranslation();
   const dispatch = useDispatch();
   const classes = useStyles();

   const userInfos = useSelector((state: RootState) => state.createConference.userInfo);

   const { id: currentConferenceId } = useParams<ConferenceRouteParams>();
   const participants = useSelector(selectParticipantList).filter((x) => !value.includes(x.id));

   useEffect(() => {
      dispatch(loadUserInfo(value));
   }, [JSON.stringify(value)]);

   const handeDeleteUser = (id: string) => {
      onChange(value.filter((x) => x !== id));
   };

   const handeAddUser = (id: string) => () => {
      onChange([...value, id]);
   };

   return (
      <List className={classes.list} id="create-conference-form-moderators-list">
         {value.map((id) => {
            const info = userInfos.find((x) => x.id === id);
            if (!info)
               return (
                  <ListItem key={id}>
                     <ListItemAvatar>
                        <Skeleton variant="circle">
                           <Avatar />
                        </Skeleton>
                     </ListItemAvatar>
                     <ListItemText primary={<Skeleton variant="text" />} />
                  </ListItem>
               );

            return (
               <ListItem key={id}>
                  <ListItemAvatar>
                     <Avatar>
                        <PersonIcon />
                     </Avatar>
                  </ListItemAvatar>
                  <ListItemText
                     primaryTypographyProps={{ color: info.notFound ? 'error' : undefined }}
                     primary={
                        info.notFound
                           ? t('dialog_create_conference.tabs.moderators.error_user_not_found', { id: info.id })
                           : info.displayName
                     }
                  />
                  <ListItemSecondaryAction>
                     <IconButton edge="end" aria-label="delete" onClick={() => handeDeleteUser(id)}>
                        <DeleteIcon />
                     </IconButton>
                  </ListItemSecondaryAction>
               </ListItem>
            );
         })}
         {conferenceId === currentConferenceId && participants.length > 0 && (
            <>
               <Divider />
               <ListSubheader disableSticky>
                  {t('dialog_create_conference.tabs.moderators.add_from_conference')}
               </ListSubheader>
               {participants.map(({ id, displayName }) => (
                  <ListItem key={id} button dense onClick={handeAddUser(id)}>
                     <ListItemIcon>
                        <AddIcon />
                     </ListItemIcon>
                     <ListItemText primary={displayName} />
                  </ListItem>
               ))}
            </>
         )}
      </List>
   );
}
