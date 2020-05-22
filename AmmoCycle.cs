using Terraria.ModLoader;

namespace AmmoCycle
{
	public class AmmoCycle : Mod
	{
        public static AmmoCycle instance;

        internal static ModHotKey TriggerAmmoCycleNext;
        internal static ModHotKey TriggerAmmoCyclePrev;
        internal static ModHotKey TriggerAmmoFix;

        public AmmoCycle()
		{
		}

        public override void Load()
        {
            TriggerAmmoCycleNext = RegisterHotKey("Cycle Ammo Next", "Q");
            TriggerAmmoCyclePrev = RegisterHotKey("Cycle Ammo Previous", "Mouse3");
            TriggerAmmoFix = RegisterHotKey("Fix Ammo Used", "J");
        }

        public override void Unload() {
            instance = null;
            base.Unload();
        }
    }
}