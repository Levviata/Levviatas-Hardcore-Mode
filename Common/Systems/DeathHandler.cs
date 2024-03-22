using levviatasprint.Common.Systems;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using static levviatashardcoremode.Common.Players.LHCMPlayer;

namespace levviatashardcoremode.Common.Systems
{
	public class DeathHandler : ModSystem
	{
		private Boolean hasRun = false;
		private SpriteBatch batch = Main.spriteBatch;
		internal UserInterface interAbove;
        internal UserInterface interBelow;
        internal DeathUIAbove UIAbove;
        internal DeathUIBelow UIBelow;
        Boolean isPressed = false;
		private GameTime _lastUpdateUiGameTime;
		public float currentVolume;
		public static float initialVolume;
		private int timer = 0;
        private int timerUIAbove = 0;
		private int timerUIBelow = 0;
		public static int aboveTop = 0;
        public static int increaseAbove = 0;
		public static int belowTop = 0;
        public static int increasebelow = 0;
		private int cyclesUntil150 = 30;
		internal void ShowMyUI()
		{
            interAbove?.SetState(UIAbove);
            interBelow?.SetState(UIBelow);
            if (aboveTop < 150)
			{
				if (timerUIAbove > 1) {
                    aboveTop += 1 * increaseAbove;
                    increaseAbove += 2;
                    UIAbove.Top.Set(aboveTop, 0);
                    interAbove?.SetState(UIAbove);
                    timerUIAbove = 0;
				}
			}
            
            if (belowTop < 150)
            {
                if (timerUIBelow > 1)
                {
                    belowTop += 1 * increasebelow;
                    increasebelow += 2;

                    UIBelow.Top.Set(-belowTop, 0);
                    interBelow?.SetState(UIBelow);
                    timerUIBelow = 0;
                }
            }
        }

        internal void HideMyUI()
		{
            interAbove?.SetState(null);
            interBelow?.SetState(null);
        }
		public override void Load()
		{
            
			if (!Main.dedServ)
			{
                interAbove = new UserInterface();
                interBelow = new UserInterface();

                UIBelow = new DeathUIBelow();
                UIAbove = new DeathUIAbove();

                UIAbove.Activate(); // Activate calls Initialize() on the UIState if not initialized and calls OnActivate, then calls Activate on every child element.
				UIBelow.Activate();

            }
		}
		public override void Unload()
		{
            UIAbove = null;
			UIBelow = null;
		}
        public override void OnWorldLoad()
        {
            increaseAbove = 0;
			increasebelow = 0;
            aboveTop = 0;
			belowTop = 0;
            playerDead = false;
            initialVolume = Main.musicVolume;
        }
        public override void UpdateUI(GameTime gameTime)
		{
			timer++;
			timerUIAbove++;
            timerUIBelow++;

            _lastUpdateUiGameTime = gameTime;
			if (interAbove?.CurrentState != null)
			{
                interAbove.Update(gameTime);
			}
            if (interBelow?.CurrentState != null)
            {
                interBelow.Update(gameTime);
            }

            if (Main.musicVolume <= 0) Main.musicVolume = 0;
            if (timer >= 10)
			{
				if (!Main.gamePaused) //trampoline
					if (playerDead) //weee
						Main.musicVolume -= 0.10f; //AAAAAAAAAAA
				timer = 0;
            }
            // Debugging

            if (KeybindSystem.showGUIKeybind.JustPressed)
			{
				isPressed = !isPressed;
				ShowMyUI();
			}
		 
			if (KeybindSystem.hideGUIKeybind.JustPressed) 
			{
				isPressed = false;
				HideMyUI(); 
			}

            if (Main.LocalPlayer.statLife > 2) 
			{

					HideMyUI();
			}
			if (Main.LocalPlayer.statLife <= 0) 
			{
                ShowMyUI();
            }
		}
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"LHardcoreMode: Death Screen",
					delegate
					{
						if (_lastUpdateUiGameTime != null && interAbove?.CurrentState != null)
						{
							interAbove.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
						}
                        if (_lastUpdateUiGameTime != null && interBelow?.CurrentState != null)
                        {
                            interBelow.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
					},
					InterfaceScaleType.UI));
			}
		}
	}
	public class DeathUIAbove : UIState
	{
		public override void OnInitialize()
		{
			UIPanel panelAbove = new UIPanel(Main.Assets.Request<Texture2D>("Images/UI/PanelBackground"), ModContent.Request<Texture2D>("levviatashardcoremode/Assets/UI/PanelBorder"), 12, 0);
            panelAbove.Top.Set(-300, 0);
            panelAbove.Left.Set(-50, 0);
			panelAbove.MaxWidth.Set(Main.screenWidth + 400, 0);
			panelAbove.Width.Set(Main.screenWidth + 400, 0);
			panelAbove.Height.Set(300, 0);
			panelAbove.BackgroundColor = new Color(0, 0, 0, 255);
			Append(panelAbove);
		}
	}
    public class DeathUIBelow : UIState
    {
        public override void OnInitialize()
        {
            UIPanel panelBelow = new UIPanel(Main.Assets.Request<Texture2D>("Images/UI/PanelBackground"), ModContent.Request<Texture2D>("levviatashardcoremode/Assets/UI/PanelBorder"), 12, 0);
            panelBelow.Top.Set(Main.screenHeight, 0);
            panelBelow.Left.Set(-50, 0);
            panelBelow.MaxWidth.Set(Main.screenWidth + 400, 0);
            panelBelow.Width.Set(Main.screenWidth + 400, 0);
            panelBelow.Height.Set(300, 0);
            panelBelow.BackgroundColor = new Color(0, 0, 0, 255);
            Append(panelBelow);
        }

    }
}
