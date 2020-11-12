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
            <motion.span
               variants={variants}
               initial="hidden"
               animate="shown"
               exit="hidden"
               style={{ display: 'inline-block' }}
            >
               {children}
            </motion.span>
         )}
      </AnimatePresence>
   );
}
