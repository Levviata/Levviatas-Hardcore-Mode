using levviatasprint.Common.Systems;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.RGB;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static Humanizer.In;
using static levviatashardcoremode.Common.Players.LHCMPlayer;
using static levviatashardcoremode.Common.Systems.DeathHandler;
using static System.Net.Mime.MediaTypeNames;


namespace levviatashardcoremode.Common.Systems
{
	public class DeathHandler : ModSystem
	{
		private Boolean hasRun = false;
		private SpriteBatch batch = Main.spriteBatch;
        public static Player[] playerVisualClone = new Player[256];

        internal UserInterface interAbove;
		internal UserInterface interBelow;
		internal UserInterface interMarker;
		internal UserInterface interBackground;

		internal DeathUIAbove UIAbove;
		internal DeathUIBelow UIBelow;
		internal DeathUIMarker UIMarker;
		internal DeathUIBackground UIBackground;

		Boolean isPressed = false;
		public static Boolean played = false;
		public static Boolean shootPlayed = false;
		public static Boolean shouldStartTimerShoot;
		public static Boolean shouldKillPlayer = false;

		private GameTime _lastUpdateUiGameTime;

		public float currentVolume;
		public static float initialVolume;
		public static float initialUISize;

        public static int timerShoot = 0;
		private int timerMusic = 0;
		private int timerUIAbove = 0;
		private int timerUIBelow = 0;
        private int timerUIBelowOut = 0;
        private int timerUIAboveOut = 0;
        public static int timerMarker = 0;
		public static int afterTimerMarker = 0;


        public static int markerIndex;
		public static int afterMarkerIndex;

		public static int timerBackground = 0;

		public static int markerUpdateFrequency = 1;

		public static int aboveTop = 0;
		public static int increaseAbove = 0;

		public static int aboveTopOut = 0;
		public static int increaseAboveOut = 0;

		public static int belowTop = 0;
		public static int increaseBelow = 0;

		public static int belowTopOut = 0;
		public static int increaseBelowOut = 0;

		public static int backgroundAlpha = 0;

		public static List<GameInterfaceLayer> initialLayers;
		internal void ShowBars()
		{
			interBackground?.SetState(UIBackground);
			interMarker?.SetState(UIMarker);
			interAbove?.SetState(UIAbove);
			interBelow?.SetState(UIBelow);
			if (aboveTop < 150)
			{
				if (timerUIAbove > 1)
				{
					aboveTop += 1 * increaseAbove;
					increaseAbove += 1;

					UIAbove.Top.Set(aboveTop, 0);
					interAbove?.SetState(UIAbove);
					timerUIAbove = 0;
				}
			}

			if (belowTop < 150)
			{
				if (timerUIBelow > 1)
				{
					belowTop += 1 * increaseBelow;
					increaseBelow += 1;

					UIBelow.Top.Set(-belowTop, 0);
					interBelow?.SetState(UIBelow);
					timerUIBelow = 0;
				}
			}
			if (timerMarker > markerUpdateFrequency)
			{
				UIMarker.Top.Set(Main.screenHeight / 2 - 128, 0);
				UIMarker.Left.Set(Main.screenWidth / 2 - 128, 0);
				UIMarker.HAlign = 0.5f;
				UIMarker.VAlign = 0.5f;
				interMarker?.SetState(UIMarker);

			}
		}
		/*internal void OutBars()
		{
			if (timerUIAboveOut > 1)
			{
				aboveTopOut -= 1 * increaseAboveOut;
				increaseAboveOut = 1;

				UIAbove.Top.Set(aboveTopOut, 0);
				interAbove?.SetState(UIAbove);
                timerUIAboveOut = 0;
			}
			if (timerUIBelowOut > 1)
			{
                belowTopOut -= 1 * increaseBelowOut;
                increaseBelowOut += 1;

				UIBelow.Top.Set(-belowTop, 0);
				interBelow?.SetState(UIBelow);
                timerUIBelowOut = 0;
			}

			/*if (timerMarker > markerUpdateFrequency)
			{
				UIMarker.Top.Set(Main.screenHeight / 2 - 128, 0);
				UIMarker.Left.Set(Main.screenWidth / 2 - 128, 0);
				UIMarker.HAlign = 0.5f;
				UIMarker.VAlign = 0.5f;
				interMarker?.SetState(UIMarker);

			}
		}*/

        internal void HideBars()
		{
			interBackground?.SetState(null);
			interMarker?.SetState(null);
			interAbove?.SetState(null);
			interBelow?.SetState(null);
		}
		public override void Load()
		{
			shouldStartTimerShoot = false;
			if (!Main.dedServ)
			{
				interBackground = new UserInterface();
				interAbove = new UserInterface();
				interBelow = new UserInterface();
				interMarker = new UserInterface();

				UIBackground = new DeathUIBackground();
				UIBelow = new DeathUIBelow();
				UIAbove = new DeathUIAbove();
				UIMarker = new DeathUIMarker();

				UIBackground.Activate();
				UIAbove.Activate(); // Activate calls Initialize() on the UIState if not initialized and calls OnActivate, then calls Activate on every child element.
				UIBelow.Activate();
				UIMarker.Activate();

			}
		}
		public override void Unload()
		{
			UIBackground = null;
			UIAbove = null;
			UIBelow = null;
			UIMarker = null;
		}
		public override void OnWorldLoad()
		{
			afterMarkerIndex = 0;
            played = false;
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
			initialVolume = Main.musicVolume;
			shootPlayed = false;
			initialUISize = Main.UIScale;
            if (Main.netMode != NetmodeID.Server && Filters.Scene["levviatashardcoremode:Shockwave"].IsActive())
            {
                Filters.Scene["levviatashardcoremode:Shockwave"].Deactivate();
            }

            Main.mapMinimapAlpha = 1f;
            Main.mapEnabled = true;
            Main.mapOverlayAlpha = 0.35f;
            Main.LocalPlayer.lifeRegen = Main.LocalPlayer.lifeRegen;
            Main.blockInput = false;
            Main.LocalPlayer.immuneNoBlink = false;
        }
		public override void OnWorldUnload()
		{

			Main.musicVolume = initialVolume;
		}
		public override void UpdateUI(GameTime gameTime)
		{
			timerMusic++;
			timerUIAbove++;
			timerUIBelow++;
			timerMarker++;
			timerBackground++;
			timerShoot++;
			afterTimerMarker++;
			timerUIBelowOut++;
			timerUIAboveOut++;

            _lastUpdateUiGameTime = gameTime;
			if (interAbove?.CurrentState != null)
			{
				interAbove.Update(gameTime);
			}
			if (interBelow?.CurrentState != null)
			{
				interBelow.Update(gameTime);
			}
			if (interMarker?.CurrentState != null)
			{
				interMarker.Update(gameTime);
			}
			if (interBackground?.CurrentState != null)
			{
				interBackground.Update(gameTime);
			}


			//Music handling

			if (Main.musicVolume <= 0) Main.musicVolume = 0;
			if (timerMusic >= 10)
			{
				if (!Main.gamePaused) //trampoline
					if (playerDead) //weee
						Main.musicVolume -= 0.10f; //AAAAAAAAAAA
				timerMusic = 0;
			}

			if (playerDead) 
			{  
				//Main.blockInput = true;
			}

            if (Main.netMode != NetmodeID.Server && Filters.Scene["levviatashardcoremode:Shockwave"].IsActive())
            {
                float progress = 180f / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                Filters.Scene["levviatashardcoremode:Shockwave"].GetShader().UseProgress(progress).UseOpacity(100f * (1 - progress / 3f));
            }

            // Debugging, useless for now

            if (KeybindSystem.showGUIKeybind.JustPressed)
			{
				isPressed = !isPressed;
                if (Main.netMode != NetmodeID.Server && !Filters.Scene["levviatashardcoremode:Shockwave"].IsActive())
                {
                    Filters.Scene.Activate("levviatashardcoremode:Shockwave", Main.LocalPlayer.Center).GetShader().UseColor(3, 5, 15).UseTargetPosition(Main.LocalPlayer.Center);
                }
            }

			if (KeybindSystem.hideGUIKeybind.JustPressed)
			{
                Filters.Scene["levviatashardcoremode:Shockwave"].Deactivate();
                isPressed = false;
			}

			//Makes things work

			if (Main.LocalPlayer.statLife > 2) //Dont change value to anything below 2
			{
				HideBars();
			}
			if (Main.LocalPlayer.statLife <= 0)
			{
				ShowBars();
			   
			}

			//Handles fade out of the UI when respawning

			if (Main.LocalPlayer.respawnTimer <= 5 && Main.LocalPlayer.respawnTimer != 0)
			{
				//OutBars();
            }

			if (timerMarker > markerUpdateFrequency)
			{
				timerMarker = 0; //We update it here and not elsewhere like our UIState to prevent desync
			}

			if (afterTimerMarker > 2)
			{
				afterTimerMarker = 0;
			}
        }
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int resourceBarsIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			// Store a copy of the original layers
			initialLayers = new List<GameInterfaceLayer>(layers);

			// Define the indices of the layers to keep
			List<int> indicesToRemove = new List<int> { 34, 36, 38, 39};

			// Create a new list with only the layers to keep
			List<GameInterfaceLayer> newLayers = new List<GameInterfaceLayer>();
			if (Main.LocalPlayer.statLife > 2 || KeybindSystem.hideGUIKeybind.JustPressed) //Dont change value to anything below 2
			{
				layers.AddRange(initialLayers);
			}
			if (Main.LocalPlayer.statLife <= 0) 
			{
				Main.mapMinimapAlpha = 0;
				Main.mapEnabled = false;
				Main.mapOverlayAlpha = 0;

				// Iterate through the list in reverse order
				for (int i = layers.Count - 1; i >= 0; i--)
				{
					// If the current index is in the list of indices to remove, remove the layer
					if (indicesToRemove.Contains(i))
					{
						layers.RemoveAt(i);
					}
				}
			}

			layers.Insert(51, new LegacyGameInterfaceLayer(
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
					/*if (_lastUpdateUiGameTime != null && interMarker?.CurrentState != null)
					{
						interMarker.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
					}*/
					return true;
				},
				InterfaceScaleType.UI));
			layers.Insert(50, new LegacyGameInterfaceLayer(
			   "LHardcoreMode: Marker",
			   delegate
               {
				   if (playerDead) {
                       if (Main.netMode != NetmodeID.Server && !Filters.Scene["levviatashardcoremode:Shockwave"].IsActive())
                       {
                           Filters.Scene.Activate("levviatashardcoremode:Shockwave", Main.LocalPlayer.Center).GetShader().UseColor(3, 5, 15).UseTargetPosition(Main.LocalPlayer.Center);
                       }
                       if (Main.netMode != NetmodeID.Server && Filters.Scene["levviatashardcoremode:Shockwave"].IsActive())
                       {
                           float progress = 180f / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                           Filters.Scene["levviatashardcoremode:Shockwave"].GetShader().UseProgress(progress).UseOpacity(100f * (1 - progress / 3f));
                       }
                       Main.UIScale = 1;
					   int playerIndex = Main.LocalPlayer.whoAmI;
					   Player player = Main.LocalPlayer;
					   if (playerVisualClone[playerIndex] == null)
					   {
						   playerVisualClone[playerIndex] = new Player();
					   }
					   Player clonePlayer = playerVisualClone[playerIndex];
					   clonePlayer.CopyVisuals(player);
					   clonePlayer.ResetEffects();
					   clonePlayer.ResetVisibleAccessories();
					   clonePlayer.DisplayDollUpdate();
					   clonePlayer.UpdateSocialShadow();
					   clonePlayer.Center = player.Center;
					   clonePlayer.direction = player.direction;
					   clonePlayer.wingFrame = player.wingFrame;
					   clonePlayer.PlayerFrame();
					   clonePlayer.socialIgnoreLight = true;
					   Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.Center - new Vector2(10, 20), 0f, player.fullRotationOrigin, 0, 1f);
                   }
                   if (_lastUpdateUiGameTime != null && interMarker?.CurrentState != null)
				   {
					   interMarker.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
				   }
				   return true;
			   },
			   InterfaceScaleType.UI));

			layers.Insert(49, new LegacyGameInterfaceLayer(
				   "LHardcoreMode: Background",
				   delegate
				   {
					   if (_lastUpdateUiGameTime != null && interBackground?.CurrentState != null)
					   {
						   interBackground.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
					   }
					   return true;
				   },
				   InterfaceScaleType.UI));
			initialLayers = layers;
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
				panelBelow.Top.Set(Main.screenHeight + 150, 0);
				panelBelow.Left.Set(-50, 0);
				panelBelow.MaxWidth.Set(Main.screenWidth + 400, 0);
				panelBelow.Width.Set(Main.screenWidth + 400, 0);
				panelBelow.Height.Set(300, 0);
				panelBelow.BackgroundColor = new Color(0, 0, 0, 255);
				Append(panelBelow);
			}
		}
		public class DeathUIMarker : UIState
		{
			private UIImage panelAnimated = new UIImage(ModContent.Request<Texture2D>("levviatashardcoremode/Assets/UI/Markers/Marker1"));
			public override void OnInitialize()
			{
				Append(panelAnimated);
			}
			public override void Update(GameTime gameTime)
			{
                if (Main.LocalPlayer.statLife > 2) //Dont change value to anything below 2
                {
                    panelAnimated.SetImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Markers/Marker1"));
                    Append(panelAnimated);
                }
                if (timerMarker > markerUpdateFrequency)
                {
                    markerIndex++;
                    if (markerIndex <= 13 && playerDead)
                    {
                        panelAnimated.SetImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Markers/Marker{markerIndex}"));
                        Append(panelAnimated);
                        if (!played)
                        {
                            played = true;
                            //Main.chatMonitor.NewText($"{timerShoot}");
                            SoundEngine.PlaySound(new SoundStyle("levviatashardcoremode/Assets/Sounds/reloadSound"), Main.LocalPlayer.position);
                            shouldStartTimerShoot = true;
                        }
                    }
                }

				if (markerIndex >= 45)
				{
                    if (afterTimerMarker > 2)
					{
						afterMarkerIndex++;
                        if (afterMarkerIndex <= 7 && playerDead)
                        {
                            panelAnimated.SetImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Markers/After/AfterMark{afterMarkerIndex}"));
                            Append(panelAnimated);
                            if (!shootPlayed)
                            {
                                PlayerDeathReason death = PlayerDeathReason.ByCustomReason($"{Main.LocalPlayer.name} was obliterated from this world.");
                                shootPlayed = true;
                                shouldKillPlayer = true;
                                Main.LocalPlayer.KillMe(death, 4444, 0);
                                SoundEngine.PlaySound(new SoundStyle("levviatashardcoremode/Assets/Sounds/shootSound"), Main.LocalPlayer.position);
                            }
                        }
                    }
				}
					
				/*if (SoundEngine.TryGetActiveSound(slotId, out var sound))
				{
					if (sound != null)
					{
						if (timerShoot % 65 == 0)
						{
							
						}
					}
				}*/
			}
		}
		public class DeathUIBackground : UIState
		{
			private UIPanel panelBackground = new UIPanel();

			public override void OnInitialize()
			{ 
			   
				panelBackground.Top.Set(-150, 0);
				panelBackground.Left.Set(-150, 0);
				panelBackground.MaxHeight.Set(4000, 0);
				panelBackground.MaxWidth.Set(2080, 0);
				panelBackground.Width.Set(4000, 0);
				panelBackground.Height.Set(2080, 0);
				panelBackground.BackgroundColor = new Color(0, 0, 0, 0);

				Append(panelBackground);
			}
			public override void Update(GameTime gameTime)
			{
				if (backgroundAlpha < 100)
				{
					if (timerBackground > 1)
					{
						backgroundAlpha += 3;
						panelBackground.BackgroundColor = new Color(0, 0, 0, backgroundAlpha);
						Append(panelBackground);

					}
				}
				else backgroundAlpha = 100;
			}
		}
	}
}
