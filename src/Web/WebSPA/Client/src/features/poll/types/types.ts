import { UseFormReturn } from 'react-hook-form';
import { CreatePollDto } from 'src/core-hub.types';
import { PollInstruction } from '../types';

export type InstructionFormProps = {
   form: UseFormReturn<CreatePollDto>;
   showAdvanced: boolean;
};

export type PollTypePresenter<I extends PollInstruction> = {
   instructionType: I['type'];
   labelTranslationKey: string;
   InstructionForm: React.ComponentType<InstructionFormProps>;
};
