namespace AF
{
    using AF.Events;
    using TigerForge;
    using UnityEngine;

    public class ShieldWorldInstance : MonoBehaviour
    {
        [Header("Shield")]
        public Shield shield;
        public GameObject shieldInTheBack;

        [Header("Equipment Database")]
        public EquipmentDatabase equipmentDatabase;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_TWO_HANDING_CHANGED, ResetStates);
        }

        private void OnEnable()
        {
            ResetStates();
        }

        bool IsShieldEquipped()
        {
            if (!equipmentDatabase.GetCurrentShield().Exists())
            {
                return false;
            }

            return equipmentDatabase.GetCurrentShield().HasItem(shield);
        }

        public void ResetStates()
        {
            if (!IsShieldEquipped())
            {
                HideShield();
                return;
            }

            if (equipmentDatabase.isTwoHanding || equipmentDatabase.IsBowEquipped())
            {
                ShowBackShield();
                return;
            }

            ShowShield();
        }

        public void HideShield()
        {
            gameObject.SetActive(false);
            shieldInTheBack?.gameObject.SetActive(false);
        }

        public void ShowBackShield()
        {
            if (shieldInTheBack != null)
            {
                shieldInTheBack.SetActive(true);
            }

            gameObject.SetActive(false);
        }

        public void ShowShield()
        {
            gameObject.SetActive(true);

            if (shieldInTheBack != null)
            {
                shieldInTheBack.SetActive(false);
            }
        }

    }
}
