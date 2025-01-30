namespace AF
{
    using AF.Inventory;
    using UnityEngine;

    public class PlayerBowShooting : MonoBehaviour
    {
        readonly int hashFireBow = Animator.StringToHash("Shoot");
        readonly int hashFireBowLockedOn = Animator.StringToHash("Locked On - Shoot Bow");

        [Header("Components")]
        public PlayerManager playerManager;
        public LockOnManager lockOnManager;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public InventoryDatabase inventoryDatabase;

        [Header("VFX")]
        public GameObject arrowPlaceholder;

        [Header("SFX")]
        public AudioSource combatAudioSource;
        public AudioClip bowDrawSfx;

        private void Awake()
        {
            HideArrowPlaceholder();
        }

        public void ResetStates()
        {
            ShowArrowPlaceholder();
        }

        public void PlayBowDrawSfx()
        {
            if (bowDrawSfx != null && combatAudioSource != null)
            {
                combatAudioSource.PlayOneShot(bowDrawSfx);
            }
        }

        public void ShowArrowPlaceholder()
        {
            if (playerManager.playerShootingManager.isAiming && equipmentDatabase.IsBowEquipped() && equipmentDatabase.HasEnoughCurrentArrows())
            {
                arrowPlaceholder.SetActive(true);
            }
        }

        public void HideArrowPlaceholder()
        {
            arrowPlaceholder.SetActive(false);
        }

        public void ShootBow()
        {
            HandleShooting();
            playerManager.uIDocumentPlayerHUDV2.equipmentHUD.UpdateUI();
        }

        void HandleShooting()
        {
            if (!equipmentDatabase.GetCurrentArrow().Exists())
            {
                return;
            }

            Arrow consumableProjectile = equipmentDatabase.GetCurrentArrow().GetItem();
            Transform lockOnTarget = lockOnManager.nearestLockOnTarget?.transform;

            if (consumableProjectile.loseUponFiring)
            {
                inventoryDatabase.RemoveItem(consumableProjectile);
            }

            playerManager.staminaStatManager.DecreaseStamina(consumableProjectile.staminaCostPerCast);

            playerManager.playerShootingManager.FireProjectile(consumableProjectile.projectile.gameObject, lockOnTarget, null);

            PlayShootingBowAnimation();

            HideArrowPlaceholder();
        }

        void PlayShootingBowAnimation()
        {
            playerManager.PlayBusyHashedAnimation(playerManager.playerShootingManager.isAiming
                ? hashFireBow
                : hashFireBowLockedOn);
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
