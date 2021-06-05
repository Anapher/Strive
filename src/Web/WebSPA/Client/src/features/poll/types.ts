export type SingleChoiceInstruction = {
   type: 'singleChoice';
   options: string[];
};

export type MultipleChoiceInstruction = {
   type: 'multipleChoice';
   options: string[];
   maxSelections?: number | null;
};

export type NumericInstruction = {
   type: 'numeric';
   min?: number | null;
   max?: number | null;
   step?: number | null;
};

export type TagCloudClusterMode = 'caseInsensitive' | 'fuzzy';

export type TagCloudInstruction = {
   type: 'tagCloud';
   maxTags?: number | null;
   mode: TagCloudClusterMode;
};

export type PollInstruction =
   | SingleChoiceInstruction
   | MultipleChoiceInstruction
   | NumericInstruction
   | TagCloudInstruction;

export type SelectionPollResults = {
   type: 'selection';
   options: { [option: string]: string[] };
};

export type NumericPollResults = {
   type: 'numeric';
   answers: { [answerId: string]: number };
};

export type TagCloudResults = {
   type: 'tagCloud';
   tags: { [tag: string]: string[] };
};

export type PollResults = SelectionPollResults | NumericPollResults | TagCloudResults;

export type SingleChoiceAnswer = {
   type: 'singleChoice';
   selected: string;
};

export type MultipleChoiceAnswer = {
   type: 'multipleChoice';
   selected: string[];
};

export type NumericAnswer = {
   type: 'numeric';
   selected: number;
};

export type TagCloudAnswer = {
   type: 'tagCloud';
   tags: string[];
};

export type PollAnswer = SingleChoiceAnswer | MultipleChoiceAnswer | NumericAnswer | TagCloudAnswer;

export type PollAnswerWithKey = {
   answer: PollAnswer;
   key: string;
};

export type PollConfig = {
   question?: string;
   isAnonymous: boolean;
   isAnswerFinal: boolean;
};

export type PollState = {
   isOpen: boolean;
   resultsPublished: boolean;
};

export type SynchronizedPoll = {
   id: string;
   instruction: PollInstruction;
   config: PollConfig;
   state: PollState;
};

export type SynchronizedPollResults = {
   pollId: string;
   results: PollResults;
   participantsAnswered: number;
   tokenIdToParticipant?: { [token: string]: string } | null;
};

export type SynchronizedPollAnswers = {
   answers: { [key: string]: PollAnswerWithKey };
};

export type PollViewModel = {
   poll: SynchronizedPoll;
   answer?: PollAnswerWithKey;
   results?: SynchronizedPollResults;
};
