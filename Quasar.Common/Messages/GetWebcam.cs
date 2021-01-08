using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetWebcam : IMessage
    {
        [ProtoMember(1)]
        public bool CreateNew { get; set; }

        [ProtoMember(2)]
        public int Quality { get; set; }

        [ProtoMember(3)]
        public int DisplayIndex { get; set; }

        [ProtoMember(4)]
        public bool Destroy { get; set; }
    }
}
