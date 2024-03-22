using levviatasprint.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI; 

namespace levviatashardcoremode.Common.Systems
{
	public class DeathHandler : ModSystem
	{
		private Boolean hasRun = false;
		private SpriteBatch batch = Main.spriteBatch;
		internal UserInterface inter;
		internal DeathUI UI;
		Boolean isPressed = false;
		private GameTime _lastUpdateUiGameTime;
		internal void ShowMyUI()
		{
			inter?.SetState(UI);
		}

		internal void HideMyUI()
		{
			inter?.SetState(null);
		}
		public override void Load()
		{
			if (!Main.dedServ)
			{
				inter = new UserInterface();

				UI = new DeathUI();
				UI.Activate(); // Activate calls Initialize() on the UIState if not initialized and calls OnActivate, then calls Activate on every child element.
			}
		}
		public override void Unload()
		{
			UI = null;
		}
		public override void UpdateUI(GameTime gameTime)
		{
			_lastUpdateUiGameTime = gameTime;
			if (inter?.CurrentState != null)
			{
				inter.Update(gameTime);
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

			//The thing that makes it work

			if (Main.LocalPlayer.statLife > 0) //Alive, boooooo
			{
				if (!isPressed)
					HideMyUI();
			}
			if (Main.LocalPlayer.dead) //Dead yipeeee
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
						if (_lastUpdateUiGameTime != null && inter?.CurrentState != null)
						{
							inter.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
						}
						return true;
					},
					InterfaceScaleType.UI));
			}
		}
	}
	public class DeathUI : UIState
	{
		public override void OnInitialize()
		{
			UIPanel panelAbove = new UIPanel(Main.Assets.Request<Texture2D>("Images/UI/PanelBackground"), ModContent.Request<Texture2D>("levviatashardcoremode/Assets/UI/PanelBorder"), 12, 0);
			panelAbove.Top.Set(-150, 0);
			panelAbove.Left.Set(-50, 0);
			panelAbove.MaxWidth.Set(Main.screenWidth + 400, 0);
			panelAbove.Width.Set(Main.screenWidth + 400, 0);
			panelAbove.Height.Set(300, 0);
			panelAbove.BackgroundColor = new Color(0, 0, 0, 255);
			Append(panelAbove);

			UIPanel panelBelow = new UIPanel(Main.Assets.Request<Texture2D>("Images/UI/PanelBackground"), ModContent.Request<Texture2D>("levviatashardcoremode/Assets/UI/PanelBorder"), 12, 0);
			panelBelow.Top.Set(Main.screenHeight - 150, 0);
			panelBelow.Left.Set(-50, 0);
			panelBelow.MaxWidth.Set(Main.screenWidth + 400, 0);
			panelBelow.Width.Set(Main.screenWidth + 400, 0);
			panelBelow.Height.Set(300, 0);
			panelBelow.BackgroundColor = new Color(0, 0, 0, 255);
			Append(panelBelow);
		}

	}
}
