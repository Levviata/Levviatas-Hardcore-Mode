using levviatashardcoremode.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
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
			return shouldKillPlayer;
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
            afterMarkerIndex = 0;
            played = false;
			shootPlayed = false;
            backgroundAlpha = 0;
            increaseAbove = 0;
            increaseBelow = 0;
            aboveTop = 0;
            belowTop = 0;
            markerIndex = 0;
            timerShoot = 0;
            playerDead = false;
            shouldKillPlayer = false;
            shouldStartTimerShoot = false;
            Main.mapMinimapAlpha = 1f;
            Main.mapEnabled = true;
            Main.mapOverlayAlpha = 0.35f;
            Main.musicVolume = initialVolume;
            Main.UIScale = initialUISize;
            if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive())
            {
                Filters.Scene["Shockwave"].Deactivate();
            }

            Main.LocalPlayer.lifeRegen = Main.LocalPlayer.lifeRegen;
            Main.blockInput = false;
            Main.LocalPlayer.immuneNoBlink = false;
		}
	}
}
