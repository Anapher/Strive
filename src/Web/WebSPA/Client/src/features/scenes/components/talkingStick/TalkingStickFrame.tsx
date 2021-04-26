import React from 'react';
import { useSelector } from 'react-redux';
import { selectIsMePresenter } from '../../selectors';

type Props = {
   children: React.ReactNode;
};

export default function TalkingStickFrame({ children }: Props) {
   const isPresenter = useSelector(selectIsMePresenter);

   return <div>test</div>;
}
