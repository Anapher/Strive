import { CountdownRenderProps } from 'react-countdown';

export default function CountdownRenderer({ hours, formatted }: CountdownRenderProps) {
   let result = '';
   if (hours > 0) result = `${formatted.hours}:${formatted.minutes}:${formatted.seconds}`;
   else result = `${formatted.minutes}:${formatted.seconds}`;

   return <span>{result}</span>;
}
