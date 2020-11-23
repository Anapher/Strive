import { createStyles, fade, InputBase, Theme, withStyles } from '@material-ui/core';

export default withStyles((theme: Theme) =>
   createStyles({
      root: {
         'label + &': {
            marginTop: theme.spacing(3),
         },
      },
      input: {
         color: theme.palette.text.secondary,
         borderRadius: theme.shape.borderRadius,
         position: 'relative',
         backgroundColor: theme.palette.background.paper,
         fontSize: 16,
         padding: '8px 16px 8px 0px',
         transition: theme.transitions.create(['background-color']),
         '&:hover': {
            backgroundColor: fade(theme.palette.text.primary, 0.2),
         },
      },
   }),
)(InputBase);
