import React from 'react';
import BaseAuthComponent from './BaseAuthComponent';

export default function AuthCallback() {
   return <BaseAuthComponent title="Authentication completed" text="You will now be redirected to our application." />;
}
