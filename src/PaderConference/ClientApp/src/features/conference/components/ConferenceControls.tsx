import { Fab, makeStyles } from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import NavigationIcon from '@material-ui/icons/Navigation';
import { RootState } from 'pader-conference';
import React from 'react';
import { connect } from 'react-redux';
import * as actions from '../actions';
import CreateConferenceDialog from './CreateConferenceDialog';

const useStyles = makeStyles((theme) => ({
   extendedIcon: {
      marginRight: theme.spacing(1),
   },
   fabMargin: {
      marginTop: theme.spacing(2),
   },
}));

const mapStateToProps = (state: RootState) => ({
   user: state.auth.token?.accessToken,
});

const dispatchProps = {
   openCreateDialog: actions.openCreateDialog,
};

type Props = ReturnType<typeof mapStateToProps> & typeof dispatchProps;

function ConferenceControls({ openCreateDialog }: Props) {
   const classes = useStyles();

   return (
      <>
         <Fab color="secondary" variant="extended" onClick={openCreateDialog}>
            <AddIcon className={classes.extendedIcon} />
            Start new conference
         </Fab>
         <Fab color="primary" variant="extended" className={classes.fabMargin}>
            <NavigationIcon className={classes.extendedIcon} />
            Join conference
         </Fab>

         <CreateConferenceDialog />
      </>
   );
}

export default connect(mapStateToProps, dispatchProps)(ConferenceControls);
