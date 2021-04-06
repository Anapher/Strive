-- @desc:   Set the room of a participant and gurantee that this room exists in the room list
-- @return: return true if successful, else return false

local function SETPARTICIPANTROOM(roomMappingKey, roomListKey, participantId, newRoomId)
  local roomExists = redis.call("HEXISTS", roomListKey, newRoomId)
  if roomExists == 0 then
    return false
  end

  redis.call("HSET", roomMappingKey, participantId, newRoomId)
  return true
end

return SETPARTICIPANTROOM(KEYS[1], KEYS[2], KEYS[3], KEYS[4])