-- @desc:   Remove a participant from a conference atomically
-- @usage:  redis-cli EVAL "$(cat JoinedParticipantsRepository_RemoveParticipant.lua)" 3 <participantId> <participantKey> <conferenceKeyTemplate>
-- @params: participantId: the id of the participant who should be removed
--          participantKey: the redis key that holds the conference id of the participant
--          conferenceKeyTemplate: the template of the conference to participants key with a star where the conference id should be placed
-- @return: return the conference id if the participant did belong to a conference, else return nil

local function REMOVEPARTICIPANT(participantId, participantKey, conferenceKeyTemplate)
  local conferenceId = redis.call("GET", participantKey)
  if not conferenceId then
    return nil
  end

  redis.call("DEL", participantKey)

  local conferenceKey = string.gsub(conferenceKeyTemplate, "%*", conferenceId)
  local previousConnectionId = redis.call("HGET", conferenceKey, participantId)

  redis.call("HDEL", conferenceKey, participantId)

  return {conferenceId, previousConnectionId}
end

return REMOVEPARTICIPANT(KEYS[1], KEYS[2], KEYS[3])