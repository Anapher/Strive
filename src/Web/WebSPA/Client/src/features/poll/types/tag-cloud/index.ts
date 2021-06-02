import { TagCloudInstruction } from '../../types';
import { PollTypePresenter } from '../types';
import TagCloudInstructionForm from './TagCloudForm';

const presenter: PollTypePresenter<TagCloudInstruction> = {
   instructionType: 'tagCloud',
   labelTranslationKey: 'conference.poll.types.tag_cloud',
   InstructionForm: TagCloudInstructionForm,
};

export default presenter;
