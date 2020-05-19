#define DEBUG
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
	class SingleCycle : ModPlayer {

		// Magic numbers for inventory slot indexes
		private const int INVENTORYLENGTH = 50; 
		private const int AMMOSLOTSTART = 54;
		private const int AMMOSLOTEND = 58;

		public override void ProcessTriggers(TriggersSet triggersSet) {


			if (AmmoCycle.TriggerAmmoCycleNext.JustPressed) {
				CycleAmmo(true);
			}
			else if (AmmoCycle.TriggerAmmoCyclePrev.JustPressed) {
				CycleAmmo(false);
			}

		}

		private void CycleAmmo(bool forward) {
			
			int heldAmmoID = player.HeldItem.useAmmo;

			// Do nothing if player is not holding weapon that uses ammo
			if (heldAmmoID == AmmoID.None) {
				return;
			}

			// Handle coin ammo separately
			else if (heldAmmoID == AmmoID.Coin) { 
				return;
			}

			Item[] inventory = player.inventory;
			List<Tuple<Item, int>> ammoList = new List<Tuple<Item, int>>();

#if (DEBUG)
			Boolean isFirst = true;
			int currentAmmoi = 0;
#endif

			Item currentAmmo = null;
			

			for (int i = AMMOSLOTSTART; i < AMMOSLOTEND; i++) {
				if (inventory[i].ammo == heldAmmoID) {
					ammoList.Add(new Tuple<Item, int>(inventory[i], i));
#if (DEBUG)
					if (isFirst) // Save the first instance of ammo used in ammo inventory slots for debugging purposes.
					{
						isFirst = false;
						currentAmmoi = i;
					}
#endif
				}

			}

			currentAmmo = ammoList[0].Item1;

			for (int i = 0; i < INVENTORYLENGTH; i++) {

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

#if (DEBUG)
			mod.Logger.DebugFormat("currentAmmo  type: {0} | currentAmmo index: {1}", currentAmmo.type, currentAmmoi);
#endif

			if (ammoList.Count <= 1) {
				return;
			}

			if (currentAmmo == null) {
				mod.Logger.Warn("Could not find currentAmmo");
				return;
			}

			// Cycles ammo in their slots by calculating number of steps to shift by (n)
			// Then shift all elements n indexes to their new places

			int cycles;
			if (forward) {
				cycles = calculateRotations(ammoList);
			}
			
			else {
				cycles = calculateRotationsBack(ammoList);
			}
#if (DEBUG)
			mod.Logger.DebugFormat("Cycles: {0}", cycles);
#endif

			if (cycles == 0) {
				return;
			}

			Rotate(forward, cycles, ammoList);
			Main.PlaySound(SoundID.Camera,-1, -1, 1, 1f, 0.25f);
		}

		private int calculateRotations(List<Tuple<Item,int>> ammoList) {

			int ret = 1;
			while (ret < ammoList.Count && ammoList[ret].Item1.type == ammoList[0].Item1.type) {
				ret++;
			}

			return ret == ammoList.Count ? 0 : ret;
		}

		// Backwards rotation is just forward rotation of (.count - n) steps.
		private int calculateRotationsBack(List<Tuple<Item, int>> ammoList) {
			int ret = ammoList.Count - 1;

			while (ret > 0 
				&& (ammoList[ret].Item1.type == ammoList[ammoList.Count - 1].Item1.type 
				|| ammoList[ret].Item1.type == ammoList[0].Item1.type)) {
				ret--;
			}

			return ++ret;
		}
		
		private void Rotate(bool forward, int amount, List<Tuple<Item, int>> ammoList) {
			if (amount <= 0) {
				return;
			}
			
			Item[] inventory = player.inventory;

			Tuple<Item, int>[] tempAmmoArr = new Tuple<Item,int>[amount];

			// Add first n elements to be sent to back to temp array
			for (int i = 0; i < amount; i++) {
				tempAmmoArr[i] = ammoList[i];
			}

			// Bring forward (ammoList.count - n) number of elements to front
			for (int i = amount; i < ammoList.Count; i++) {
				inventory[ammoList[i - amount].Item2] = ammoList[i].Item1;

#if (DEBUG)
				mod.Logger.DebugFormat("Swap index {0}({2}) with {1}({3})",
						ammoList[i - 1].Item2, ammoList[i].Item2, ammoList[i - 1].Item1.type, ammoList[i].Item1.type);
#endif
			}

			// Replace last n elements of ammoList with original n elements.
			int bringForward = ammoList.Count - amount;
			for (int i = 0; i < amount; i++) {
				inventory[ammoList[bringForward].Item2] = tempAmmoArr[i].Item1;

#if (DEBUG)
				mod.Logger.DebugFormat("Swap index {0}({2}) with {1}({3})",
						ammoList[bringForward].Item2, tempAmmoArr[i].Item2, ammoList[bringForward].Item1.type, tempAmmoArr[i].Item1.type);
#endif

				bringForward++;
			}
		}

		// Legacy code (v0.1.2.1)
		// TODO: Create mod config to enable legacy behaviour
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
	}
}
