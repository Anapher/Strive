export type WhiteboardInfo = {
   friendlyName: string;
   everyoneCanEdit: boolean;
};

export type SynchronizedWhiteboards = {
   whiteboards: Record<string, WhiteboardInfo>;
};
