import { Grid, IconButton, makeStyles, Typography } from '@material-ui/core';

const useStyles = makeStyles({
   container: {
      width: 200,
   },
   emojiIcon: {
      fontSize: 18,
   },
});

type Props = {
   onEmojiSelected: (s: string) => void;
   onClose: () => void;
};

const emojis: string[] = ['👍', '👎', '🎉', '😂', '😭', '❤️', '🔥', '🤔', '😫', '🙄', '🚀', '👀'];

export default function EmojisPopper({ onEmojiSelected }: Props) {
   const classes = useStyles();
   return (
      <Grid container className={classes.container}>
         {emojis.map((x) => (
            <Grid item xs={3} key={x}>
               <IconButton onClick={() => onEmojiSelected(x)} aria-label={`Insert emoji ${x}`}>
                  <Typography className={classes.emojiIcon}>{x}</Typography>
               </IconButton>
            </Grid>
         ))}
      </Grid>
   );
}
