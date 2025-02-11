namespace AF
{
    using AF.Inventory;
    using AF.Music;
    using AF.UI;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentCraftScreen : MonoBehaviour
    {
        [Header("SFX")]
        public AudioClip sfxOnEnterMenu;

        [Header("UI Components")]
        public UIDocument uIDocument;
        [HideInInspector] public VisualElement root;
        public UIDocumentBonfireMenu uIDocumentBonfireMenu;

        [Header("Components")]
        public PlayerManager playerManager;
        public CursorManager cursorManager;
        public BGMManager bgmManager;
        public Soundbank soundbank;
        public StarterAssetsInputs starterAssetsInputs;

        [HideInInspector] public bool returnToBonfire = false;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public BlacksmithAction blacksmithAction = BlacksmithAction.UPGRADE;

        [Header("Blacksmith Components")]
        public UIBlacksmithWeaponsList uIBlacksmithWeaponsList;
        public UIBlacksmithUpgradeWeapon uIBlacksmithUpgradeWeapon;
        public UIBlacksmithGemstones uIBlacksmithGemstones;

        [Header("Footer Actions")]
        public MenuFooter menuFooter;
        public ActionButton selectWeaponButton, confirmButton, exitMenuButton, returnToWeaponSelection, previousMenu, nextMenu;

        private VisualElement weaponSelectionInfo;
        private VisualElement weaponSelectionRoot, selectedWeaponMenuRoot;
        private VisualElement previousMenuPlaceholder, nextMenuPlaceholder;

        private void Awake()
        {
            this.gameObject.SetActive(false);

            starterAssetsInputs.onMenuEvent.AddListener(OnClose);

            starterAssetsInputs.onPreviousMenu.AddListener(OnPreviousMenu);
            starterAssetsInputs.onNextMenu.AddListener(OnNextMenu);
        }

        bool CanNavigateMenus()
        {
            if (!this.isActiveAndEnabled)
            {
                return false;
            }

            if (!uIBlacksmithWeaponsList.HasWeaponSelected())
            {
                return false;
            }

            return true;
        }

        void OnPreviousMenu()
        {
            if (!CanNavigateMenus()) return;

            if (blacksmithAction == BlacksmithAction.UPGRADE)
            {
                SetBlacksmithAction(BlacksmithAction.SHARPEN);
            }
            else if (blacksmithAction == BlacksmithAction.CUSTOMIZE_GEMSTONE)
            {
                SetBlacksmithAction(BlacksmithAction.UPGRADE);
            }
            else
            {
                SetBlacksmithAction(BlacksmithAction.CUSTOMIZE_GEMSTONE);
            }
        }

        void OnNextMenu()
        {
            if (!CanNavigateMenus()) return;

            if (blacksmithAction == BlacksmithAction.UPGRADE)
            {
                SetBlacksmithAction(BlacksmithAction.CUSTOMIZE_GEMSTONE);
            }
            else if (blacksmithAction == BlacksmithAction.CUSTOMIZE_GEMSTONE)
            {
                SetBlacksmithAction(BlacksmithAction.SHARPEN);
            }
            else
            {
                SetBlacksmithAction(BlacksmithAction.UPGRADE);
            }
        }

        private void OnEnable()
        {
            this.root = uIDocument.rootVisualElement;

            DisablePlayerControl();

            weaponSelectionRoot = root.Q<VisualElement>("WeaponSelection");
            weaponSelectionInfo = weaponSelectionRoot.Q<VisualElement>("WeaponSelectionInfo");
            selectedWeaponMenuRoot = root.Q<VisualElement>("SelectedWeaponMenu");

            previousMenuPlaceholder = root.Q<VisualElement>("PreviousMenuPlaceholder");
            nextMenuPlaceholder = root.Q<VisualElement>("NextMenuPlaceholder");

            bgmManager.PlaySound(sfxOnEnterMenu, null);
            cursorManager.ShowCursor();

            SetupTabButtons();

            UpdateUI();
        }

        void DisablePlayerControl()
        {
            playerManager.playerComponentManager.DisableComponents();
            playerManager.playerComponentManager.DisableCharacterController();
        }

        void SetupFooterActionsForWeaponSelection()
        {
            menuFooter.SetupReferences();

            menuFooter.GetFooterActionsContainer().Add(selectWeaponButton.GetKey(starterAssetsInputs));
            menuFooter.GetFooterActionsContainer().Add(exitMenuButton.GetKey(starterAssetsInputs));
        }

        void SetupFooterActionsForSelectedWeaponMenu()
        {
            menuFooter.SetupReferences();

            menuFooter.GetFooterActionsContainer().Add(confirmButton.GetKey(starterAssetsInputs));
            menuFooter.GetFooterActionsContainer().Add(returnToWeaponSelection.GetKey(starterAssetsInputs));
        }

        void FocusOnWeaponSelection()
        {
            SetupFooterActionsForWeaponSelection();

            selectedWeaponMenuRoot.style.opacity = 0;
            weaponSelectionRoot.style.opacity = 1;
            weaponSelectionInfo.style.opacity = 1;
            weaponSelectionRoot.focusable = false;
        }

        void FocusOnSelectedWeaponMenu()
        {
            SetupFooterActionsForSelectedWeaponMenu();

            selectedWeaponMenuRoot.style.opacity = 1;
            weaponSelectionInfo.style.opacity = 0.2f;
            weaponSelectionRoot.focusable = true;
        }

        public void UpdateUI()
        {
            if (!uIBlacksmithWeaponsList.HasWeaponSelected())
            {
                FocusOnWeaponSelection();
            }
            else
            {
                FocusOnSelectedWeaponMenu();
            }

            SetupTabs();

            RedrawUI();
        }

        void SetBlacksmithAction(BlacksmithAction nextBlackmsithAction)
        {
            this.blacksmithAction = nextBlackmsithAction;
            soundbank.PlaySound(soundbank.uiDecision);
            UpdateUI();
        }

        void SetupTabButtons()
        {
            previousMenuPlaceholder.Clear();
            previousMenuPlaceholder.Add(previousMenu.GetKey(starterAssetsInputs));

            nextMenuPlaceholder.Clear();
            nextMenuPlaceholder.Add(nextMenu.GetKey(starterAssetsInputs));

            UIUtils.SetupButton(root.Q<Button>("UpgradesButton"), () =>
            {
                SetBlacksmithAction(BlacksmithAction.UPGRADE);
            }, soundbank);
            UIUtils.SetupButton(root.Q<Button>("GemstonesButton"), () =>
            {
                SetBlacksmithAction(BlacksmithAction.CUSTOMIZE_GEMSTONE);
            }, soundbank);
            UIUtils.SetupButton(root.Q<Button>("SharpeningButton"), () =>
            {
                SetBlacksmithAction(BlacksmithAction.SHARPEN);
            }, soundbank);
        }

        void SetupTabs()
        {
            Label tabDescriptionInfo = root.Q<Label>("TabInfoDescription");
            tabDescriptionInfo.text = "";
            DisableTabs();

            string activeButtonId = "";

            if (blacksmithAction == BlacksmithAction.CUSTOMIZE_GEMSTONE)
            {
                activeButtonId = "GemstonesButton";
                tabDescriptionInfo.text = Glossary.IsPortuguese()
                    ? "Personaliza os atributos da tua arma e o tipo de dano usando gemas especiais."
                    : "Customize your weapon's attributes and damage type by slotting special gemstones.";
            }
            else if (blacksmithAction == BlacksmithAction.SHARPEN)
            {
                activeButtonId = "SharpeningButton";
                tabDescriptionInfo.text = Glossary.IsPortuguese()
                    ? "Afia a tua arma com óleo, aumentando temporariamente o seu dano por 1000 ataques garantidos."
                    : "Sharpen your weapon with oil to temporarily boost its damage for 1000 guaranteed hits.";
            }
            else
            {
                activeButtonId = "UpgradesButton";
                tabDescriptionInfo.text = Glossary.IsPortuguese()
                    ? "Aumenta permanentemente o dano da tua arma ao usar materiais de melhoria."
                    : "Permanently increase your weapon's damage by using upgrade materials.";
            }

            root.Q<Button>(activeButtonId).AddToClassList("blacksmith-tab-button-active");
        }

        void RedrawUI()
        {
            uIBlacksmithWeaponsList.DrawUI(root, Close);

            bool hasWeaponSelected = uIBlacksmithWeaponsList.selectedWeaponInstance != null;

            if (!hasWeaponSelected)
            {
                root.Q<VisualElement>("WeaponUpgrade").style.display = DisplayStyle.None;
                root.Q<VisualElement>("WeaponGemstones").style.display = DisplayStyle.None;
                return;
            }

            root.Q<VisualElement>("WeaponUpgrade").style.display = blacksmithAction == BlacksmithAction.UPGRADE ? DisplayStyle.Flex : DisplayStyle.None;
            root.Q<VisualElement>("WeaponGemstones").style.display = blacksmithAction == BlacksmithAction.CUSTOMIZE_GEMSTONE ? DisplayStyle.Flex : DisplayStyle.None;

            if (blacksmithAction == BlacksmithAction.UPGRADE)
            {
                uIBlacksmithUpgradeWeapon.DrawUI(uIBlacksmithWeaponsList.selectedWeaponInstance, root);
            }
            else if (blacksmithAction == BlacksmithAction.CUSTOMIZE_GEMSTONE)
            {
                uIBlacksmithGemstones.DrawUI(root, () => { });
            }
        }

        void DisableTabs()
        {
            root.Q<Button>("UpgradesButton").RemoveFromClassList("blacksmith-tab-button-active");
            root.Q<Button>("GemstonesButton").RemoveFromClassList("blacksmith-tab-button-active");
            root.Q<Button>("SharpeningButton").RemoveFromClassList("blacksmith-tab-button-active");
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OpenBlacksmithMenu()
        {
            LogAnalytic(AnalyticsUtils.OnUIButtonClick("Blacksmith"));

            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (uIBlacksmithWeaponsList.HasWeaponSelected())
            {
                uIBlacksmithWeaponsList.UnselectWeapon();
                return;
            }

            if (!this.isActiveAndEnabled)
            {
                return;
            }

            Close();
        }

        public void Close()
        {
            if (returnToBonfire)
            {
                returnToBonfire = false;

                uIDocumentBonfireMenu.gameObject.SetActive(true);
                cursorManager.ShowCursor();
                this.gameObject.SetActive(false);
                return;
            }

            playerManager.playerComponentManager.EnableComponents();
            playerManager.playerComponentManager.EnableCharacterController();

            this.gameObject.SetActive(false);
            cursorManager.HideCursor();
        }

        void LogAnalytic(string eventName)
        {
        }


        private void OnDisable()
        {
            uIBlacksmithWeaponsList.ClearPreviews(root);
        }
    }
}
