using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoaLogger
{
    public partial class PKTNewZone
    {
        public void SteamDecode(BitReader reader)
        {
            zoneID = reader.ReadUInt32();
            field1 = reader.ReadUInt32();
            field2 = reader.ReadUInt32();
            field3 = reader.ReadUInt32();
        }
    }
}
