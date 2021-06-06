import { TFunction } from 'react-i18next';
import { TagCloudAnswer, TagCloudInstruction } from '../../types';
import { PollTypePresenter } from '../types';
import TagCloudAnswerForm from './TagCloudAnswerForm';
import TagCloudInstructionForm from './TagCloudForm';
import TagCloudResults from './TagCloudResults';

const getPollDescription = (instruction: TagCloudInstruction, t: TFunction<string>) =>
   instruction.maxTags
      ? t('conference.poll.types.tag_cloud.description_max', { count: instruction.maxTags })
      : t('conference.poll.types.tag_cloud.label');

const presenter: PollTypePresenter<TagCloudInstruction, TagCloudAnswer> = {
   getPollDescription,

   instructionType: 'tagCloud',
   labelTranslationKey: 'conference.poll.types.tag_cloud.label',

   answerType: 'tagCloud',
   InstructionForm: TagCloudInstructionForm,
   PollAnswerForm: TagCloudAnswerForm,

   ResultsView: TagCloudResults,
};

export default presenter;
