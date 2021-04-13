import { Fab, makeStyles } from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import { openDialogToCreateAsync } from '../reducer';
import CreateConferenceDialog from './CreateConferenceDialog';

const useStyles = makeStyles((theme) => ({
   extendedIcon: {
      marginRight: theme.spacing(1),
   },
   fabMargin: {
      marginTop: theme.spacing(2),
   },
}));

function ConferenceControls() {
   const classes = useStyles();
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const handleCreateConference = () => dispatch(openDialogToCreateAsync());

   return (
      <>
         <Fab color="secondary" variant="extended" onClick={handleCreateConference}>
            <AddIcon className={classes.extendedIcon} />
            {t('view_main.start_new_conference')}
         </Fab>

         <CreateConferenceDialog />
      </>
   );
}

export default ConferenceControls;
