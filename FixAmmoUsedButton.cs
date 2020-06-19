#define DEBUG
using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.ID;

namespace AmmoCycle {
	class FixAmmoUsedButton : ModPlayer {

		private FixAmmoUseList fixedAmmoList = FixAmmoUseList.Instance;

		public override void ProcessTriggers(TriggersSet triggersSet) {
			if (AmmoCycle.TriggerAmmoFix.JustPressed) {
				FixAmmo();
			}
		}

		private void FixAmmo() {
			Item currentWeapon = player.HeldItem;

#if (DEBUG)
			mod.Logger.DebugFormat("Current weapon: {0}", currentWeapon);
#endif

			if (currentWeapon.useAmmo == AmmoID.None) return;

			Item[] inventory = player.inventory;
			Item currentAmmo = null;

			// Find first usable ammo in inventory
			for (int i = CONSTANTS.AMMOSLOTSTART; i != CONSTANTS.INVENTORYLENGTH; i++) {
#if (DEBUGVERBOSE)
				mod.Logger.DebugFormat("i: {0} |.ammo: {1} | .useAmmo: {2}", i, inventory[i].ammo, currentWeapon.useAmmo);
#endif
				if (inventory[i].ammo == currentWeapon.useAmmo) {
					currentAmmo = inventory[i];
					break;
				}
				i %= CONSTANTS.AMMOSLOTEND; 
			}
#if (DEBUG)
			mod.Logger.DebugFormat("Ammo item found: {0}", currentAmmo == null ? "null" : currentAmmo.Name);
#endif

			// Register current weapon/ammo pair to first ammo found.
			if (currentAmmo != null &&
				fixedAmmoList.GetAmmoValue(currentWeapon) != null &&
				currentAmmo.type != fixedAmmoList.GetAmmoValue(currentWeapon).type) {
#if (DEBUG)
				mod.Logger.DebugFormat("Adding weapon/ammo pair {0}/{1}", currentWeapon, currentAmmo == null ? "null" : currentAmmo.Name);
#endif
				fixedAmmoList.AddAmmoPair(currentWeapon, currentAmmo);
				
			}

			else if (currentAmmo.type == fixedAmmoList.GetAmmoValue(currentWeapon).type) {
#if (DEBUG)
				mod.Logger.DebugFormat("Removing weapon/ammo pair {0}/{1}", currentWeapon, currentAmmo == null ? "null" : currentAmmo.Name);
#endif
				fixedAmmoList.RemoveAmmoPair(currentWeapon);
			}

			else {
				mod.Logger.InfoFormat("FixedAmmoUsed.cs reached else condition! {0} | {1}", currentWeapon, currentAmmo.Name);
			}

#if (DEBUG)
			fixedAmmoList.PrintList();
#endif

		}
	}
}
