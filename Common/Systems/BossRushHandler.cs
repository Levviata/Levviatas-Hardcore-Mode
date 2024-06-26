﻿using levviatasprint.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static levviatashardcoremode.Common.Players.LHCMPlayer;
using static levviatashardcoremode.Common.Systems.BossRushHandler;
using static levviatashardcoremode.Common.Systems.DeathHandler;
using static levviatashardcoremode.Common.Systems.BossRushTimer;

namespace levviatashardcoremode.Common.Systems
{
    public class BossRushHandler : ModSystem
    {
        internal UserInterface inter;
        internal BossRushTimer UI;

        public static bool playedCounterSong = false;

        private GameTime _lastUpdateUiGameTime;
        public static int timer;

        public static DateTime startTime;
        public static TimeSpan time;
        public static readonly int countDownStartTime = 120000;


        public override void Load()
        {
            if (!Main.dedServ)
            {
                inter = new UserInterface();

                UI = new BossRushTimer();
                UI.Activate(); // Activate calls Initialize() on the UIState if not initialized and calls OnActivate, then calls Activate on every child element.
            }
        }
        public override void Unload()
        {
            UI = null;
        }
        public override void OnWorldLoad()
        {
            shouldShowCounter = false;
        }
        internal void ShowMyUI()
        {
            inter?.SetState(UI);
        }
        internal void HideMyUI()
        {
            inter?.SetState(null);
        }
        public override void UpdateUI(GameTime gameTime)
        {
            timer++;
            _lastUpdateUiGameTime = gameTime;
            if (inter?.CurrentState != null)
            {
                inter.Update(gameTime);
            }
            if (KeybindSystem.showGUIKeybind.JustPressed)
            {
               
                ShowMyUI();
            }
            if (KeybindSystem.hideGUIKeybind.JustPressed)
            {
                HideMyUI();
            }

            if (shouldShowCounter)
            {
                 inter?.SetState(UI);
            }
            
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LHardcoreMode: Boss Rush Timer",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && inter?.CurrentState != null)
                        {
                            inter.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.None));
            }
        }
    }
    public class BossRushTimer : UIState 
    {

        int oneOffset = 14;
        

        int seconds;
        
        public static UIImage container = new UIImage(ModContent.Request<Texture2D>("levviatashardcoremode/Assets/UI/Font/Container/digitContainer"));

        public static UIImage text1 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/0")); //Counter never reaches >0 so no point in trying to find first digit lel


        static UIImage text2 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{0}"));

        static UIImage text3 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{0}"));
        
        static UIImage text4 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{0}"));
        
        static UIImage text5 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{0}"));
        
        static UIImage text6 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{0}"));
        static UIImage text7 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{0}"));

        public override void OnInitialize()
        {
            

            container.Top.Set(60, 0);
            container.Left.Set(Main.screenWidth / 2 + 115, 0);
            container.ImageScale = 3f;
            /*panel1.Width.Set(14, 0);
            panel1.Height.Set(24, 0);*/
            Append(container);

           
            text1.Top.Set(oneOffset, 0);
            text1.Left.Set(-115 + 16, 0);
            text1.ImageScale = 3f;
            container.Append(text1);

           
            text2.Top.Set(oneOffset, 0);
            text2.Left.Set(-54, 0);
            text2.ImageScale = 3f;
            container.Append(text2);

           
            text3.Top.Set(oneOffset, 0);
            text3.Left.Set(19 + 8, 0);
            text3.ImageScale = 3f;
            container.Append(text3);

            
            text4.Top.Set(oneOffset, 0);
            text4.Left.Set(64 + 8, 0);
            text4.ImageScale = 3f;
            container.Append(text4);
            
           
            text5.Top.Set(oneOffset, 0);
            text5.Left.Set(145 + 8, 0);
            text5.ImageScale = 3f;
            container.Append(text5);

          
            text6.Top.Set(oneOffset, 0);
            text6.Left.Set(198, 0);
            text6.ImageScale = 3f;
            container.Append(text6);

            /*text7.Top.Set(oneOffset, 0);
            text7.Left.Set(198, 0);
            text7.ImageScale = 3f;
            container.Append(text7);*/

            /*UIImage text2 = new UIImage(ModContent.Request<Texture2D>("levviatashardcoremode/Assets/UI/Font/1"));
            text2.Top.Set(2, 0);
            text2.Left.Set(-1, 0);
            text2.ImageScale = 4f;
            container.Append(text2);*/
            startTime = DateTime.Now;
        }
        public override void Update(GameTime gameTime)
        {
            if (timer > 1)
            {
                if (time.Minutes > 0 && time.Seconds > 0 && time.Milliseconds > 0)
                {
                    shouldShowCounter = false; // Set the flag to hide the counter
                }
                if (!playedCounterSong)
                {
                    playedCounterSong = true;
                    //SoundEngine.PlaySound(new SoundStyle("levviatashardcoremode/Assets/Music/redemption"));
                }
                if (!Main.gamePaused && shouldShowCounter)
                {
                    time = DateTime.Now - startTime;
                    /*Main.NewText(time.Minutes);
                    Main.NewText(time.Seconds);
                    Main.NewText(time.Milliseconds);*/
                    // Convert milliseconds to minutes, seconds, and milliseconds

                    int[] secondsList = GetDigits(time.Seconds, 2);
                    int[] millisecondsList = GetDigits(time.Milliseconds, 3);
                    int[] minutesList = GetDigits(time.Minutes, 1);
                    
                    text2.SetImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{minutesList[0]}"));
                    text3.SetImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{secondsList[0]}"));
                    text4.SetImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{secondsList[1]}"));
                    text5.SetImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{millisecondsList[0]}"));
                    text6.SetImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{millisecondsList[1]}"));

                    container.Append(text2);
                    container.Append(text3);
                    container.Append(text4);
                    container.Append(text5);
                    container.Append(text6);
                    Append(container);
                }
                timer = 0;
            }
        }

        public static int[] GetDigits(int number, int desiredLength)
        {
            // Check if the number is 0, then return an array containing only 0
            if (number == 0)
            {
                // Return an array of zeros with the desired length
                return Enumerable.Repeat(0, desiredLength).ToArray();
            }

            List<int> digits = new List<int>();

            // If the number is negative, treat its absolute value
            number = Math.Abs(number);

            // Extract digits
            while (number > 0)
            {
                int digit = number % 10;
                digits.Add(digit);
                number /= 10;
            }

            // Pad the list with zeros to ensure the desired length
            while (digits.Count < desiredLength)
            {
                digits.Add(0);
            }

            // Reverse the list to maintain the correct order of digits
            digits.Reverse();

            return digits.ToArray();
        }
    }
}
