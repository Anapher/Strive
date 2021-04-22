using System.Collections.Generic;
using System.Collections.Immutable;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Scenes.Utilities;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.Scenes.Utilities
{
    public class SceneUtilitiesTests
    {
        [Fact]
        public void ParticipantsOfRoomChanged_PreviousValueNull_ReturnTrue()
        {
            // arrange
            var current = new SynchronizedRooms(ImmutableList<Room>.Empty, "default",
                ImmutableDictionary<string, string>.Empty);
            SynchronizedRooms? previous = null;

            // act
            var result = SceneUtilities.ParticipantsOfRoomChanged("room1", current, previous);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void ParticipantsOfRoomChanged_NoChanges_ReturnFalse()
        {
            // arrange
            var current = new SynchronizedRooms(ImmutableList<Room>.Empty, "default",
                ImmutableDictionary<string, string>.Empty);
            var previous = new SynchronizedRooms(ImmutableList<Room>.Empty, "default",
                ImmutableDictionary<string, string>.Empty);

            // act
            var result = SceneUtilities.ParticipantsOfRoomChanged("room1", current, previous);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void ParticipantsOfRoomChanged_SomeChangesInOtherRooms_ReturnFalse()
        {
            // arrange
            var current = new SynchronizedRooms(ImmutableList<Room>.Empty, "default",
                ImmutableDictionary<string, string>.Empty);
            var previous = new SynchronizedRooms(ImmutableList<Room>.Empty, "default",
                new Dictionary<string, string> {{"p1", "room2"}});

            // act
            var result = SceneUtilities.ParticipantsOfRoomChanged("room1", current, previous);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void ParticipantsOfRoomChanged_SomeChanges_ReturnTrue()
        {
            // arrange
            var current = new SynchronizedRooms(ImmutableList<Room>.Empty, "default",
                ImmutableDictionary<string, string>.Empty);
            var previous = new SynchronizedRooms(ImmutableList<Room>.Empty, "default",
                new Dictionary<string, string> {{"p1", "room1"}});

            // act
            var result = SceneUtilities.ParticipantsOfRoomChanged("room1", current, previous);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void GetParticipantsOfRoom_EmptyParticipants_ReturnEmpty()
        {
            // arrange
            var rooms = new SynchronizedRooms(ImmutableList<Room>.Empty, "d",
                ImmutableDictionary<string, string>.Empty);

            // act
            var result = SceneUtilities.GetParticipantsOfRoom(rooms, "test");

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetParticipantsOfRoom_SomeParticipants_ReturnParticipants()
        {
            // arrange
            var rooms = new SynchronizedRooms(ImmutableList<Room>.Empty, "d", new Dictionary<string, string>
            {
                {"p1", "room1"},
                {"p2", "room1"},
                {"p3", "room2"},
                {"p4", "room4"},
            });

            // act
            var result = SceneUtilities.GetParticipantsOfRoom(rooms, "room1");

            // assert
            AssertHelper.AssertScrambledEquals(new[] {"p1", "p2"}, result);
        }

        [Fact]
        public void HasSceneStackChanged_PreviousNull_ReturnTrue()
        {
            // arrange
            var current = new SynchronizedScene(AutonomousScene.Instance, null, ImmutableList<IScene>.Empty,
                ImmutableList<IScene>.Empty);

            SynchronizedScene? previous = null;

            // act
            var result = SceneUtilities.HasSceneStackChanged(current, previous);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void HasSceneStackChanged_NotChanged_ReturnFalse()
        {
            // arrange
            var current = new SynchronizedScene(AutonomousScene.Instance, null, ImmutableList<IScene>.Empty,
                new IScene[] {AutonomousScene.Instance, GridScene.Instance});

            var previous = new SynchronizedScene(GridScene.Instance, null, ImmutableList<IScene>.Empty,
                new IScene[] {AutonomousScene.Instance, GridScene.Instance});

            // act
            var result = SceneUtilities.HasSceneStackChanged(current, previous);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void HasSceneStackChanged_Changed_ReturnTrue()
        {
            // arrange
            var current = new SynchronizedScene(AutonomousScene.Instance, null, ImmutableList<IScene>.Empty,
                new IScene[] {AutonomousScene.Instance, GridScene.Instance});

            var previous = new SynchronizedScene(GridScene.Instance, null, ImmutableList<IScene>.Empty,
                new IScene[] {AutonomousScene.Instance});

            // act
            var result = SceneUtilities.HasSceneStackChanged(current, previous);

            // assert
            Assert.True(result);
        }
    }
}
