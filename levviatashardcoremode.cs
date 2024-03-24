using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace levviatashardcoremode
{
	public class levviatashardcoremode : Mod
	{
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> screenRef = new Ref<Effect>(this.Assets.Request<Effect>("Assets/Effects/ShockwaveEffect", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.
                Filters.Scene["levviatashardcoremode:Shockwave"] = new Filter(new ScreenShaderData(screenRef, "levviatashardcoremode:Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["levviatashardcoremode:Shockwave"].Load();
            }
        }
    }
}