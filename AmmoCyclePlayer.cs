﻿using System;
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

			// Cycles ammo in their slots by calculating number of steps to shift by (n)
			// Then shift all elements n indexes to their new places

			int cycles;
			if (forward) {
				cycles = calculateRotations(ammoList);
				mod.Logger.DebugFormat("Cycles: {0}", cycles);

				if (cycles == 0) {
					return;
				}
			}
			
			else {
				cycles = calculateRotationsBack(ammoList);
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

		private int calculateRotationsBack(List<Tuple<Item, int>> ammoList) {
			int ret = ammoList.Count - 1;

			while (ret > 0 && ammoList[ret].Item1.type == ammoList[ret - 1].Item1.type) {
				ret--;
			}

			return ret;
		}
		
		private void Rotate(bool forward, int amount, List<Tuple<Item, int>> ammoList) {
			if (amount <= 0) {
				return;
			}
			
			Item[] inventory = player.inventory;

			if (forward) {
				Tuple<Item, int>[] tempAmmoArr = new Tuple<Item,int>[amount];

				// Add first n elements to be sent to back to temp array
				for (int i = 0; i < amount; i++) {
					tempAmmoArr[i] = ammoList[i];
				}

				// Bring forward (ammoList.count - n) number of elements to front
				for (int i = amount; i < ammoList.Count; i++) {
					inventory[ammoList[i - amount].Item2] = ammoList[i].Item1;

					mod.Logger.DebugFormat("Swap index {0}({2}) with {1}({3})",
							ammoList[i - 1].Item2, ammoList[i].Item2, ammoList[i - 1].Item1.type, ammoList[i].Item1.type);
				}

				// Replace last n elements of ammoList with original n elements.
				int bringForward = ammoList.Count - amount;
				for (int i = 0; i < amount; i++) {
					inventory[ammoList[bringForward].Item2] = tempAmmoArr[i].Item1;

					mod.Logger.DebugFormat("Swap index {0}({2}) with {1}({3})",
							ammoList[bringForward].Item2, tempAmmoArr[i].Item2, ammoList[bringForward].Item1.type, tempAmmoArr[i].Item1.type);

					bringForward++;
				}
			}

			else {
				Tuple<Item, int>[] tempAmmoArr = new Tuple<Item, int>[amount];

				for ()
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
