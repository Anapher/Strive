import { UseFormReturn } from 'react-hook-form';
import { TFunction } from 'react-i18next';
import { CreatePollDto } from 'src/core-hub.types';
import { PollAnswer, PollInstruction, PollViewModel } from '../types';

export type InstructionFormProps = {
   form: UseFormReturn<CreatePollDto>;
   showAdvanced: boolean;
};

export type PollAnswerFormProps<A extends PollAnswer> = {
   poll: PollViewModel;
   currentAnswer: Partial<A>;
   onChangeCurrentAnswer: (value: Partial<A>) => void;
   onSubmit: (value: A) => void;
   onDelete: () => void;
};

export type PollResultsProps = {
   viewModel: PollViewModel;
};

export type PollTypePresenter<I extends PollInstruction, A extends PollAnswer> = {
   getPollDescription: (instruction: I, t: TFunction<string>) => string;

   instructionType: I['type'];
   labelTranslationKey: string;
   InstructionForm: React.ComponentType<InstructionFormProps>;

   answerType: A['type'];
   PollAnswerForm: React.ComponentType<PollAnswerFormProps<A>>;
   pollAnswerFormExternalSubmit?: boolean;

   ResultsView: React.ComponentType<PollResultsProps>;
};
