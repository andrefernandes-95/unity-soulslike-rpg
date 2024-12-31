namespace AF
{
    using System.Collections.Generic;
    using AF.Inventory;
    using UnityEngine.Events;
    using UnityEngine.Localization.Settings;

    public static class CraftingUtils
    {
        public static bool IsFullyUpgraded(WeaponInstance weaponInstance) =>
            weaponInstance.level >= weaponInstance.GetItem().weaponDamage.weaponUpgradeLevels.Length - 1;

        public static bool ShouldSkipUpgrade(WeaponInstance weaponInstance)
        {
            Weapon wp = weaponInstance.GetItem();

            if (wp == null)
            {
                return true;
            }

            if (!wp.canBeUpgraded)
            {
                return true;
            }

            if (IsFullyUpgraded(weaponInstance))
            {
                return true;
            }

            return false;
        }

        public static bool CanCraftItem(InventoryDatabase inventoryDatabase, CraftingRecipe recipe)
        {
            bool hasEnoughMaterial = true;

            foreach (var recipeIngredient in recipe.ingredients)
            {
                hasEnoughMaterial = inventoryDatabase.HasItemAmount(recipeIngredient.ingredient, recipeIngredient.amount);
                if (!hasEnoughMaterial) break;
            }

            return hasEnoughMaterial;
        }

        public static bool CanImproveWeapon(InventoryDatabase inventoryDatabase, WeaponInstance weaponInstance, int ownedGold)
        {
            Weapon weapon = weaponInstance.GetItem();

            if (ShouldSkipUpgrade(weaponInstance))
            {
                return false;
            }

            WeaponUpgradeLevel nextWeaponUpgradeLevel = weapon.weaponDamage.GetWeaponUpgradeLevel(weaponInstance.level);

            if (nextWeaponUpgradeLevel == null)
            {
                return false;
            }

            bool hasAllMaterialsRequired = true;

            foreach (var upgradeMaterial in nextWeaponUpgradeLevel.upgradeMaterials)
            {
                if (!inventoryDatabase.HasItem(upgradeMaterial.Key) || inventoryDatabase.GetItemAmount(upgradeMaterial.Key) < upgradeMaterial.Value)
                {
                    hasAllMaterialsRequired = false;
                    break;
                }
            }

            if (!hasAllMaterialsRequired)
            {
                return false;
            }

            if (ownedGold < nextWeaponUpgradeLevel.goldCostForUpgrade)
            {
                return false;
            }

            return true;
        }

        public static void UpgradeWeapon(
            WeaponInstance weaponInstance,
            UnityAction<int> onUpgrade,
            UnityAction<KeyValuePair<UpgradeMaterial, int>> onUpgradeMaterialUsed,
            InventoryDatabase inventoryDatabase
        )
        {
            var currentWeaponLevel = weaponInstance.level;

            WeaponUpgradeLevel weaponUpgradeForNextLevel = weaponInstance.GetItem().weaponDamage.GetWeaponUpgradeLevel(currentWeaponLevel);

            onUpgrade(weaponUpgradeForNextLevel.goldCostForUpgrade);

            foreach (var upgradeMaterial in weaponUpgradeForNextLevel.upgradeMaterials)
            {
                onUpgradeMaterialUsed(upgradeMaterial);
            }

            inventoryDatabase.UpgradeWeapon(weaponInstance.GetId());
        }


        public static bool CanBeUpgradedFurther(WeaponInstance weaponInstance)
        {
            Weapon weapon = weaponInstance.GetItem();

            if (weapon == null)
            {
                return false;
            }

            if (!weapon.canBeUpgraded)
            {
                return false;
            }

            return weaponInstance.level + 1 < weapon.weaponDamage.weaponUpgradeLevels.Length;
        }


        public static string GetMaterialCostForNextLevel(WeaponInstance weaponInstance)
        {
            if (CraftingUtils.CanBeUpgradedFurther(weaponInstance))
            {
                Weapon weapon = weaponInstance.GetItem();
                WeaponDamage weaponDamage = weapon.weaponDamage;

                WeaponUpgradeLevel nextWeaponUpgradeLevel = weaponDamage.weaponUpgradeLevels[weaponInstance.level + 1];

                string text = $"{LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Next Weapon Level: ")}{weaponInstance.level + 1}\n";

                text += $"{LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Required Gold:")} {nextWeaponUpgradeLevel.goldCostForUpgrade} {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Coins")}\n";
                text += $"{LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Required Items:")} \n";

                foreach (var upgradeMat in weaponDamage.weaponUpgradeLevels[weaponInstance.level + 1].upgradeMaterials)
                {
                    if (upgradeMat.Key != null)
                    {
                        text += $"- {upgradeMat.Key.GetName()}: x{upgradeMat.Value}\n";
                    }
                }

                return text;
            }

            return "";
        }

    }
}
