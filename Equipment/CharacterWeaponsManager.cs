namespace AF.Equipment
{
    using UnityEngine;

    public class CharacterWeaponsManager : MonoBehaviour
    {
        public WeaponHitbox leftHandWeapon, rightHandWeapon, headWeapon, leftFootWeapon, rightFootWeapon;
        public GameObject bow;
        public GameObject shield;
        public bool shouldHideShield = true;

        [HideInInspector] public WeaponHitbox currentAttackingWeapon;

        public void ResetStates()
        {
            CloseAllWeaponHitboxes();
        }

        public void ShowEquipment()
        {
            ShowWeapon();
            ShowBow();
            ShowShield();
        }

        public void HideEquipment()
        {
            HideWeapon();
            HideBow();
            HideShield();
        }

        public void ShowWeapon()
        {
            rightHandWeapon?.gameObject.SetActive(true);
            leftHandWeapon?.gameObject.SetActive(true);
        }

        public void HideWeapon()
        {
            rightHandWeapon?.gameObject.SetActive(false);
            leftHandWeapon?.gameObject.SetActive(false);
        }

        public void ShowShield()
        {
            if (shield != null)
            {
                shield.SetActive(true);
            }
        }
        public void HideShield()
        {
            if (shield != null && shouldHideShield)
            {
                shield.SetActive(false);
            }
        }

        public void ShowBow()
        {
            if (bow != null)
            {
                bow.SetActive(true);
            }
        }

        public void HideBow()
        {
            if (bow != null)
            {
                bow.SetActive(false);
            }
        }

        public void OpenLeftHandWeaponHitbox()
        {
            leftHandWeapon?.EnableHitbox();
            this.currentAttackingWeapon = leftHandWeapon;
        }
        public void OpenRightHandWeaponHitbox()
        {
            rightHandWeapon?.EnableHitbox();
            this.currentAttackingWeapon = rightHandWeapon;
        }
        public void OpenLeftFootWeaponHitbox()
        {
            leftFootWeapon?.EnableHitbox();
            this.currentAttackingWeapon = leftFootWeapon;
        }
        public void OpenRightFootWeaponHitbox()
        {
            rightFootWeapon?.EnableHitbox();
            this.currentAttackingWeapon = rightFootWeapon;
        }
        public void OpenHeadWeaponHitbox()
        {
            headWeapon?.EnableHitbox();
            this.currentAttackingWeapon = headWeapon;
        }

        public void CloseAllWeaponHitboxes()
        {
            leftHandWeapon?.DisableHitbox();
            rightHandWeapon?.DisableHitbox();
            leftFootWeapon?.DisableHitbox();
            rightFootWeapon?.DisableHitbox();
            headWeapon?.DisableHitbox();
        }
    }
}
