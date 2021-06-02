import { NumericInstruction } from '../../types';
import { PollTypePresenter } from '../types';
import NumericInstructionForm from './NumericInstructionForm';

const presenter: PollTypePresenter<NumericInstruction> = {
   instructionType: 'numeric',
   labelTranslationKey: 'conference.poll.types.numeric',
   InstructionForm: NumericInstructionForm,
};

export default presenter;
