using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoaLogger
{
    public class Zone
    {
        public static Dictionary<Int32, String> Zones = (Dictionary<Int32, String>)ObjectSerialize.Deserialize(Properties.Resources.Zones);


        public static String GetZoneName(UInt32 id)
        {
            var zone = "Unknown Zone";
            if (Zones.ContainsKey((int)id))
            {
                zone = Zones[(int)id];
            }
            return zone;
        }
    }
}
