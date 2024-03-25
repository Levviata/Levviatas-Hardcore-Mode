using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static levviatashardcoremode.Common.Systems.BossRushHandler;

namespace levviatashardcoremode.Common.Effects
{
    public class MusicPlayer : ModSceneEffect
    {
        //public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/redemption");
        public override int Music => 0;
        public override bool IsSceneEffectActive(Player player)
        {
            if (played == true) return true;
            return false;
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    }
}
