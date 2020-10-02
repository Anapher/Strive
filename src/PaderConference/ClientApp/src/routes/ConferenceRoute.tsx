import React, { useEffect } from 'react';

export default function ConferenceRoute() {
   useEffect(() => {
      navigator.mediaDevices.enumerateDevices().then((x) => console.log(x));
   }, []);
   return <div>Hello to this conference</div>;
}
