﻿using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetMicrophoneResponse : IMessage
    {
        [ProtoMember(1)]
        public byte[] Audio { get; set; }

        [ProtoMember(2)]
        public int Device { get; set; }
    }
}
