namespace AFV2
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UICharacterInventory : MonoBehaviour
    {
        [SerializeField] CharacterEquipment characterEquipment;

        [Header("UI Screens")]
        [SerializeField] UICharacterEquipment uICharacterEquipment;

        [Header("UI Components")]
        public InventoryItemList inventoryItemList;
        public InventoryFilter inventoryFilter;

        public enum InventoryScreenMode
        {
            DEFAULT,
            EQUIP_ITEM
        }

        InventoryScreenMode _mode = InventoryScreenMode.DEFAULT;
        public InventoryScreenMode Mode => _mode;

        void OnEnable()
        {
            inventoryItemList.RenderItemsList();
        }

        void OnDisable()
        {
            EventSystem.current.SetSelectedGameObject(null);

            ResetInventoryScreenMode();
        }

        void ResetInventoryScreenMode()
        {
            this._mode = InventoryScreenMode.DEFAULT;
            inventoryFilter.ResetFilters();
        }

        public void SetEquipMode(EquipmentSlotType filterToApply, int slotToEquip)
        {
            this._mode = InventoryScreenMode.EQUIP_ITEM;
            inventoryFilter.SetEquipmentFilter(filterToApply, slotToEquip);
        }

        public void OnItemSelect(ItemInstance itemInstance)
        {
            if (Mode == InventoryScreenMode.EQUIP_ITEM)
            {
                uICharacterEquipment.OnItemEquipped(itemInstance, inventoryFilter.Filter, inventoryFilter.SlotFilter);
            }
        }
    }
}
