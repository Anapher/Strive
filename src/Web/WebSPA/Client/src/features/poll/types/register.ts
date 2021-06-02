import { PollTypePresenter } from './types';
import singleChoice from './single-choice';
import multipleChoice from './multiple-choice';
import numeric from './numeric';
import tagCloud from './tag-cloud';

const pollTypes: PollTypePresenter<any>[] = [singleChoice, multipleChoice, numeric, tagCloud];

export default pollTypes;
