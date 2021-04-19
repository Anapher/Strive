using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Strive.Messaging.SFU;
using Strive.Messaging.SFU.Dto;
using Xunit;

namespace Strive.Tests.Messaging.SFU
{
    public class SfuConferenceInfoUpdateAggregatorTests
    {
        private SfuConferenceInfoUpdateAggregator Create()
        {
            return new();
        }

        [Fact]
        public void GetUpdate_NoAppends_ReturnEmpty()
        {
            // arrange
            var aggregator = Create();

            // act
            var result = aggregator.GetUpdate();

            // assert
            Assert.Empty(result.RemovedParticipants);
            Assert.Empty(result.ParticipantPermissions);
            Assert.Empty(result.ParticipantToRoom);
        }

        [Fact]
        public void GetUpdate_AppendOneUpdate_ReturnUpdate()
        {
            // arrange
            var aggregator = Create();

            var update = new SfuConferenceInfoUpdate(new Dictionary<string, string> {{"participant1", "room1"}},
                new Dictionary<string, SfuParticipantPermissions>
                {
                    {"participant2", new SfuParticipantPermissions(false, true, true)},
                }, new[] {"participant3"});
            aggregator.Append(update);

            // act
            var result = aggregator.GetUpdate();

            // assert
            Assert.Single(result.RemovedParticipants, "participant3");
            Assert.Single(result.ParticipantPermissions,
                new KeyValuePair<string, SfuParticipantPermissions>("participant2",
                    new SfuParticipantPermissions(false, true, true)));
            Assert.Single(result.ParticipantToRoom, new KeyValuePair<string, string>("participant1", "room1"));
        }

        [Fact]
        public void GetUpdate_AppendUpdateOverwriteRoom_ReturnUpdate()
        {
            // arrange
            var aggregator = Create();

            aggregator.Append(new SfuConferenceInfoUpdate(new Dictionary<string, string> {{"participant1", "room1"}},
                ImmutableDictionary<string, SfuParticipantPermissions>.Empty, Array.Empty<string>()));
            aggregator.Append(new SfuConferenceInfoUpdate(new Dictionary<string, string> {{"participant1", "room2"}},
                ImmutableDictionary<string, SfuParticipantPermissions>.Empty, Array.Empty<string>()));

            // act
            var result = aggregator.GetUpdate();

            // assert
            Assert.Single(result.ParticipantToRoom, new KeyValuePair<string, string>("participant1", "room2"));
        }

        [Fact]
        public void GetUpdate_AppendUpdateOverwritePermissions_ReturnUpdate()
        {
            // arrange
            var aggregator = Create();

            aggregator.Append(new SfuConferenceInfoUpdate(ImmutableDictionary<string, string>.Empty,
                new Dictionary<string, SfuParticipantPermissions>
                {
                    {"participant1", new SfuParticipantPermissions(true, false, false)},
                }, Array.Empty<string>()));

            aggregator.Append(new SfuConferenceInfoUpdate(ImmutableDictionary<string, string>.Empty,
                new Dictionary<string, SfuParticipantPermissions>
                {
                    {"participant1", new SfuParticipantPermissions(false, false, false)},
                }, Array.Empty<string>()));

            // act
            var result = aggregator.GetUpdate();

            // assert
            Assert.Single(result.ParticipantPermissions,
                new KeyValuePair<string, SfuParticipantPermissions>("participant1",
                    new SfuParticipantPermissions(false, false, false)));
        }

        [Fact]
        public void GetUpdate_RemoveParticipant_ReturnUpdateRemoveRoomMapping()
        {
            // arrange
            var aggregator = Create();

            aggregator.Append(new SfuConferenceInfoUpdate(new Dictionary<string, string> {{"participant1", "room1"}},
                new Dictionary<string, SfuParticipantPermissions>
                {
                    {"participant1", new SfuParticipantPermissions(true, false, false)},
                }, Array.Empty<string>()));

            aggregator.Append(new SfuConferenceInfoUpdate(ImmutableDictionary<string, string>.Empty,
                ImmutableDictionary<string, SfuParticipantPermissions>.Empty, new[] {"participant1"}));

            // act
            var result = aggregator.GetUpdate();

            // assert
            Assert.Empty(result.ParticipantToRoom);
            Assert.Single(result.RemovedParticipants, "participant1");
            Assert.Single(result.ParticipantPermissions,
                new KeyValuePair<string, SfuParticipantPermissions>("participant1",
                    new SfuParticipantPermissions(true, false, false)));
        }
    }
}
