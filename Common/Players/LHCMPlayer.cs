using levviatashardcoremode.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using static levviatashardcoremode.Common.Systems.DeathHandler;

namespace levviatashardcoremode.Common.Players
{
	public class LHCMPlayer : ModPlayer
	{
		public static bool playerDead = false;
		public override void PreUpdate()
		{
			if (playerDead)
			{
				Main.LocalPlayer.statLife = 0;
				Main.LocalPlayer.lifeRegen = 0;
			}
		}
		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
		{
			Main.LocalPlayer.immuneNoBlink = true;
			Main.LocalPlayer.immuneTime = 100000;
			playerDead = true;
			return false;
		}
		public override void PreUpdateMovement()
		{
			if (playerDead == true) Main.LocalPlayer.velocity *= 0;
		}
        public override bool CanHitNPC(NPC target)
        {
			if (playerDead) return false;
			else return true;
        }
        public override void OnRespawn()
		{
			Main.musicVolume = initialVolume;
            Main.LocalPlayer.lifeRegen = Main.LocalPlayer.lifeRegen;
            playerDead = false;
			Main.LocalPlayer.immuneNoBlink = false;
		}
	}
}
