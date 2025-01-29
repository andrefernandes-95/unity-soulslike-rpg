using UnityEngine;

namespace AF.Shooting
{
    public abstract class CharacterBaseShooter : MonoBehaviour
    {
        public GameObject rifleWeapon;

        [Header("Components")]
        public CharacterBaseManager characterBaseManager;

        public abstract bool CanShoot();

        public abstract void CastSpell();

        public abstract void FireArrow();

        public void ShowRifleWeapon()
        {
            if (rifleWeapon == null)
            {
                return;
            }
            rifleWeapon.gameObject.SetActive(true);
        }

        public void HideRifleWeapon()
        {
            if (rifleWeapon == null)
            {
                return;
            }
            rifleWeapon.gameObject.SetActive(false);
        }
    }
}
