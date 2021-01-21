import { SerializedError } from '@reduxjs/toolkit';
import { AxiosError } from 'axios';
import * as errors from 'src/errors';

export function serializeRequestError(error: unknown): SerializedError {
   const axiosError: AxiosError = error as AxiosError;
   if (!axiosError.isAxiosError) {
      return errors.unknownRequestError(axiosError.toString());
   }

   if (!axiosError.response) {
      return errors.serverUnavailable();
   }

   if (!axiosError.response.data.code) {
      return errors.unknownRequestError(`Status: ${axiosError.response.status}`);
   }

   return axiosError.response.data;
}
