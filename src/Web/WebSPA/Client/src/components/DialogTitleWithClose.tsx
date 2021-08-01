import { IconButton, makeStyles } from '@material-ui/core';
import DialogTitle, { DialogTitleProps } from '@material-ui/core/DialogTitle/DialogTitle';
import CloseIcon from '@material-ui/icons/Close';
import React from 'react';

const useStyles = makeStyles((theme) => ({
   content: {
      display: 'flex',
      justifyContent: 'space-between',
   },
   closeButton: {
      position: 'absolute',
      right: theme.spacing(1),
      top: theme.spacing(1),
      color: theme.palette.grey[500],
   },
}));

type Props = DialogTitleProps & { onClose: () => void; showClose?: boolean };

export default function DialogTitleWithClose({ onClose, children, showClose = true, ...props }: Props) {
   const classes = useStyles();

   return (
      <DialogTitle {...props}>
         <div className={classes.content}>
            {children}
            {showClose && (
               <IconButton onClick={onClose} aria-label="close" className={classes.closeButton}>
                  <CloseIcon />
               </IconButton>
            )}
         </div>
      </DialogTitle>
   );
}
