namespace AFV2
{
    using UnityEngine;

    public class CharacterEquipmentScreen : MonoBehaviour
    {
        [SerializeField] CharacterEquipmentUI characterEquipmentUI;
        [SerializeField] CharacterInventoryUI characterInventoryUI;

        void Awake()
        {
            this.gameObject.SetActive(false);
        }

        void OnDisable()
        {
            characterEquipmentUI.ShowEquipmentSlots();
        }
    }
}
