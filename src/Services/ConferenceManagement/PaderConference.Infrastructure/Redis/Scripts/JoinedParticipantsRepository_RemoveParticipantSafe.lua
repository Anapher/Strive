-- @desc:   Remove a participant from a conference atomically
-- @usage:  redis-cli EVAL "$(cat JoinedParticipantsRepository_RemoveParticipant.lua)" 3 <participantId> <participantKey> <conferenceKeyTemplate>
-- @params: participantId: the id of the participant who should be removed
--          participantKey: the redis key that holds the conference id of the participant
--          conferenceKeyTemplate: the template of the conference to participants key with a star where the conference id should be placed
--          connectionId: the connection id of the participant that must match the records, else this operation fails
-- @return: return the conference id if the participant did belong to a conference, else return nil

local function REMOVEPARTICIPANTSAFE(participantId, participantKey, conferenceKeyTemplate, connectionId)
  local conferenceId = redis.call("GET", participantKey)
  if not conferenceId then
    return nil
  end

  local conferenceKey = string.gsub(conferenceKeyTemplate, "%*", conferenceId)

  local actualConnectionId = redis.call("HGET", conferenceKey, participantId)
  if actualConnectionId == connectionId then
    redis.call("HDEL", conferenceKey, participantId)
    redis.call("DEL", participantKey)

    return true
  end

  return false
end

return REMOVEPARTICIPANTSAFE(KEYS[1], KEYS[2], KEYS[3], KEYS[4])