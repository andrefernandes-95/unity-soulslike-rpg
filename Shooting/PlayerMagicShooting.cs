namespace AF
{
    using UnityEngine;

    public class PlayerMagicShooting : MonoBehaviour
    {
        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        readonly int hashTwoHandCast = Animator.StringToHash("Two Hand Casting");
        readonly int hashOneHandCast = Animator.StringToHash("One Hand Casting");

        public void PlayCastingAnimation()
        {
            if (equipmentDatabase.isTwoHanding || !HasWeaponOrShieldOnSecondarySlot())
            {
                playerManager.PlayBusyHashedAnimationWithRootMotion(hashTwoHandCast);
            }
            else
            {
                playerManager.PlayBusyHashedAnimationWithRootMotion(hashOneHandCast);
            }
        }

        bool HasWeaponOrShieldOnSecondarySlot()
            => equipmentDatabase.GetCurrentSecondaryWeapon() != null || equipmentDatabase.GetCurrentShield() != null;

    }
}
