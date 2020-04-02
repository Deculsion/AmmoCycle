using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.ID;

namespace AmmoCycle {
	class AmmoCyclePlayer : ModPlayer {

		private const int inventoryLength = 50;
		private const int ammoSlotStart = 54;
		private const int ammoSlotEnd = 58;

		public override void ProcessTriggers(TriggersSet triggersSet) {


			if (AmmoCycle.TriggerAmmoCycleNext.JustPressed) {
				CycleAmmo(true);
			}
			else if (AmmoCycle.TriggerAmmoCyclePrev.JustPressed) {
				CycleAmmo(false);
			}


		}

		private void CycleAmmo(bool forward) {

			// Do nothing if player is not holding weapon that uses ammo
			int heldAmmoID = player.HeldItem.useAmmo;

			if (heldAmmoID == AmmoID.None) {
				return;
			}

			else if (heldAmmoID == AmmoID.Coin) { // Handle coin ammo separately
				return;
			}

			Item[] inventory = player.inventory;
			List<Tuple<Item, int>> ammoList = new List<Tuple<Item, int>>();

			Boolean isFirst = true;
			Item currentAmmo = null;
			int currentAmmoi = 0;

			for (int i = ammoSlotStart; i < ammoSlotEnd; i++) {
				if (inventory[i].ammo == heldAmmoID) {
					ammoList.Add(new Tuple<Item, int>(inventory[i], i));

					if (isFirst) // Save the first instance of ammo used in ammo inventory slots.
					{
						isFirst = false;
						currentAmmo = inventory[i];
						currentAmmoi = i;

					}
				}
			}

			for (int i = 0; i < inventoryLength; i++) {

				if (inventory[i].ammo == heldAmmoID) {

					ammoList.Add(new Tuple<Item, int>(inventory[i], i));

					if (isFirst) // Save the first instance of ammo used if ammo slots are empty.
					{
						isFirst = false;
						currentAmmo = inventory[i];
						currentAmmoi = i;
						
					}
				}

			}

			mod.Logger.DebugFormat("currentAmmo  type: {0} | currentAmmo index: {1}", currentAmmo.type, currentAmmoi);

			if (ammoList.Count <= 1) {
				return;
			}

			if (currentAmmo == null) {
				mod.Logger.Warn("Could not find currentAmmo");
				return;
			}

			// Cycles ammo in their slots by shifting first ammo item in inventory to the last place
			// Then push other ammo items forward to replace previous slot.
			// TODO: Make rotations play a sound
			// TODO: Rotate additional times if there are multiple stacks of the same ammo.

			//int maxCycles = ammoList.Count + 1;

			//do {
			//	Rotate(forward, ammoList);
			//	maxCycles--;
			//} while (maxCycles > 0 && !isDoneRotating(currentAmmo));

			int cycles = calculateRotations(ammoList);
			mod.Logger.DebugFormat("Cycles: {0}", cycles);

			Rotate(forward, cycles, ammoList);
		}

		private int calculateRotations(List<Tuple<Item,int>> ammoList) {

			int ret = 1;
			while (ret < ammoList.Count && ammoList[ret].Item1.type == ammoList[0].Item1.type) {
				ret++;
			}

			return ret == ammoList.Count-1 ? 0 : ret;
		}
		
		private void Rotate(bool forward, int amount, List<Tuple<Item, int>> ammoList) {
			
			Item[] inventory = player.inventory;

			if (forward) {

				while(amount --> 0) {
					Tuple<Item, int> firstAmmo = ammoList[0];

					for (int i = 1; i < ammoList.Count; i++) {
						inventory[ammoList[i - 1].Item2] = ammoList[i].Item1;
						mod.Logger.DebugFormat("Swap index {0}({2}) with {1}({3})", 
							ammoList[i - 1].Item2, ammoList[i].Item2, ammoList[i-1].Item1.type, ammoList[i].Item1.type);
					}

					

					inventory[ammoList[ammoList.Count - 1].Item2] = firstAmmo.Item1;
					mod.Logger.DebugFormat("Replace index {0}({2}) with {1}({3})",
						ammoList[ammoList.Count - 1].Item2, firstAmmo.Item2, ammoList[ammoList.Count - 1].Item1.type, firstAmmo.Item1.type);
					mod.Logger.Debug("Rotate.");
				}

			}
		}
		
		private void Rotate(bool forward, List<Tuple<Item, int>> ammoList) {

			Item[] inventory = player.inventory;

			if (forward) {
				Tuple<Item, int> firstAmmo = ammoList[0];

				for (int i = 1; i < ammoList.Count; i++) {
					inventory[ammoList[i - 1].Item2] = ammoList[i].Item1;
				}

				inventory[ammoList[ammoList.Count - 1].Item2] = firstAmmo.Item1;

			}

			else {
				Tuple<Item, int> lastAmmo = ammoList[ammoList.Count - 1];

				for (int i = 0; i < ammoList.Count - 1; i++) {
					inventory[ammoList[i + 1].Item2] = ammoList[i].Item1;
				}

				inventory[ammoList[0].Item2] = lastAmmo.Item1;

			}
		}

		
		// Checks the first ammo in inventory with the original first ammo.
		private bool isDoneRotating(Item currentAmmo) {

			Item[] inventory = player.inventory;

			for (int i = ammoSlotStart; i < ammoSlotEnd; i++) {
				if (inventory[i].ammo != AmmoID.None) {
					if (inventory[i].type == currentAmmo.type) { // Check ammo slots for first instance of ammo.
						mod.Logger.DebugFormat("isDoneRotating(): false | Slot {0}", i);
						return false;
					}

					else {
						mod.Logger.DebugFormat("isDoneRotating(): true | Slot {0} | i type: {1}", i, inventory[i].type);
						return true;
					}
				}
			}

			for (int i = 0; i < inventoryLength; i++) {
				if (inventory[i].ammo != AmmoID.None) {
					if (inventory[i].type == currentAmmo.type) { // Check inventory for first instance.
						mod.Logger.Debug("isDoneRotating returned false in inventory check");
						return false;
					}

					else {
						return true;
					}
				}

			}

			mod.Logger.Warn("Reached end of isDoneRotating()");
			return true;
		}
		
		// Check ignores non traditional ammo types like coins which are handled separately.
		private bool isAmmo(Item item) {
			if (item.ammo != AmmoID.None) {
				return !item.notAmmo;
			}

			return false;
		}
		
	}
}
