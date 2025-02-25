namespace AFV2
{
    using UnityEngine;

    public class InventoryFilter : MonoBehaviour
    {
        [SerializeField] UICharacterInventory uICharacterInventory;
        InventoryFilterButton[] inventoryFilterButtons => GetComponentsInChildren<InventoryFilterButton>();

        private EquipmentSlotType filter = EquipmentSlotType.ALL;
        public EquipmentSlotType Filter => filter;
        private int slotFilter = -1;
        public int SlotFilter => slotFilter;


        void Awake()
        {
            AssignFilterButtonFilters();
        }

        void OnEnable()
        {
            HandleFilterButtons();
        }


        #region Filtering
        void AssignFilterButtonFilters()
        {
            foreach (InventoryFilterButton inventoryFilterButton in inventoryFilterButtons)
            {
                inventoryFilterButton.Button.onClick.AddListener(() =>
                {
                    this.filter = inventoryFilterButton.filter;

                    HandleFilterButtons();

                    uICharacterInventory.inventoryItemList.RenderItemsList();
                });
            }
        }

        void HandleFilterButtons()
        {
            foreach (InventoryFilterButton inventoryFilterButton in inventoryFilterButtons)
            {
                inventoryFilterButton.SetEnabled(inventoryFilterButton.filter == filter || ShouldEnableFilters());
                inventoryFilterButton.SetIsActive(filter);
            }
        }

        public void SetEquipmentFilter(EquipmentSlotType filterToApply, int slotToEquip)
        {
            this.filter = filterToApply;
            this.slotFilter = slotToEquip;
        }

        public void ResetFilters()
        {
            this.slotFilter = -1;
        }
        #endregion

        #region Booleans
        bool ShouldEnableFilters()
        {
            return uICharacterInventory.Mode != UICharacterInventory.InventoryScreenMode.EQUIP_ITEM;
        }
        #endregion
    }
}
