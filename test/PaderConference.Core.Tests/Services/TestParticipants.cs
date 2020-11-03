using System;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Tests.Services
{
    public static class TestParticipants
    {
        public static Participant Default = new Participant("ALPHA", "Vincent", "mod",
            new DateTimeOffset(2020, 11, 2, 0, 0, 0, TimeSpan.Zero));

        public static Participant Default2 = new Participant("BRAVO", "Titus", "usr",
            new DateTimeOffset(2020, 11, 2, 0, 0, 0, TimeSpan.Zero));
    }
}
