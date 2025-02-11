namespace AFV2
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public abstract class SlotButton : MonoBehaviour
    {
        protected EquipmentSlotRow equipmentSlotRow => GetComponentInParent<EquipmentSlotRow>();
        protected Button button => GetComponent<Button>();
        protected TextMeshProUGUI buttonLabel => GetComponentInChildren<TextMeshProUGUI>();

        void Start()
        {
            AssignCallbacks();
        }

        void OnEnable()
        {
            PopulateButton();
        }

        abstract protected void AssignCallbacks();

        abstract protected void OnClickSlot();

        abstract protected void PopulateButton();

    }
}
