using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ModLoader;

namespace AmmoCycle {
	class FixAmmoUseList {
		private static FixAmmoUseList fixAmmouseList;
		private List<Tuple<Item, Item>> ammoList = new List<Tuple<Item, Item>>();
		private static Mod mod = ModContent.GetInstance<AmmoCycle>();

		public static FixAmmoUseList Instance {
			get { return fixAmmouseList ?? (fixAmmouseList = new FixAmmoUseList()); }
		}

		public void AddAmmoPair(Item weapon, Item ammo) {
			RemoveAmmoPair(weapon);
			ammoList.Add(new Tuple<Item, Item>(weapon, ammo));
		}

		public void RemoveAmmoPair(Item weapon) {
			for (int i = 0; i < ammoList.Count; i++) {
				if (weapon.Name == ammoList[i].Item1.Name) {
					ammoList.RemoveAt(i);
					break;
				}
			}
		}

		public Item GetAmmoValue(Item weapon) {
			for (int i = 0; i < ammoList.Count; i++) {
				if (weapon.type == ammoList[i].Item1.type) {
					return ammoList[i].Item2;
				}
			}

			return new Item();
		}

		public void PrintList() {
			mod.Logger.DebugFormat("Fixed Ammo List contents");
			for (int i = 0; i < ammoList.Count; i++) {
				mod.Logger.DebugFormat("{0}. {1} | {2}", i, ammoList[i].Item1, ammoList[i].Item2.Name);
			}
		}

	}
}
