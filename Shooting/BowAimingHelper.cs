namespace AF
{
    using AF.Events;
    using TigerForge;
    using UnityEngine;

    public class BowAimingHelper : MonoBehaviour
    {
        public Vector3 crossBowPosition;
        public EquipmentDatabase equipmentDatabase;

        Vector3 originalPosition;

        private void Awake()
        {
            originalPosition = transform.localPosition;

            EventManager.StartListening(EventMessages.ON_EQUIPMENT_CHANGED, Evaluate);

            Evaluate();
        }

        void Evaluate()
        {
            if (equipmentDatabase.GetCurrentWeapon().Exists() && equipmentDatabase.GetCurrentWeapon().GetItem().isCrossbow)
            {
                transform.localPosition = crossBowPosition;
            }
            else
            {
                transform.localPosition = originalPosition;
            }
        }

    }
}
