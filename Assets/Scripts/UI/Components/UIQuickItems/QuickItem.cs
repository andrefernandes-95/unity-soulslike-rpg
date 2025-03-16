namespace AFV2
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public abstract class QuickItem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] Image equippedItemIcon;
        public CharacterApi characterApi;

        [Header("Item Count")]
        [SerializeField] TextMeshProUGUI itemCount;

        protected void Awake()
        {
            HideItemCount();
        }

        protected void UpdateIcon(Sprite equippedItemSprite)
        {
            equippedItemIcon.sprite = equippedItemSprite;
            equippedItemIcon.gameObject.SetActive(true);
        }

        protected void ShowItemCount(Item item)
        {
            itemCount.text = characterApi.characterInventory.GetItemCount(item).ToString();
            itemCount.gameObject.SetActive(true);
        }

        protected void HideItemCount()
        {
            itemCount.gameObject.SetActive(false);
        }
    }
}
