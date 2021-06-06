import { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { userInteractionMade } from '../reducer';

export default function UserInteractionListener() {
   const dispatch = useDispatch();

   useEffect(() => {
      const events = ['scroll', 'keydown', 'click', 'touchstart'];

      const handleUserInteraction = () => {
         dispatch(userInteractionMade());

         for (const event of events) {
            document.body.removeEventListener(event, handleUserInteraction);
         }
      };

      for (const event of events) {
         document.body.addEventListener(event, handleUserInteraction, { once: true });
      }
   }, []);

   return null;
}
