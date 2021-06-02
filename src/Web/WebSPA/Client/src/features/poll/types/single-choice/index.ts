import { SingleChoiceInstruction } from '../../types';
import { PollTypePresenter } from '../types';
import SingleChoiceInstructionForm from './SingleChoiceInstructionForm';

const presenter: PollTypePresenter<SingleChoiceInstruction> = {
   instructionType: 'singleChoice',
   labelTranslationKey: 'conference.poll.types.single_choice',
   InstructionForm: SingleChoiceInstructionForm,
};

export default presenter;
