import { TFunction } from 'react-i18next';
import { NumericAnswer, NumericInstruction } from '../../types';
import { PollTypePresenter } from '../types';
import NumericAnswerForm from './NumericAnswerForm';
import NumericInstructionForm from './NumericInstructionForm';
import NumericPollResults from './NumericPollResults';

const getPollDescription = (instruction: NumericInstruction, t: TFunction<string>) => {
   const modifiers = new Array<string>();

   if (typeof instruction.min === 'number' && typeof instruction.max === 'number') {
      modifiers.push(
         t('conference.poll.types.numeric.description_between', { min: instruction.min, max: instruction.max }),
      );
   } else if (typeof instruction.min === 'number') {
      modifiers.push(t('conference.poll.types.numeric.description_min', { count: instruction.min }));
   } else if (typeof instruction.max === 'number') {
      modifiers.push(t('conference.poll.types.numeric.description_max', { count: instruction.max }));
   }

   if (typeof instruction.step === 'number') {
      modifiers.push(t('conference.poll.types.numeric.description_step', { count: instruction.step }));
   }

   if (modifiers.length > 0) {
      return `${t('conference.poll.types.numeric.label')} (${modifiers.join(', ')})`;
   } else return t('conference.poll.types.numeric.label');
};

const presenter: PollTypePresenter<NumericInstruction, NumericAnswer> = {
   getPollDescription,

   instructionType: 'numeric',
   labelTranslationKey: 'conference.poll.types.numeric.label',

   answerType: 'numeric',
   InstructionForm: NumericInstructionForm,
   PollAnswerForm: NumericAnswerForm,

   ResultsView: NumericPollResults,
};

export default presenter;
