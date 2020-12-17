﻿using System.Collections.Generic;
using System.Text.Json;

namespace PaderConference.Core.Services.Permissions
{
    public static class CommonPermissionLayers
    {
        public static PermissionLayer ConferenceDefault(IReadOnlyDictionary<string, JsonElement> permissions)
        {
            return new(10, "CONFERENCE_DEFAULT", permissions);
        }

        public static PermissionLayer Conference(IReadOnlyDictionary<string, JsonElement> permissions)
        {
            return new(11, "CONFERENCE", permissions);
        }

        public static PermissionLayer BreakoutroomDefault(IReadOnlyDictionary<string, JsonElement> permissions)
        {
            return new(20, "BREAKOUTROOM_DEFAULT", permissions);
        }

        public static PermissionLayer BreakoutRoom(IReadOnlyDictionary<string, JsonElement> permissions)
        {
            return new(21, "BREAKOUTROOM", permissions);
        }

        public static PermissionLayer ModeratorDefault(IReadOnlyDictionary<string, JsonElement> permissions)
        {
            return new(30, "MODERATOR_DEFAULT", permissions);
        }

        public static PermissionLayer Moderator(IReadOnlyDictionary<string, JsonElement> permissions)
        {
            return new(31, "MODERATOR", permissions);
        }

        public static PermissionLayer Temporary(IReadOnlyDictionary<string, JsonElement> permissions)
        {
            return new(100, "TEMPORARY", permissions);
        }
    }
}