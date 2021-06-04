import { TFunction } from 'react-i18next';
import { TagCloudAnswer, TagCloudInstruction } from '../../types';
import { PollTypePresenter } from '../types';
import TagCloudInstructionForm from './TagCloudForm';

const getPollDescription = (instruction: TagCloudInstruction, t: TFunction<string>) =>
   instruction.maxTags
      ? t('conference.poll.types.tag_cloud.description', { count: instruction.maxTags })
      : t('conference.poll.types.tag_cloud.label');

const presenter: PollTypePresenter<TagCloudInstruction, TagCloudAnswer> = {
   getPollDescription,

   instructionType: 'tagCloud',
   labelTranslationKey: 'conference.poll.types.tag_cloud.label',

   answerType: 'tagCloud',
   InstructionForm: TagCloudInstructionForm,
   PollAnswerForm: null as any,

   ResultsView: null as any,
};

export default presenter;
