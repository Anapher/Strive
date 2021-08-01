import { Button, DialogActions, DialogContent, makeStyles, TextField } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import to from 'src/utils/to';
import { closeDialog } from '../reducer';

const useStyles = makeStyles((theme) => ({
   conferenceUrlField: {
      flex: 1,
      [theme.breakpoints.up('md')]: {
         marginRight: theme.spacing(2),
      },
      [theme.breakpoints.down('sm')]: {
         marginBottom: theme.spacing(1),
      },
   },
   conferenceUrlContainer: {
      display: 'flex',
      flexDirection: 'column',
      [theme.breakpoints.up('md')]: {
         alignItems: 'center',
         flexDirection: 'row',
      },
   },
}));

type Props = {
   conferenceId: string;
   showClose: boolean;
};

export default function ConferenceCreatedView({ conferenceId, showClose }: Props) {
   const classes = useStyles();
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const handleClose = () => dispatch(closeDialog());

   return (
      <DialogContent>
         <div className={classes.conferenceUrlContainer}>
            <TextField
               id="created-conference-url"
               variant="outlined"
               label={t('dialog_create_conference.created.conference_url')}
               InputProps={{ readOnly: true }}
               value={new URL('/c/' + conferenceId, document.baseURI).href}
               className={classes.conferenceUrlField}
            />
            <Button
               id="join-conference-button"
               variant="contained"
               {...to('/c/' + conferenceId)}
               onClick={handleClose}
               color="primary"
            >
               {t('dialog_create_conference.created.join')}
            </Button>
         </div>
         {showClose && (
            <DialogActions>
               <Button onClick={handleClose} color="primary">
                  {t('common:close')}
               </Button>
            </DialogActions>
         )}
      </DialogContent>
   );
}
