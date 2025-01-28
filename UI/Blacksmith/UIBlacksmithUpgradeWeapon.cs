namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using AF.Health;
    using AF.Inventory;
    using GameAnalyticsSDK;
    using UnityEngine;
    using UnityEngine.Localization.Settings;
    using UnityEngine.UIElements;

    public class UIBlacksmithUpgradeWeapon : MonoBehaviour
    {
        [Header("UI")]
        public VisualTreeAsset ingredientItem;
        public Sprite goldSprite;

        [Header("SFX")]
        public UIDocumentPlayerGold uIDocumentPlayerGold;

        [Header("Components")]
        public NotificationManager notificationManager;
        public PlayerManager playerManager;
        public Soundbank soundbank;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("Components")]
        public UIDocumentCraftScreen uIDocumentCraftScreen;
        public UIWeaponStatsContainer uIWeaponStatsContainer;

        void ClearPreviews(VisualElement root)
        {
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;
        }

        public void DrawUI(WeaponInstance selectedWeaponInstance, VisualElement root)
        {
            ClearPreviews(root);

            PreviewWeaponUpgrade(selectedWeaponInstance, root);

            UIUtils.PlayFadeInAnimation(root.Q<VisualElement>("WeaponUpgrade"), 0.2f);
        }

        void PreviewWeaponUpgrade(WeaponInstance weaponInstance, VisualElement root)
        {
            root.Q<VisualElement>("WeaponStatsContainer").Clear();
            root.Q<VisualElement>("WeaponStatsContainer").style.opacity = 1;

            Weapon weapon = weaponInstance.GetItem();

            WeaponUpgradeLevel weaponUpgradeLevel = weapon.weaponDamage.GetWeaponUpgradeLevel(weaponInstance.level);

            if (weaponUpgradeLevel == null)
            {
                return;
            }

            Damage currentWeaponDamage = weapon.weaponDamage.GetCurrentDamage(playerManager,
                playerManager.statsBonusController.GetCurrentStrength(),
                playerManager.statsBonusController.GetCurrentDexterity(),
                playerManager.statsBonusController.GetCurrentIntelligence(),
                weaponInstance);

            uIWeaponStatsContainer.PreviewWeaponDamageDifference(
                weapon.GetName() + " +" + weaponInstance.level,
                currentWeaponDamage,
                currentWeaponDamage,
                root);

            if (CraftingUtils.CanBeUpgradedFurther(weaponInstance))
            {
                root.Q<VisualElement>("WeaponStatsContainer").Add(uIWeaponStatsContainer.CreateLabel(" > ", 0));

                // Serialize the original weapon into JSON
                string json = JsonUtility.ToJson(weapon);

                WeaponInstance nextWeaponInstance = new(
                    "fakeId",
                    weaponInstance.GetItem(),
                    weaponInstance.level + 1,
                    new());

                Damage desiredWeaponDamage = weapon.weaponDamage.GetCurrentDamage(
                    playerManager,
                    playerManager.statsBonusController.GetCurrentStrength(),
                    playerManager.statsBonusController.GetCurrentDexterity(),
                    playerManager.statsBonusController.GetCurrentIntelligence(),
                    nextWeaponInstance);

                uIWeaponStatsContainer.PreviewWeaponDamageDifference(
                    weapon.GetName() + " +" + nextWeaponInstance.level,
                    currentWeaponDamage,
                    desiredWeaponDamage,
                    root);
            }

            // Requirements
            root.Q<VisualElement>("ItemInfo").Clear();

            foreach (var upgradeMaterial in weaponUpgradeLevel.upgradeMaterials)
            {
                UpgradeMaterial upgradeMaterialItem = upgradeMaterial.Key;
                int amountRequiredFoUpgrade = upgradeMaterial.Value;

                var ingredientItemEntry = ingredientItem.CloneTree();
                ingredientItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(upgradeMaterialItem.sprite);
                ingredientItemEntry.Q<Label>("Title").text = upgradeMaterialItem.GetName();

                var playerOwnedIngredientAmount = inventoryDatabase.GetItemAmount(upgradeMaterialItem);

                ingredientItemEntry.Q<Label>("Amount").text = playerOwnedIngredientAmount + " / " + amountRequiredFoUpgrade;
                ingredientItemEntry.Q<Label>("Amount").style.opacity =
                    playerOwnedIngredientAmount >= amountRequiredFoUpgrade ? 1 : 0.25f;

                root.Q<VisualElement>("ItemInfo").Add(ingredientItemEntry);
                root.Q<VisualElement>("ItemInfo").style.opacity = 1;
            }

            // Add Gold

            var goldItemEntry = ingredientItem.CloneTree();
            goldItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(goldSprite);
            goldItemEntry.Q<Label>("Title").text = LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Gold");

            goldItemEntry.Q<Label>("Amount").text = playerStatsDatabase.gold + " / " + weaponUpgradeLevel.goldCostForUpgrade;
            goldItemEntry.Q<Label>("Amount").style.opacity = playerStatsDatabase.gold >= weaponUpgradeLevel.goldCostForUpgrade ? 1 : 0.25f;

            root.Q<VisualElement>("ItemInfo").Add(goldItemEntry);

            bool shouldSkipUpgrade = CraftingUtils.ShouldSkipUpgrade(weaponInstance);
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = shouldSkipUpgrade ? 0 : 1;

            UIUtils.SetupButton(root.Q<Button>("UpgradeButton"), () =>
            {
                HandleWeaponUpgrade(weaponInstance, root);
            }, soundbank);

            if (!shouldSkipUpgrade)
            {
                StartCoroutine(GiveFocus_Coroutine(root.Q<Button>("UpgradeButton")));
            }

            root.Q<Button>("UpgradeButton").SetEnabled(CanImproveWeapon(weaponInstance));

            root.Q<Button>("UpgradeButton").style.display = shouldSkipUpgrade ? DisplayStyle.None : DisplayStyle.Flex;

            root.Q<Label>("WeaponFullyUpgradedLabel").style.display = CraftingUtils.IsFullyUpgraded(weaponInstance) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        IEnumerator GiveFocus_Coroutine(Button button)
        {
            yield return new WaitForEndOfFrame();
            button.Focus();
        }

        bool CanImproveWeapon(WeaponInstance weaponInstance) => CraftingUtils.CanImproveWeapon(inventoryDatabase, weaponInstance, playerStatsDatabase.gold);

        void HandleWeaponUpgrade(WeaponInstance weaponInstance, VisualElement root)
        {
            if (!CanImproveWeapon(weaponInstance))
            {
                return;
            }

            playerManager.playerAchievementsManager.achievementForUpgradingFirstWeapon.AwardAchievement();
            soundbank.PlaySound(soundbank.craftSuccess);
            notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Weapon improved!"), weaponInstance.GetItem().sprite);

            LogAnalytic(AnalyticsUtils.OnUIButtonClick("UpgradeWeapon"), new() {
                { "weapon_upgraded", weaponInstance.GetItem().name }
            });

            CraftingUtils.UpgradeWeapon(
                weaponInstance,
                (goldUsed) => uIDocumentPlayerGold.LoseGold(goldUsed),
                (upgradeMaterialUsed) =>
                {
                    for (int i = 0; i < upgradeMaterialUsed.Value; i++)
                    {
                        playerManager.playerInventory.RemoveItem(upgradeMaterialUsed.Key);
                    }
                },
                inventoryDatabase
            );

            uIDocumentCraftScreen.UpdateUI();
        }

        void LogAnalytic(string eventName, Dictionary<string, object> values)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName, values);
        }
    }
}
