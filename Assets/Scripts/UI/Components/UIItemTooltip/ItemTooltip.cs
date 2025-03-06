namespace AFV2
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(CanvasGroup))]
    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] WeaponTooltip weaponTooltip;

        [Header("UI Components")]
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemID;
        public TextMeshProUGUI itemDescription;
        public Image itemIcon;

        [Header("Canvas Group")]
        CanvasGroup canvasGroup => GetComponent<CanvasGroup>();

        void Awake()
        {
            Hide();
        }

        public void Show(ItemInstance itemInstance)
        {
            if (itemInstance == null || itemInstance.item == null)
            {
                return;
            }

            itemName.text = itemInstance.item.DisplayName;
            itemID.text = itemInstance.ID;
            itemDescription.text = itemInstance.item.Description;
            itemIcon.sprite = itemInstance.item.Sprite;

            if (itemInstance is WeaponInstance weaponInstance)
            {
                weaponTooltip.Show(weaponInstance);
            }

            canvasGroup.alpha = 1f;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0f;
        }
    }
}
