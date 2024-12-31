namespace AF
{
    using System;
    using System.Collections;
    using AF.Inventory;

    public class EV_AutoEquipConsumable : EventBase
    {
        public EquipmentDatabase equipmentDatabase;
        public InventoryDatabase inventoryDatabase;
        public Consumable consumableToEquip;

        public override IEnumerator Dispatch()
        {
            int freeSlot = Array.FindIndex(equipmentDatabase.consumables, (slot) => slot.IsEmpty());

            if (freeSlot != -1)
            {
                ConsumableInstance addedConsumable = inventoryDatabase.AddItem(consumableToEquip) as ConsumableInstance;

                equipmentDatabase.EquipConsumable(addedConsumable, freeSlot);
            }

            yield return null;
        }
    }
}
