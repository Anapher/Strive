import { AnimatePresence, motion } from 'framer-motion';
import React from 'react';

type Props = {
   hidden?: boolean;
   children?: React.ReactNode;
};

const variants = {
   shown: {
      opacity: 1,
      scale: 1,
   },
   hidden: {
      opacity: 0,
      scale: 0,
   },
};

export default function IconHide({ hidden, children }: Props) {
   return (
      <AnimatePresence>
         {!hidden && (
            <motion.div variants={variants} initial="hidden" animate="shown" exit="hidden">
               {children}
            </motion.div>
         )}
      </AnimatePresence>
   );
}
