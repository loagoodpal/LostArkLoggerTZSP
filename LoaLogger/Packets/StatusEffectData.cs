using System;
using System.Collections.Generic;
namespace LoaLogger
{
    public partial class StatusEffectData
    {
        public StatusEffectData(BitReader reader)
        {
            if (Parser.region == Parser.Region.Steam) SteamDecode(reader);
            if (Parser.region == Parser.Region.Korea) SteamDecode(reader);
        }
        public UInt32 field0;
        public Byte field1;
        public Byte field2;
        public Byte hasValue;
        public Byte[] Value;
        public UInt64 InstanceId;
        public UInt32 BuffId;
        public List<Byte[]> field6;
        public UInt32 field7;
        public UInt64 field8;
        public Byte hasfield9;
        public UInt64 field9;
        public UInt64 SourceId;
    }
}
