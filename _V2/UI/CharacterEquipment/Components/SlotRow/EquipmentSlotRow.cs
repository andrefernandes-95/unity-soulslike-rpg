namespace AFV2
{
    using TMPro;
    using UnityEngine;

    public class EquipmentSlotRow : MonoBehaviour
    {
        CharacterEquipmentScreen characterEquipmentScreen => GetComponentInParent<CharacterEquipmentScreen>();
        public CharacterEquipmentScreen CharacterEquipmentScreen => characterEquipmentScreen;

        [Header("Components")]
        [SerializeField] protected TextMeshProUGUI categoryLabel;
        [SerializeField] protected Transform slotsContainer;

        protected void ClearSlots()
        {
            foreach (Transform child in this.slotsContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
