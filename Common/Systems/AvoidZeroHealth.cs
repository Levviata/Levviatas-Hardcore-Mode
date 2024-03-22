using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace levviatashardcoremode.Common.Systems
{
    public class AvoidZeroHealth : ModSystem
    {
        public override void OnWorldLoad()
        {
            if (Main.LocalPlayer.statLife < 0) 
            {
                Main.LocalPlayer.statLife = Main.LocalPlayer.statLifeMax;
            }
        }
    }
}
