namespace levviatasprint.Common.Systems;

public class KeybindSystem : ModSystem
{
	public static ModKeybind showGUIKeybind { get; private set; }
    public static ModKeybind hideGUIKeybind { get; private set; }

    public override void Load() {
		showGUIKeybind = KeybindLoader.RegisterKeybind(Mod, "Show Gui", "Q");
        hideGUIKeybind= KeybindLoader.RegisterKeybind(Mod, "Hide Gui", "Q");
    }

	public override void Unload() {
        showGUIKeybind = null;
        hideGUIKeybind = null;
    }
}