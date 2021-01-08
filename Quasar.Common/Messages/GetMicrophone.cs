﻿using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetMicrophone : IMessage
    {
        [ProtoMember(1)]
        public bool CreateNew { get; set; }

        [ProtoMember(2)]
        public int DeviceIndex { get; set; }

        [ProtoMember(3)]
        public int Bitrate { get; set; }

        [ProtoMember(4)]
        public bool Destroy { get; set; }
    }
}
