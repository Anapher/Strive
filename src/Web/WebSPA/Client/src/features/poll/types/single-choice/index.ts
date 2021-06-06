import { TFunction } from 'react-i18next';
import { SingleChoiceAnswer, SingleChoiceInstruction } from '../../types';
import { PollTypePresenter } from '../types';
import SelectionPollResults from './SelectionPollResults';
import SingleChoiceAnswerForm from './SingleChoiceAnswerForm';
import SingleChoiceInstructionForm from './SingleChoiceInstructionForm';

const getPollDescription = (_: SingleChoiceInstruction, t: TFunction<string>) =>
   t('conference.poll.types.single_choice.label');

const presenter: PollTypePresenter<SingleChoiceInstruction, SingleChoiceAnswer> = {
   getPollDescription,

   instructionType: 'singleChoice',
   labelTranslationKey: 'conference.poll.types.single_choice.label',

   answerType: 'singleChoice',
   InstructionForm: SingleChoiceInstructionForm,
   PollAnswerForm: SingleChoiceAnswerForm,

   ResultsView: SelectionPollResults,
};

export default presenter;
