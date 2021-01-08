﻿using ProtoBuf;
using Quasar.Common.Video;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetWebcamResponse : IMessage
    {
        [ProtoMember(1)]
        public byte[] Image { get; set; }

        [ProtoMember(2)]
        public int Quality { get; set; }

        [ProtoMember(3)]
        public int Webcam { get; set; }

        [ProtoMember(4)]
        public Resolution Resolution { get; set; }
    }
}
