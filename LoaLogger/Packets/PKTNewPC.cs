using System;
using System.Collections.Generic;
namespace LoaLogger
{
    public partial class PKTNewPC
    {
        public PKTNewPC(BitReader reader)
        {
            if (Parser.region == Parser.Region.Steam) SteamDecode(reader);
            if (Parser.region == Parser.Region.Korea) SteamDecode(reader);
        }
        public Byte hasfield0;
        public Byte[] field0;
        public Byte hasfield1;
        public UInt32 field1;
        public Byte hasfield2;
        public PKTNewPC_1 pKTNewPC_1;
        public Byte field3;
        public Byte hasfield4;
        public Byte[] field4;
        public PCStruct pCStruct;
        public Byte field6;
    }
}
