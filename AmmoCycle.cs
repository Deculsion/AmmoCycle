using Terraria.ModLoader;

namespace AmmoCycle
{
	public class AmmoCycle : Mod
	{

        internal static ModHotKey TriggerAmmoCycleNext;
        internal static ModHotKey TriggerAmmoCyclePrev;

        public AmmoCycle()
		{
		}

        public override void Load()
        {
            TriggerAmmoCycleNext = RegisterHotKey("Cycle Ammo Next", "Q");
            TriggerAmmoCyclePrev = RegisterHotKey("Cycle Ammo Previous", "Mouse3");
        }
    }
}