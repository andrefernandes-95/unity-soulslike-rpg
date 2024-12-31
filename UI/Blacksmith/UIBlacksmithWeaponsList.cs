namespace AF
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using AF.Inventory;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIBlacksmithWeaponsList : MonoBehaviour
    {

        [Header("UI")]
        public VisualTreeAsset recipeItem;

        [Header("Components")]
        public Soundbank soundbank;
        public UIDocumentCraftScreen uIDocumentCraftScreen;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        // Last scroll position
        int lastScrollElementIndex = -1;
        [HideInInspector] public WeaponInstance selectedWeaponInstance;

        public void ClearPreviews(VisualElement root)
        {
            selectedWeaponInstance = null;
        }

        public void DrawUI(VisualElement root, Action onClose)
        {
            PopulateScrollView(root, onClose);
        }

        void PopulateScrollView(VisualElement root, Action onClose)
        {
            var scrollView = root.Q<ScrollView>("WeaponsListScrollView");
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

            PopulateWeaponsScrollView(root, onClose);

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
                root.Q<ScrollView>("WeaponsListScrollView"),
                () =>
                {
                    lastScrollElementIndex = -1;
                }
            );
        }

        void PopulateWeaponsScrollView(VisualElement root, Action onClose)
        {
            var scrollView = root.Q<ScrollView>("WeaponsListScrollView");

            int i = 0;
            foreach (WeaponInstance weaponInstance in GetWeaponsList())
            {
                Weapon wp = weaponInstance.GetItem();

                int currentIndex = i;

                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<VisualElement>("ItemIcon").style.backgroundImage = new StyleBackground(wp.sprite);
                scrollItem.Q<Label>("ItemName").text = GetWeaponName(weaponInstance);
                scrollItem.Q<VisualElement>("RemoveIngredient").style.display = DisplayStyle.None;
                scrollItem.Q<VisualElement>("AddIngredient").style.display = DisplayStyle.None;

                var craftBtn = scrollItem.Q<Button>("CraftButtonItem");

                if (selectedWeaponInstance != null && selectedWeaponInstance.GetId() == weaponInstance.GetId())
                {
                    scrollItem.Q<Button>("CraftButtonItem").AddToClassList("blacksmith-craft-button-active");
                }

                scrollItem.Q<Button>("CraftButtonItem").AddToClassList("blacksmith-craft-button");

                UIUtils.SetupButton(craftBtn, () =>
                {
                    lastScrollElementIndex = currentIndex;

                    SelectWeapon(weaponInstance);

                    DrawUI(root, onClose);
                },
                () =>
                {
                    scrollView.ScrollTo(craftBtn);
                },
                () =>
                {
                },
                true,
                soundbank);

                scrollView.Add(craftBtn);

                i++;
            }
        }

        void SelectWeapon(WeaponInstance weaponInstance)
        {
            selectedWeaponInstance = weaponInstance;
            uIDocumentCraftScreen.UpdateUI();
        }

        List<WeaponInstance> GetWeaponsList() => inventoryDatabase.FilterByType<WeaponInstance>();

        string GetWeaponName(WeaponInstance wp) => $"{wp.GetItem().GetName()} +{wp.level}";

    }
}
