using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace levviatashardcoremode.Common.Systems
{
    public class PlayerDeathHandler : ModSystem
    {
        private Boolean hasRun = false;
        private SpriteBatch batch = Main.spriteBatch;
        internal UserInterface inter;
        internal DeathUI UI;
        private GameTime _lastUpdateUiGameTime;

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

    }
}
