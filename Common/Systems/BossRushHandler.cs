using levviatasprint.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static levviatashardcoremode.Common.Systems.BossRushHandler;

namespace levviatashardcoremode.Common.Systems
{
    public class BossRushHandler : ModSystem
    {
        internal UserInterface inter;
        internal BossRushTimer UI;

        private GameTime _lastUpdateUiGameTime;

        public static int timer;
        public static int milliseconds = 147600;
        
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
        public override void OnInitialize()
        {
            TimeSpan time = TimeSpan.FromMilliseconds(milliseconds);

            UIImage container = new UIImage(ModContent.Request<Texture2D>("levviatashardcoremode/Assets/UI/Font/Container/digitContainer"));
            container.Top.Set(60, 0);
            container.Left.Set(Main.screenWidth / 2 + 115, 0);
            container.ImageScale = 3f;
            /*panel1.Width.Set(14, 0);
            panel1.Height.Set(24, 0);*/
            Append(container);

            UIImage text1 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/0")); //Counter never reaches >0 so no point in trying to find first digit lel
            text1.Top.Set(oneOffset, 0);
            text1.Left.Set(-115 + 16, 0);
            text1.ImageScale = 3f;
            container.Append(text1);

            UIImage text2 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{GetNthDigit(time.Minutes, 1)}"));
            text2.Top.Set(oneOffset, 0);
            text2.Left.Set(-54, 0);
            text2.ImageScale = 3f;
            container.Append(text2);

            UIImage text3 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{GetFirstDigit(time.Seconds)}"));
            text3.Top.Set(oneOffset, 0);
            text3.Left.Set(-64 + 8, 0);
            text3.ImageScale = 3f;
            container.Append(text3);

            /*UIImage text4 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{GetSecondDigit(time.Seconds)}"));
            text4.Top.Set(oneOffset, 0);
            text4.Left.Set(-49 + 8, 0);
            text4.ImageScale = 3f;
            container.Append(text4);

            UIImage text5 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{GetFirstDigit(time.Milliseconds)}"));
            text5.Top.Set(oneOffset, 0);
            text5.Left.Set(-22 + 8, 0);
            text5.ImageScale = 3f;
            container.Append(text5);

            UIImage text6 = new UIImage(ModContent.Request<Texture2D>($"levviatashardcoremode/Assets/UI/Font/{GetSecondDigit(time.Milliseconds)}"));
            text6.Top.Set(oneOffset, 0);
            text6.Left.Set(-8, 0);
            text6.ImageScale = 3f;
            container.Append(text6);*/

            /*UIImage text2 = new UIImage(ModContent.Request<Texture2D>("levviatashardcoremode/Assets/UI/Font/1"));
            text2.Top.Set(2, 0);
            text2.Left.Set(-1, 0);
            text2.ImageScale = 4f;
            container.Append(text2);*/
        }
        static int GetNthDigit(int n, int position)
        {
            // Convert integer to string
            string numStr = n.ToString();

            // Ensure the position is within the range of the number's digits
            if (position < 1 || position > numStr.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Position is out of range.");
            }

            // Extract the character at the specified position
            char digitChar = numStr[position - 1]; // Adjust position to 0-based indexing

            // Convert the character back to integer
            int digit = (int)Char.GetNumericValue(digitChar);

            return digit;
        }
    }
}
