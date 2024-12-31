namespace AF
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Health;
    using AF.Inventory;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIBlacksmithGemstones : MonoBehaviour
    {

        [Header("UI")]
        public VisualTreeAsset gemstoneItemPrefab;

        [Header("Components")]
        public Soundbank soundbank;
        public UIDocumentCraftScreen uIDocumentCraftScreen;
        public PlayerManager playerManager;
        public UIWeaponStatsContainer uIWeaponStatsContainer;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public GemstonesDatabase gemstonesDatabase;

        // Last scroll position
        int lastScrollElementIndex = -1;
        Gemstone selectedGemstone;

        public void ClearPreviews(VisualElement root)
        {
            selectedGemstone = null;
        }

        public void DrawUI(VisualElement root, Action onClose)
        {
            PreviewCurrentDamage(root);

            PopulateScrollView(root, onClose);
        }

        void PopulateScrollView(VisualElement root, Action onClose)
        {
            var scrollView = root.Q<ScrollView>("GemstonesScrollView");
            scrollView.Clear();

            Button exitButton = new()
            {
                text = Glossary.IsPortuguese() ? "Regressar" : "Return"
            };
            exitButton.AddToClassList("primary-button");
            UIUtils.SetupButton(exitButton, () =>
            {
                onClose();
            }, soundbank);

            scrollView.Add(exitButton);

            PopulateGemstonesScrollView(root, onClose);

            if (lastScrollElementIndex == -1)
            {
                scrollView.ScrollTo(exitButton);
                exitButton.Focus();
            }
            else
            {
                StartCoroutine(GiveFocusCoroutine(root));
            }
        }

        IEnumerator GiveFocusCoroutine(VisualElement root)
        {
            yield return new WaitForSeconds(0);
            GiveFocus(root);
        }

        void GiveFocus(VisualElement root)
        {
            UIUtils.ScrollToLastPosition(
                lastScrollElementIndex,
                root.Q<ScrollView>("GemstonesScrollView"),
                () =>
                {
                    lastScrollElementIndex = -1;
                }
            );
        }

        string GetEquippedText()
        {
            return Glossary.IsPortuguese() ? "Equipado" : "Equipped";
        }

        string GetEquippedToWeaponText(Weapon weapon)
        {
            if (weapon == null)
            {
                return "";
            }

            return Glossary.IsPortuguese() ? $"Equipado em {weapon.GetName()}" : $"Equipped in {weapon.GetName()}";
        }

        void PopulateGemstonesScrollView(VisualElement root, Action onClose)
        {
            var scrollView = root.Q<ScrollView>("GemstonesScrollView");

            int i = 0;
            foreach (var gemstoneInstance in GetGemstonesList())
            {
                int currentIndex = i;

                Gemstone gemstone = gemstoneInstance.GetItem();

                var scrollItem = this.gemstoneItemPrefab.CloneTree();

                scrollItem.Q<VisualElement>("ItemIcon").style.backgroundImage = new StyleBackground(gemstone.sprite);
                scrollItem.Q<Label>("Title").text = gemstone.GetName();

                WeaponInstance selectedWeaponInstance = uIDocumentCraftScreen.uIBlacksmithWeaponsList?.selectedWeaponInstance;

                string weaponIdThatThisGemstoneIsAttachedTo = gemstonesDatabase.GetWeaponIdByAttachedGemstone(gemstone);

                bool isEquipped = weaponIdThatThisGemstoneIsAttachedTo == selectedWeaponInstance.GetId();

                WeaponInstance weaponThatThisGemstoneIsAttachedTo = inventoryDatabase.FindItemById(weaponIdThatThisGemstoneIsAttachedTo) as WeaponInstance;

                scrollItem.Q<Label>("EquippedIndicator").text = isEquipped
                    ? GetEquippedText()
                    : GetEquippedToWeaponText(weaponThatThisGemstoneIsAttachedTo?.GetItem());

                var equipGemstoneButton = scrollItem.Q<Button>();

                if (isEquipped)
                {
                    equipGemstoneButton.AddToClassList("blacksmith-craft-button-active");
                }

                UIUtils.SetupButton(equipGemstoneButton, () =>
                {
                    lastScrollElementIndex = currentIndex;
                    PreviewGemstone(gemstone, root, isEquipped);

                    SelectGemstone(gemstone);

                    DrawUI(root, onClose);
                },
                () =>
                {
                    scrollView.ScrollTo(equipGemstoneButton);
                    PreviewGemstone(gemstone, root, isEquipped);
                },
                () =>
                {
                    root.Q<VisualElement>("WeaponStatsContainer").style.opacity = 0;
                },
                true,
                soundbank);

                scrollView.Add(equipGemstoneButton);

                i++;
            }
        }

        void SelectGemstone(Gemstone gemstone)
        {
            selectedGemstone = gemstone;

            WeaponInstance weaponInstanceToAttach = uIDocumentCraftScreen.uIBlacksmithWeaponsList.selectedWeaponInstance;

            if (weaponInstanceToAttach != null)
            {
                if (gemstonesDatabase.GetWeaponIdByAttachedGemstone(gemstone) == weaponInstanceToAttach.GetId())
                {
                    gemstonesDatabase.DettachGemstoneFromWeapon(weaponInstanceToAttach, selectedGemstone);
                }
                else
                {
                    gemstonesDatabase.AttachGemstoneToWeapon(weaponInstanceToAttach, selectedGemstone);
                }
            }

            uIDocumentCraftScreen.UpdateUI();
        }

        List<GemstoneInstance> GetGemstonesList() => inventoryDatabase.FilterByType<GemstoneInstance>();

        void ClearPreview(VisualElement root)
        {
            root.Q<VisualElement>("WeaponStatsContainer").Clear();
            root.Q<VisualElement>("WeaponStatsContainer").style.opacity = 1;
        }

        void PreviewCurrentDamage(VisualElement root)
        {
            ClearPreview(root);
            WeaponInstance selectedWeaponInstance = uIDocumentCraftScreen.uIBlacksmithWeaponsList.selectedWeaponInstance;

            Weapon weapon = selectedWeaponInstance.GetItem();

            Damage currentWeaponDamage = weapon.weaponDamage.GetCurrentDamage(playerManager,
                playerManager.statsBonusController.GetCurrentStrength(),
                playerManager.statsBonusController.GetCurrentDexterity(),
                playerManager.statsBonusController.GetCurrentIntelligence(),
                selectedWeaponInstance,
                playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(selectedWeaponInstance));

            Gemstone[] equippedGemstones = gemstonesDatabase.GetAttachedGemstonesFromWeapon(selectedWeaponInstance);

            string gemstoneNames = string.Join(", ", equippedGemstones.Select(gemstone => gemstone.GetName()));

            uIWeaponStatsContainer.PreviewWeaponDamageDifference(
                weapon.GetName() + " +" + selectedWeaponInstance.level + ", " + gemstoneNames,
                currentWeaponDamage,
                currentWeaponDamage,
                root);
        }

        void PreviewGemstone(Gemstone gemstone, VisualElement root, bool isGemstoneEquipped)
        {
            PreviewCurrentDamage(root);

            WeaponInstance selectedWeaponInstance = uIDocumentCraftScreen.uIBlacksmithWeaponsList.selectedWeaponInstance;

            if (selectedWeaponInstance == null)
            {
                return;
            }

            Weapon weapon = selectedWeaponInstance.GetItem();

            Damage currentWeaponDamage = weapon.weaponDamage.GetCurrentDamage(playerManager,
                playerManager.statsBonusController.GetCurrentStrength(),
                playerManager.statsBonusController.GetCurrentDexterity(),
                playerManager.statsBonusController.GetCurrentIntelligence(),
                selectedWeaponInstance,
                playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(selectedWeaponInstance));

            if (gemstone != null)
            {
                root.Q<VisualElement>("WeaponStatsContainer").Add(uIWeaponStatsContainer.CreateLabel(" > ", 0));

                Damage gemstoneDamage = !isGemstoneEquipped
                    ? weapon.weaponDamage.EnhanceWithGemstonesDamage(currentWeaponDamage, new List<Gemstone>() { gemstone }.ToArray())
                    : weapon.weaponDamage.GetCurrentDamage(playerManager,
                        playerManager.statsBonusController.GetCurrentStrength(),
                        playerManager.statsBonusController.GetCurrentDexterity(),
                        playerManager.statsBonusController.GetCurrentIntelligence(),
                        selectedWeaponInstance,
                        playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(selectedWeaponInstance).Where(gem => gem.Id != gemstone.Id).ToArray());

                uIWeaponStatsContainer.PreviewWeaponDamageDifference(
                    weapon.GetName() + " + " + gemstone.GetName(),
                    currentWeaponDamage,
                    gemstoneDamage,
                    root);
            }
        }
    }
}
