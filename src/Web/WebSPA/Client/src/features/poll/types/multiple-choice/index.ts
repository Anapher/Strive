import { TFunction } from 'react-i18next';
import { MultipleChoiceAnswer, MultipleChoiceInstruction } from '../../types';
import SelectionPollResults from '../single-choice/SelectionPollResults';
import { PollTypePresenter } from '../types';
import MultipleChoiceAnswerForm from './MultipleChoiceAnswerForm';
import MultipleChoiceInstructionForm from './MultipleChoiceInstructionForm';

const getPollDescription = (instruction: MultipleChoiceInstruction, t: TFunction<string>) =>
   instruction.maxSelections
      ? t('conference.poll.types.multiple_choice.description', { count: instruction.maxSelections })
      : t('conference.poll.types.multiple_choice.label');

const presenter: PollTypePresenter<MultipleChoiceInstruction, MultipleChoiceAnswer> = {
   getPollDescription,

   instructionType: 'multipleChoice',
   labelTranslationKey: 'conference.poll.types.multiple_choice.label',
   InstructionForm: MultipleChoiceInstructionForm,

   answerType: 'multipleChoice',
   PollAnswerForm: MultipleChoiceAnswerForm,

   ResultsView: SelectionPollResults,
};

export default presenter;
