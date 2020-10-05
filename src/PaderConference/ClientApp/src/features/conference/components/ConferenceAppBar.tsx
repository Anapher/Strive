import { IconButton, Toolbar, Typography, AppBar, makeStyles, createStyles, Button } from '@material-ui/core';
import React from 'react';
import MenuIcon from '@material-ui/icons/Menu';
import { useDispatch } from 'react-redux';
import { signOut } from 'src/features/auth/authSlice';

const useStyles = makeStyles((theme) =>
   createStyles({
      root: {
         flexGrow: 1,
      },
      menuButton: {
         marginRight: theme.spacing(2),
      },
      title: {
         flexGrow: 1,
      },
   }),
);

export default function ConferenceAppBar() {
   const classes = useStyles();
   const dispatch = useDispatch();
   const handleSignOut = () => dispatch(signOut());

   return (
      <AppBar position="static">
         <Toolbar>
            <IconButton edge="start" className={classes.menuButton} color="inherit" aria-label="menu">
               <MenuIcon />
            </IconButton>
            <Typography variant="h6" className={classes.title}>
               PaderConference
            </Typography>
            <Button onClick={handleSignOut}>Sign out</Button>
         </Toolbar>
      </AppBar>
   );
}
