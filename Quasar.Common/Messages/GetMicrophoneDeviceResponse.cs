using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetMicrophoneDeviceResponse : IMessage
    {
        [ProtoMember(1)]
        public List<Tuple<int, string>> DeviceInfos { get; set; }
    }
}
