using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoaLogger
{
  public partial class PKTNewZone
    {
        public PKTNewZone(BitReader reader)
        {
            if (Parser.region == Parser.Region.Steam) SteamDecode(reader);
            if (Parser.region == Parser.Region.Korea) SteamDecode(reader);
        }
        public UInt32 zoneID;
        public UInt32 field1;
        public UInt32 field2;
        public UInt32 field3;
    }
}
