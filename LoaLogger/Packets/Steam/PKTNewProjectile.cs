using System;
using System.Collections.Generic;
namespace LoaLogger
{
    public partial class PKTNewProjectile
    {
        public void SteamDecode(BitReader reader)
        {
            projectileInfo = reader.Read<ProjectileInfo>();
        }
    }
}
