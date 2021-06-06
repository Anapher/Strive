import PollIcon from '@material-ui/icons/Poll';
import React, { useMemo } from 'react';
import { TFunction, useTranslation } from 'react-i18next';
import { AvailableSceneListItemProps } from '../../types';
import SceneListItemWithPopper from '../SceneListItemWithPopper';
import PollOptionsPopper from './PollOptionsPopper';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import { useSelector } from 'react-redux';
import { selectPoll } from 'src/features/poll/selectors';
import { RootState } from 'src/store';
import { SynchronizedPoll } from 'src/features/poll/types';
import pollTypes from 'src/features/poll/types/register';
import { deleteStopwords } from 'src/utils/string-utils';

function buildPollLabel(t: TFunction, locale: string, poll: SynchronizedPoll | undefined) {
   if (!poll) return t('conference.scenes.poll.scene_label');

   const pollPresenter = pollTypes.find((x) => x.instructionType === poll.instruction.type);
   const label = pollPresenter && t(pollPresenter.labelTranslationKey);

   const importantQuestion = poll.config.question ? deleteStopwords(poll.config.question, locale) : undefined;

   return (importantQuestion ? importantQuestion + ': ' : '') + label;
}

export default function PollListItem(props: AvailableSceneListItemProps) {
   const scene = props.scene;
   if (scene.type !== 'poll') throw new Error('Invalid scene type');

   const { t, i18n } = useTranslation();
   const poll = useSelector((state: RootState) => selectPoll(state, scene.pollId));

   const label = useMemo(() => buildPollLabel(t, i18n.language, poll), [t, i18n.language, poll]);

   return (
      <SceneListItemWithPopper
         PopperComponent={PollOptionsPopper}
         {...props}
         icon={<PollIcon />}
         listItemIcon={<MoreVertIcon />}
         title={label}
      />
   );
}
