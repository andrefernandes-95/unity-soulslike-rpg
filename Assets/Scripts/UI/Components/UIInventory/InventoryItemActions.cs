namespace AFV2
{
    using UnityEngine;

    public class InventoryItemActions : MonoBehaviour
    {
        ItemInstance selectedItemInstance;

        [Header("UI")]
        [SerializeField] GameObject itemActionsContainer;
        [SerializeField] InventoryItemList inventoryItemList;

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;
        [SerializeField] InputListener inputListener;

        void Awake()
        {
            inputListener.onInteract.AddListener(OnDropItem);

            HideActions();
        }

        void OnDisable()
        {
            HideActions();
        }

        public void DisplayActionsForItem(ItemInstance itemInstance)
        {
            selectedItemInstance = itemInstance;
            itemActionsContainer.SetActive(true);
        }

        public void HideActions()
        {
            selectedItemInstance = null;
            itemActionsContainer.SetActive(false);
        }

        void OnDropItem()
        {
            if (selectedItemInstance != null)
            {
                characterApi.characterInventory.DropItem(selectedItemInstance);
                HideActions();
                inventoryItemList.RenderItemsList();
            }
        }
    }
}
