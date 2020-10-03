import { StartConferenceRequestDto, StartConferenceResponseDto } from 'MyModels';
import { IRequestErrorResponse } from 'src/utils/error-result';
import { createAction, createAsyncAction } from 'typesafe-actions';

export const createConferenceAsync = createAsyncAction(
   'CONFERENCE/CREATE_REQUEST',
   'CONFERENCE/CREATE_SUCCESS',
   'CONFERENCE/CREATE_FAILURE',
)<StartConferenceRequestDto, StartConferenceResponseDto, IRequestErrorResponse>();

export const openCreateDialog = createAction('CONFERENCE/OPEN_CREATE_DIALOG')();
export const closeCreateDialog = createAction('CONFERENCE/CLOSE_CREATE_DIALOG')();
