namespace AFV2
{
    using UnityEngine;
    using UnityEngine.UI;

    public class InventorySlotButton : SlotButton
    {
        [SerializeField] Image equippedIcon;

        public void ShowEquippedIcon() => equippedIcon.gameObject.SetActive(true);
        public void HideEquippedIcon() => equippedIcon.gameObject.SetActive(false);

        void Awake()
        {
            HideEquippedIcon();
        }
    }
}
