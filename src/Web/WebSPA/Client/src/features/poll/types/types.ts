import { PortalProps } from '@material-ui/core';
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
   onSubmit: (value: A) => void;
   onDelete: () => void;

   footerPortalRef: PortalProps['container'];
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

   ResultsView: React.ComponentType<PollResultsProps>;
};
