import { ButtonBase, makeStyles, Typography } from '@material-ui/core';
import StarIcon from '@material-ui/icons/Star';
import clsx from 'classnames';
import React from 'react';
import { RoomViewModel } from '../types';
import { fade } from '@material-ui/core/styles';
import PeopleIcon from '@material-ui/icons/People';
const useStyles = makeStyles((theme) => ({
   root: {
      borderRadius: theme.shape.borderRadius,
      padding: theme.spacing(0, 2),
      paddingTop: 6,
      paddingBottom: 6,
      width: '100%',
      display: 'flex',
      flexDirection: 'row',
      justifyContent: 'flex-start',
      transition: theme.transitions.create('background-color'),
      '&:hover': {
         textDecoration: 'none',
         backgroundColor: fade(theme.palette.text.primary, 0.05),
      },
      '&$selected': {
         color: theme.palette.action.active,
         backgroundColor: fade(theme.palette.action.active, 0.12),
         '&:hover': {
            backgroundColor: fade(theme.palette.action.active, 0.15),
         },
         '& + &': {
            borderLeft: 0,
            marginLeft: 0,
         },
      },
   },
   /* Pseudo-class applied to the root element if `selected={true}`. */
   selected: {},
   rootSelected: {
      backgroundColor: fade(theme.palette.text.primary, 0.1),
   },
   icon: {
      marginRight: 8,
   },
   starIcon: {
      marginRight: 8,
      color: '#f1c40f',
   },
   nameContainer: {
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'center',
   },
}));

type Props = {
   room: RoomViewModel;
   selected?: boolean;
   onClick: () => void;
   className?: string;
};

export default function RoomHeader({ room: { displayName, isDefaultRoom }, selected, onClick, className }: Props) {
   const classes = useStyles();
   return (
      <ButtonBase
         className={clsx([classes.root, className, { [classes.selected]: selected }])}
         onClick={selected ? undefined : onClick}
         aria-pressed={selected}
      >
         <div className={classes.nameContainer}>
            {isDefaultRoom ? (
               <StarIcon fontSize="small" className={classes.starIcon} />
            ) : (
               <PeopleIcon fontSize="small" className={classes.icon} />
            )}
            <Typography variant="subtitle2">{displayName}</Typography>
         </div>
      </ButtonBase>
   );
}
