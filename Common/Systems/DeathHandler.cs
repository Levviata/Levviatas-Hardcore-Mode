using levviatasprint.Common.Systems;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Terraria;
using Terraria.GameContent.RGB;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static levviatashardcoremode.Common.Players.LHCMPlayer;
using static levviatashardcoremode.Common.Systems.DeathHandler;
using static System.Net.Mime.MediaTypeNames;


namespace levviatashardcoremode.Common.Systems
{
    public class DeathHandler : ModSystem
    {
        private Boolean hasRun = false;
        private SpriteBatch batch = Main.spriteBatch;

        internal UserInterface interAbove;
        internal UserInterface interBelow;
        internal UserInterface interMarker;
        internal UserInterface interBackground;

        internal DeathUIAbove UIAbove;
        internal DeathUIBelow UIBelow;
        internal DeathUIMarker UIMarker;
        internal DeathUIBackground UIBackground;

        Boolean isPressed = false;

        private GameTime _lastUpdateUiGameTime;

        public float currentVolume;
        public static float initialVolume;

        private int timer = 0;
        private int timerUIAbove = 0;
        private int timerUIBelow = 0;

        public static int timerMarker = 0;
        public static int markerIndex = 0;

        public static int timerBackground = 0;

        public static int markerUpdateFrequency = 3;

        public static int aboveTop = 0;
        public static int increaseAbove = 0;

        public static int belowTop = 0;
        public static int increasebelow = 0;

        public static int backgroundAlpha = 0;

        
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
            if (timerMarker > markerUpdateFrequency)
            {
                UIMarker.Top.Set(Main.screenHeight / 2 - 128, 0);
                UIMarker.Left.Set(Main.screenWidth / 2 - 128, 0);
                UIMarker.HAlign = 0.5f;
                UIMarker.VAlign = 0.5f;
                interMarker?.SetState(UIMarker);

            }
        }

        internal void HideBars()
        {
            interBackground?.SetState(null);
            interMarker?.SetState(null);
            interAbove?.SetState(null);
            interBelow?.SetState(null);
        }
        public override void Load()
        {
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
            backgroundAlpha = 0;
            increaseAbove = 0;
            increasebelow = 0;
            aboveTop = 0;
            belowTop = 0;
            markerIndex = 0;
            playerDead = false;
            initialVolume = Main.musicVolume;
        }
        public override void OnWorldUnload()
        {

            Main.musicVolume = initialVolume;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            timer++;
            timerUIAbove++;
            timerUIBelow++;
            timerMarker++;
            timerBackground++;

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
            if (timer >= 10)
            {
                if (!Main.gamePaused) //trampoline
                    if (playerDead) //weee
                        Main.musicVolume -= 0.10f; //AAAAAAAAAAA
                timer = 0;
            }

            //if (playerDead) Main.blockInput = true;

            //Marker handling

            
            // Debugging, useless for now

            if (KeybindSystem.showGUIKeybind.JustPressed)
            {
                isPressed = !isPressed;
                ShowBars();
            }

            if (KeybindSystem.hideGUIKeybind.JustPressed)
            {
                isPressed = false;
                HideBars();
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
            if (timerMarker > markerUpdateFrequency)
            {

                timerMarker = 0; //We update it here and not elsewhere like our UIState to prevent desync
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            int resourceBarsIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

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
        public class DeathUIMarker : UIState
        {

            private UIImage panelAnimated = new UIImage(ModContent.Request<Texture2D>("levviatashardcoremode/Assets/UI/Marker1"));
            public override void OnInitialize()
            {
                Append(panelAnimated);
            }
            public override void Update(GameTime gameTime)
            {
                if (markerIndex < 11)
                {
                    if (timerMarker > markerUpdateFrequency)
                    {
                        markerIndex++;
                        panelAnimated.SetImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Marker{markerIndex}"));
                        Append(panelAnimated);

                    }
                }
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
                if (backgroundAlpha > 180)
                {
                    if (timerBackground > 1)
                    {
                        backgroundAlpha += 3;
                        panelBackground.BackgroundColor = new Color(0, 0, 0, backgroundAlpha);
                        Append(panelBackground);

                    }
                }
            }
        }
    }
}
