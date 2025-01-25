namespace AF
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
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
            UnselectWeapon();
        }

        public void DrawUI(VisualElement root, Action onClose)
        {
            PopulateScrollView(root, onClose);
        }

        void PopulateScrollView(VisualElement root, Action onClose)
        {
            var scrollView = root.Q<ScrollView>("WeaponsListScrollView");
            scrollView.Clear();


            PopulateWeaponsScrollView(root, onClose);

            if (HasWeaponSelected())
            {
                return;
            }

            if (lastScrollElementIndex != -1)
            {
                StartCoroutine(GiveFocusCoroutine(root));
            }
            else if (scrollView.childCount > 0)
            {
                scrollView.Children().FirstOrDefault().Focus();
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

                if (IsWeaponInstanceSelected(weaponInstance))
                {
                    scrollItem.Q<VisualElement>("Selected").style.display = DisplayStyle.Flex;
                    scrollItem.Q<Button>("CraftButtonItem").AddToClassList("blacksmith-craft-button-active");
                }
                else if (HasWeaponSelected())
                {
                    craftBtn.style.opacity = 0.2f;
                }

                scrollItem.Q<Button>("CraftButtonItem").AddToClassList("blacksmith-craft-button");

                UIUtils.SetupButton(craftBtn, () =>
                {
                    lastScrollElementIndex = currentIndex;

                    SelectWeapon(weaponInstance);
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
            if (IsWeaponInstanceSelected(weaponInstance))
            {
                UnselectWeapon();
                return;
            }

            selectedWeaponInstance = weaponInstance;
            soundbank.PlaySound(soundbank.uiDecision);
            uIDocumentCraftScreen.UpdateUI();
        }

        bool IsWeaponInstanceSelected(WeaponInstance weaponInstance)
        {
            if (weaponInstance == null || selectedWeaponInstance == null)
            {
                return false;
            }

            return weaponInstance.GetId() == selectedWeaponInstance.GetId();
        }

        public void UnselectWeapon()
        {
            selectedWeaponInstance = null;
            uIDocumentCraftScreen.UpdateUI();
            soundbank.PlaySound(soundbank.uiCancel);
        }

        public bool HasWeaponSelected()
        {
            return selectedWeaponInstance != null && selectedWeaponInstance.Exists();
        }

        List<WeaponInstance> GetWeaponsList() => inventoryDatabase.FilterByType<WeaponInstance>();

        string GetWeaponName(WeaponInstance wp) => $"{wp.GetItem().GetName()} +{wp.level}";

    }
}
