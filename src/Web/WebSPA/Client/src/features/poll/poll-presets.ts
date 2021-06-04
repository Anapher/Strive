import { TFunction } from 'react-i18next';
import { CreatePollDto } from 'src/core-hub.types';

type PollPreset = {
   label: string;
   data: CreatePollDto;
};

const getPresets: (t: TFunction) => PollPreset[] = (t) => [
   {
      label: t<string>('conference.poll.create_dialog.presets.yes_no.label'),
      data: {
         config: { isAnonymous: true, isAnswerFinal: true },
         initialState: { isOpen: true, resultsPublished: false },
         instruction: {
            type: 'singleChoice',
            options: [
               t<string>('conference.poll.create_dialog.presets.yes_no.yes'),
               t<string>('conference.poll.create_dialog.presets.yes_no.no'),
            ],
         },
      },
   },
   {
      label: t<string>('conference.poll.create_dialog.presets.a_b_c.label'),
      data: {
         config: { isAnonymous: true, isAnswerFinal: true },
         initialState: { isOpen: true, resultsPublished: false },
         instruction: {
            type: 'singleChoice',
            options: ['A', 'B', 'C'],
         },
      },
   },
   {
      label: t<string>('conference.poll.create_dialog.presets.true_false.label'),
      data: {
         config: { isAnonymous: true, isAnswerFinal: true },
         initialState: { isOpen: true, resultsPublished: false },
         instruction: {
            type: 'singleChoice',
            options: [
               t<string>('conference.poll.create_dialog.presets.true_false.true'),
               t<string>('conference.poll.create_dialog.presets.true_false.false'),
            ],
         },
      },
   },
   {
      label: t<string>('conference.poll.create_dialog.presets.task_status.label'),
      data: {
         config: { isAnonymous: true, isAnswerFinal: false },
         initialState: { isOpen: true, resultsPublished: false },
         instruction: {
            type: 'singleChoice',
            options: [
               t<string>('conference.poll.create_dialog.presets.task_status.finished'),
               t<string>('conference.poll.create_dialog.presets.task_status.surrendered'),
               t<string>('conference.poll.create_dialog.presets.task_status.need_time'),
            ],
         },
      },
   },
];

export default getPresets;
