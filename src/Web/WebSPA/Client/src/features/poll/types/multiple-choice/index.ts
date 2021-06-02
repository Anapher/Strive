import { MultipleChoiceInstruction } from '../../types';
import { PollTypePresenter } from '../types';
import MultipleChoiceInstructionForm from './MultipleChoiceInstructionForm';

const presenter: PollTypePresenter<MultipleChoiceInstruction> = {
   instructionType: 'multipleChoice',
   labelTranslationKey: 'conference.poll.types.multiple_choice',
   InstructionForm: MultipleChoiceInstructionForm,
};

export default presenter;
