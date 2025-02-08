namespace AFV2
{
    using AF;
    using UnityEngine;

    public static class InventoryUtils
    {
        public static void AddSerializedItemToCharacterInventory(
            CharacterInventory characterInventory,
            SerializedItem serializedItem,
            InventoryBank inventoryBank
        )
        {
            Item item = inventoryBank.FindByGameObjectName(serializedItem.id);

            if (item != null)
            {
                characterInventory.AddItem(item, 1);
            }

            // Item item = Resources.Load<Item>(serializedItem.itemPath);



            /* if (item is Weapon weapon)
             {
                 WeaponInstance weaponInstance = new(
                     serializedItem.id,
                     weapon,
                     serializedItem.level,
                     serializedItem.attachedGemstoneIds.ToList());

                 ownedItems.Add(weaponInstance);
             }
             else if (item is Shield shield)
             {
                 ShieldInstance shieldInstance = new(
                     serializedItem.id,
                     shield);

                 ownedItems.Add(shieldInstance);
             }
             else if (item is Arrow arrow)
             {
                 ArrowInstance arrowInstance = new(
                     serializedItem.id,
                     arrow);

                 ownedItems.Add(arrowInstance);
             }
             else if (item is Spell spell)
             {
                 SpellInstance spellInstance = new(
                     serializedItem.id,
                     spell);

                 ownedItems.Add(spellInstance);
             }
             else if (item is Consumable consumable)
             {
                 ConsumableInstance consumableInstance = new(serializedItem.id, consumable)
                 {
                     wasUsed = serializedItem.wasUsed
                 };
                 ownedItems.Add(consumableInstance);
             }
             else if (item is Armor armor)
             {
                 ArmorInstance armorInstance = new(
                     serializedItem.id,
                     armor);

                 ownedItems.Add(armorInstance);
             }
             else if (item is Helmet helmet)
             {
                 HelmetInstance helmetInstance = new(
                     serializedItem.id,
                     helmet);

                 ownedItems.Add(helmetInstance);
             }
             else if (item is Gauntlet gauntlet)
             {
                 GauntletInstance gauntletInstance = new(
                     serializedItem.id,
                     gauntlet);

                 ownedItems.Add(gauntletInstance);
             }
             else if (item is Legwear legwear)
             {
                 LegwearInstance legwearInstance = new(
                     serializedItem.id,
                     legwear);

                 ownedItems.Add(legwearInstance);
             }
             else if (item is Accessory accessory)
             {
                 AccessoryInstance accessoryInstance = new(
                     serializedItem.id,
                     accessory);

                 ownedItems.Add(accessoryInstance);
             }
             else if (item is CraftingMaterial craftingMaterial)
             {
                 CraftingMaterialInstance craftingMaterialInstance = new(
                     serializedItem.id,
                     craftingMaterial);

                 ownedItems.Add(craftingMaterialInstance);
             }
             else if (item is UpgradeMaterial upgradeMaterial)
             {
                 UpgradeMaterialInstance upgradeMaterialInstance = new(
                     serializedItem.id,
                     upgradeMaterial);

                 ownedItems.Add(upgradeMaterialInstance);
             }
             else if (item is Gemstone gemstone)
             {
                 GemstoneInstance gemstoneInstance = new(serializedItem.id, gemstone);
                 ownedItems.Add(gemstoneInstance);
             }
             else
             {
                 ItemInstance itemInstance = new(serializedItem.id, item);
                 ownedItems.Add(itemInstance);
             }*/
        }

    }
}