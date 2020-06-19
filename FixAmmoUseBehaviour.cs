#define DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace AmmoCycle {
	class FixAmmoUseBehaviour : ModPlayer {
		private FixAmmoUseList fixedAmmoList = FixAmmoUseList.Instance;

		public override bool PreItemCheck() {

#if (DEBUG)
			mod.Logger.DebugFormat("Running AmmoFix behaviour");
#endif
			// Find current ammo used
			// Check with list the correct ammo to use
			// Find correct ammo to use
			// Cycle to correct ammo
			Item heldItem = player.HeldItem;

			if (heldItem.useAmmo == AmmoID.None) return true; // Held item not ammo using weapon

			Item[] inventory = player.inventory;
			Item currentAmmo = null;

			// Find currently used ammo
			for (int i = CONSTANTS.AMMOSLOTSTART; i != CONSTANTS.INVENTORYLENGTH; i++) {
				if (heldItem.useAmmo == inventory[i].ammo) {
					currentAmmo = inventory[i];
					break;
				}
			}

			if (currentAmmo == null) return base.PreItemCheck(); // Could not find ammo in inventory

			Item ammoToUse = fixedAmmoList.GetAmmoValue(heldItem);

#if (DEBUG)
			if (ammoToUse == null) {
				mod.Logger.DebugFormat("Failed to find ammo to use");
			}
			else {
				mod.Logger.DebugFormat("Ammo to change to: {0} {1}", ammoToUse.type, ammoToUse.Name);
			}
			
#endif

			if (currentAmmo.type == ammoToUse.type) return base.PreItemCheck(); // Correct ammo being used

			// Populate list with ammo in inventory
			List<Tuple<Item, int>> ammoList = new List<Tuple<Item, int>>();
			for (int i = CONSTANTS.AMMOSLOTSTART; i < CONSTANTS.AMMOSLOTEND; i++) {
				if (inventory[i].ammo == heldItem.useAmmo) {
					ammoList.Add(new Tuple<Item, int>(inventory[i], i));
				}
			}

			for (int i = 0; i < CONSTANTS.INVENTORYLENGTH; i++) {
				if (inventory[i].ammo == heldItem.useAmmo) {
					ammoList.Add(new Tuple<Item, int>(inventory[i], i));
				}
			}

			// Calculate rotations
			int rotations = 0;
			for (int i = 0; i < ammoList.Count; i++) {
				if (ammoList[i].Item1.type != ammoToUse.type) {
					rotations++;
				}
			}
#if (DEBUG)
			mod.Logger.DebugFormat("Rotations calculted: {0}", rotations);
#endif

			// Ammo to use not found in inventory
			if (rotations == ammoList.Count && ammoList[rotations].Item1.type != ammoToUse.type) {
#if (DEBUG)
				mod.Logger.DebugFormat("Could not find ammo to change to");
#endif
				return base.PreItemCheck(); }



			return base.PreItemCheck();
		}

	}
}
