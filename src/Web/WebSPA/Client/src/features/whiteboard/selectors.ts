import { RootState } from 'src/store';

export const selectWhiteboard = (state: RootState, id: string) => state.whiteboard.whiteboards?.whiteboards[id];
export const selectWhiteboardsCount = (state: RootState) =>
   state.whiteboard.whiteboards ? Object.keys(state.whiteboard.whiteboards.whiteboards).length : 0;
