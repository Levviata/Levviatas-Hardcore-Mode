using Microsoft.Xna.Framework.Audio;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static levviatashardcoremode.Common.Systems.BossRushHandler;

namespace levviatashardcoremode.Common.Effects
{
    public class MusicLastHalf : ModSceneEffect
    {
        //public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/redemption");
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/redemptionAlmostLast");
        public override bool IsSceneEffectActive(Player player)
        {
            if (playedCounterSong == true)
            {
                if (time.Seconds > -30 && time.Minutes == 0) return true;
            }
            return false;
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;
    }
}
