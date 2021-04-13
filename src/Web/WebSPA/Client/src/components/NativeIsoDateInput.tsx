import { DateTime } from 'luxon';
import React from 'react';

type Props = Omit<Omit<React.HTMLProps<HTMLInputElement>, 'onChange'>, 'value'> & {
   min?: string | null;
   value?: string | null;
   onChange?: (newVal: string | undefined) => void;
};

const format = "yyyy-MM-dd'T'HH:mm";

const toHtmlDate = (date?: string | null) => (date ? DateTime.fromISO(date).toLocal().toFormat(format) : undefined);
const fromHtmlDate = (s?: string) => (s === undefined ? undefined : DateTime.fromFormat(s, format).toISO());

export default React.forwardRef<HTMLInputElement, Props>(function NativeIsoDateInput(
   { min, value, onChange, ...inputProps },
   ref,
) {
   return (
      <input
         {...inputProps}
         ref={ref}
         type="datetime-local"
         min={toHtmlDate(min)}
         value={toHtmlDate(value)}
         onChange={(ev) => {
            if (onChange) onChange(fromHtmlDate(ev.target.value));
         }}
      />
   );
});
