using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.ID;

namespace AmmoCycle
{
    class AmmoCyclePlayer : ModPlayer
    {

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (AmmoCycle.TriggerAmmoCycleNext.JustPressed)
            {
                CycleAmmo(true);
            }

            else if (AmmoCycle.TriggerAmmoCyclePrev.JustPressed)
            {
                CycleAmmo(false);
            }


        }

        private void CycleAmmo(bool forward)
        {

            // Do nothing if player is not holding weapon that uses ammo
            int heldAmmoID = player.HeldItem.useAmmo;
            if (heldAmmoID == AmmoID.None)
            {
                return;
            }

            Item[] inventory = player.inventory;

            List<Tuple<Item, int>> ammoList = new List<Tuple<Item, int>>();
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].ammo == heldAmmoID)
                {
                    ammoList.Add(new Tuple<Item, int>(inventory[i], i));
                }

            }

            if (ammoList.Count <= 1)
            {
                return;
            }

            // Cycles ammo in their slots by shifting first ammo item in inventory to the last place
            // Then push other ammo items forward to replace previous slot.
            

            if (forward)
            {
                Tuple<Item, int> firstAmmo = ammoList[0];
                for (int i = 1; i < ammoList.Count; i++)
                {
                    inventory[ammoList[i - 1].Item2] = ammoList[i].Item1;
                }

                inventory[ammoList[ammoList.Count - 1].Item2] = firstAmmo.Item1;
            }

            else
            {
                Tuple<Item, int> lastAmmo = ammoList[ammoList.Count - 1];

                for (int i = 0; i < ammoList.Count - 1; i++)
                {
                    inventory[ammoList[i + 1].Item2] = ammoList[i].Item1;
                }

                inventory[ammoList[0].Item2] = lastAmmo.Item1;

            }


        }
    }
}
