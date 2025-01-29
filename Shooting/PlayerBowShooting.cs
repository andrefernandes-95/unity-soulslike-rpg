namespace AF
{
    using UnityEngine;

    public class PlayerBowShooting : MonoBehaviour
    {
        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        public void PlayShootingBowAnimation()
        {

        }

        public bool CanShootBow()
        {
            if (!equipmentDatabase.IsBowEquipped())
            {
                return false;
            }
            if (!equipmentDatabase.HasEnoughCurrentArrows())
            {
                return false;
            }

            if (IsRangeWeaponIncompatibleWithProjectile())
            {
                return false;
            }

            return true;
        }

        bool IsRangeWeaponIncompatibleWithProjectile()
        {
            Weapon currentRangeWeapon = equipmentDatabase.GetCurrentWeapon().GetItem();
            Arrow arrow = equipmentDatabase.GetCurrentArrow().GetItem();

            if (currentRangeWeapon == null || arrow == null)
            {
                return true;
            }

            if (currentRangeWeapon.isHuntingRifle && arrow.isRifleBullet)
            {
                return false;
            }

            if (currentRangeWeapon.isCrossbow && arrow.isBolt)
            {
                return false;
            }

            bool isBow = currentRangeWeapon.isCrossbow == false && currentRangeWeapon.isHuntingRifle == false;
            bool isArrow = arrow.isRifleBullet == false && arrow.isBolt == false;

            if (isBow && isArrow)
            {
                return false;
            }

            return true;
        }
    }
}
