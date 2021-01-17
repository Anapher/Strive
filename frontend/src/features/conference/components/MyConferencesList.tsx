import { Chip, IconButton, List, ListItem, ListItemSecondaryAction, ListItemText, makeStyles } from '@material-ui/core';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import StarIcon from '@material-ui/icons/Star';
import StarBorder from '@material-ui/icons/StarBorder';
import _ from 'lodash';
import React from 'react';
import to from 'src/utils/to';
import { ConferenceLink } from '../types';

const useStyles = makeStyles(() => ({
   chipsRoot: {
      display: 'flex',
      '& > *': {
         cursor: 'pointer',
      },
   },
   chipActive: {
      borderColor: '#2ecc71',
   },
}));

type Props = {
   links: ConferenceLink[];
};

export default function MyConferencesList({ links }: Props) {
   const classes = useStyles();

   return (
      <List>
         {_.orderBy(links, [(x) => x.starred, (x) => x.lastJoin], ['asc', 'desc']).map((x) => (
            <ListItem key={x.conferenceId} button {...to(`/c/${x.conferenceId}`)}>
               <ListItemText
                  primary={x.conferenceName || 'Unnamed conference'}
                  secondary={
                     <div className={classes.chipsRoot}>
                        <Chip
                           className={x.isActive ? classes.chipActive : undefined}
                           size="small"
                           variant="outlined"
                           label={x.isActive ? 'Active' : 'Inactive'}
                        />
                        {x.isModerator && (
                           <Chip
                              size="small"
                              color="primary"
                              variant="outlined"
                              label="Moderator"
                              style={{ marginLeft: 8 }}
                           />
                        )}
                     </div>
                  }
               />
               <ListItemSecondaryAction>
                  <IconButton edge="start" aria-label="star">
                     {x.starred ? <StarIcon /> : <StarBorder />}
                  </IconButton>
                  <IconButton edge="end" aria-label="options">
                     <MoreVertIcon />
                  </IconButton>
               </ListItemSecondaryAction>
            </ListItem>
         ))}
      </List>
   );
}
