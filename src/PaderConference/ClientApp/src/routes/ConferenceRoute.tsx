import { RootState } from 'pader-conference';
import React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import { joinConference } from 'src/store/conference-signal/actions';

const mapStateToProps = (state: RootState) => ({
   isConnected: state.signalr.isConnected,
});

const dispatchProps = {
   joinConference,
};

type RouteParams = {
   id: string;
};

type Props = ReturnType<typeof mapStateToProps> & typeof dispatchProps & RouteComponentProps<RouteParams>;

function ConferenceRoute({
   joinConference,
   match: {
      params: { id },
   },
}: Props) {
   return <div>Hello to this conference: {id}</div>;
}

export default connect(mapStateToProps, dispatchProps)(ConferenceRoute);
