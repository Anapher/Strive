﻿using System;
using PaderConference.Infrastructure.Services.Media.Mediasoup;

namespace PaderConference.Infrastructure.Services.Equipment.Dto
{
    public class EquipmentCommand
    {
        public Guid EquipmentId { get; set; }

        public ProducerSource Source { get; set; }

        public string? DeviceId { get; set; }

        public EquipmentCommandType Action { get; set; }
    }

    public enum EquipmentCommandType
    {
        Enable,
        Disable,
        Pause,
        Resume,
        SwitchDevice,
    }
}