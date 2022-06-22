using System;
using System.Collections.Generic;
namespace LoaLogger
{
    public partial class PKTInitEnv_1
    {
        public PKTInitEnv_1(BitReader reader)
        {
            if (Parser.region == Parser.Region.Steam) SteamDecode(reader);
            if (Parser.region == Parser.Region.Korea) SteamDecode(reader);
        }
        public UInt16 num;
        public List<List<Byte>> field0s = new List<List<Byte>>();
        public List<List<Byte>> field1s = new List<List<Byte>>();
        public List<List<Byte>> field2s = new List<List<Byte>>();
    }
}
