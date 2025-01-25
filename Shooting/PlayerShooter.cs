namespace AF.Shooting
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Health;
    using AF.Inventory;
    using Unity.Cinemachine;
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Events;

    public class PlayerShooter : CharacterBaseShooter
    {
        public readonly int hashFireBowLockedOn = Animator.StringToHash("Locked On - Shoot Bow");

        [Header("Stamina Cost")]
        public int minimumStaminaToShoot = 10;

        [Header("Achievements")]
        public Achievement achievementOnShootingBowForFirstTime;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        public GameSession gameSession;

        [Header("Aiming")]
        public GameObject aimingCamera;
        public LookAtConstraint lookAtConstraint;

        public float bowAimCameraDistance = 1.25f;
        public float crossbowAimCameraDistance = 1.5f;
        public float spellAimCameraDistance = 2.25f;

        [Header("Components")]
        public LockOnManager lockOnManager;
        public UIManager uIManager;
        public MenuManager menuManager;

        [Header("Refs")]
        public Transform playerFeetRef;
        public GameObject arrowPlaceholder;

        [Header("Flags")]
        public bool isAiming = false;
        public bool isShooting = false;

        // For cache purposes
        Spell previousSpell;

        [Header("Events")]
        public UnityEvent onSpellAim_Begin;
        public UnityEvent onBowAim_Begin;

        [Header("Cinemachine")]
        Cinemachine3rdPersonFollow cinemachineThirdPersonFollow;

        public CinemachineImpulseSource cinemachineImpulseSource;

        Coroutine FireDelayedProjectileCoroutine;

        public GameObject queuedProjectile;
        public Spell queuedSpell;

        [Header("SFX")]
        public AudioSource combatAudioSource;
        public AudioClip bowDrawSfx;

        private void Awake()
        {
            HideArrowPlaceholder();
        }

        void ShowArrowPlaceholder()
        {
            if (isAiming && equipmentDatabase.IsBowEquipped() && equipmentDatabase.HasEnoughCurrentArrows())
            {
                arrowPlaceholder.SetActive(true);
            }
        }

        void HideArrowPlaceholder()
        {
            arrowPlaceholder.SetActive(false);
        }

        public void ResetStates()
        {
            isShooting = false;
            queuedProjectile = null;
            queuedSpell = null;

            ShowArrowPlaceholder();
        }

        void SetupCinemachine3rdPersonFollowReference()
        {
            if (cinemachineThirdPersonFollow != null)
            {
                return;
            }

            cinemachineThirdPersonFollow = aimingCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnFireInput()
        {
            if (CanShoot())
            {
                if (equipmentDatabase.IsBowEquipped() && equipmentDatabase.HasEnoughCurrentArrows())
                {
                    if (IsRangeWeaponIncompatibleWithProjectile())
                    {
                        return;
                    }

                    ShootBow(equipmentDatabase.GetCurrentArrow()?.GetItem(), transform, lockOnManager.nearestLockOnTarget?.transform);
                    uIDocumentPlayerHUDV2.equipmentHUD.UpdateUI();
                    return;
                }

                PlayerManager playerManager = GetPlayerManager();

                if (
                   equipmentDatabase.IsStaffEquipped()
                   && equipmentDatabase.GetCurrentSpell() != null
                   && playerManager.manaManager.HasEnoughManaForSpell(equipmentDatabase.GetCurrentSpell()?.GetItem()))
                {
                    playerManager.manaManager.DecreaseMana(equipmentDatabase.GetCurrentSpell().GetItem()?.costPerCast ?? 0);

                    HandleSpellCastAnimationOverrides();

                    playerManager.PlayBusyHashedAnimationWithRootMotion(hashCast);
                }
            }
        }

        void HandleSpellCastAnimationOverrides()
        {
            Spell currentSpell = equipmentDatabase.GetCurrentSpell()?.GetItem();

            if (currentSpell == previousSpell)
            {
                return;
            }

            previousSpell = currentSpell;

            bool ignoreSpellsAnimationClips = false;
            if (
                currentSpell.animationCanNotBeOverriden == false &&
                equipmentDatabase.GetCurrentWeapon().Exists() &&
                equipmentDatabase.GetCurrentWeapon().GetItem().ignoreSpellsAnimationClips)
            {
                ignoreSpellsAnimationClips = true;
            }

            if (currentSpell.castAnimationOverride != null && ignoreSpellsAnimationClips == false)
            {
                GetPlayerManager().UpdateAnimatorOverrideControllerClip("Cacildes - Spell - Cast", currentSpell.castAnimationOverride);
                GetPlayerManager().RefreshAnimationOverrideState();
            }
        }

        public void Aim_Begin()
        {
            if (!CanAim())
            {
                return;
            }

            isAiming = true;
            aimingCamera.SetActive(true);
            GetPlayerManager().thirdPersonController.rotateWithCamera = true;
            lockOnManager.DisableLockOn();

            SetupCinemachine3rdPersonFollowReference();

            if (equipmentDatabase.IsBowEquipped())
            {
                GetPlayerManager().animator.SetBool(hashIsAiming, true);

                cinemachineThirdPersonFollow.CameraDistance = (equipmentDatabase.GetCurrentWeapon().Exists() && equipmentDatabase.GetCurrentWeapon().GetItem().isCrossbow)
                    ? crossbowAimCameraDistance : bowAimCameraDistance;

                onBowAim_Begin?.Invoke();
            }
            else if (equipmentDatabase.IsStaffEquipped())
            {
                cinemachineThirdPersonFollow.CameraDistance = spellAimCameraDistance;
                onSpellAim_Begin?.Invoke();
            }

            GetPlayerManager().thirdPersonController.virtualCamera.gameObject.SetActive(false);


            if (bowDrawSfx != null && combatAudioSource != null)
            {
                combatAudioSource.PlayOneShot(bowDrawSfx);
            }

            ShowArrowPlaceholder();
        }

        public void Aim_End()
        {
            if (!isAiming)
            {
                return;
            }

            isAiming = false;
            aimingCamera.SetActive(false);
            lookAtConstraint.constraintActive = false;
            GetPlayerManager().thirdPersonController.rotateWithCamera = false;
            GetPlayerManager().animator.SetBool(hashIsAiming, false);
            GetPlayerManager().thirdPersonController.virtualCamera.gameObject.SetActive(true);

            HideArrowPlaceholder();
        }

        private void Update()
        {
            if (isAiming && equipmentDatabase.IsBowEquipped())
            {
                lookAtConstraint.constraintActive = GetPlayerManager().thirdPersonController._input.move.magnitude <= 0;
            }
        }
        public void ShootBow(ConsumableProjectile consumableProjectile, Transform origin, Transform lockOnTarget)
        {
            if (equipmentDatabase.IsBowEquipped())
            {
                achievementOnShootingBowForFirstTime.AwardAchievement();
            }

            if (equipmentDatabase.GetCurrentArrow().GetItem()?.loseUponFiring ?? false)
            {
                inventoryDatabase.RemoveItem(consumableProjectile);
            }

            GetPlayerManager().staminaStatManager.DecreaseStamina(minimumStaminaToShoot);

            FireProjectile(consumableProjectile.projectile.gameObject, lockOnTarget, null);

            HideArrowPlaceholder();
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        public override void CastSpell()
        {
            ShootSpell(equipmentDatabase.GetCurrentSpell()?.GetItem(), lockOnManager.nearestLockOnTarget?.transform);

            OnShoot();
        }

        public override void FireArrow()
        {
        }

        public void ShootSpell(Spell spell, Transform lockOnTarget)
        {
            if (spell == null)
            {
                return;
            }

            GetPlayerManager().staminaStatManager.DecreaseStamina(minimumStaminaToShoot);

            if (spell.projectile != null)
            {
                FireProjectile(spell.projectile.gameObject, lockOnTarget, spell);
            }
        }

        public void FireProjectile(GameObject projectile, Transform lockOnTarget, Spell spell)
        {
            if (lockOnTarget != null && lockOnManager.isLockedOn)
            {
                var rotation = lockOnTarget.transform.position - characterBaseManager.transform.position;
                rotation.y = 0;
                characterBaseManager.transform.rotation = Quaternion.LookRotation(rotation);
            }

            if (equipmentDatabase.IsBowEquipped())
            {
                if (isAiming)
                {
                    characterBaseManager.PlayBusyHashedAnimation(hashFireBow);
                }
                else
                {
                    characterBaseManager.PlayBusyHashedAnimation(hashFireBowLockedOn);
                }
            }

            queuedProjectile = projectile;
            queuedSpell = spell;
        }

        public void HandleProjectileShot(bool ignoreSpawnFromCamera)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));

            cinemachineImpulseSource.GenerateImpulse();
            isShooting = true;

            if (FireDelayedProjectileCoroutine != null)
            {
                StopCoroutine(FireDelayedProjectileCoroutine);
            }

            float distanceFromCamera = 0f;
            if (queuedProjectile != null)
            {
                distanceFromCamera = 1f;
            }

            (Vector3 origin, Quaternion lookPosition, Ray updatedRay) =
                GetProjectileOriginAndLookPosition(ray, distanceFromCamera, queuedSpell, ignoreSpawnFromCamera);

            HandleProjectile(
                queuedProjectile,
                origin,
                lookPosition,
                updatedRay,
                queuedSpell);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnShoot()
        {
            HandleProjectileShot(false);
            queuedProjectile = null;
            queuedSpell = null;
        }

        (Vector3 origin, Quaternion lookPosition, Ray updatedRay) GetProjectileOriginAndLookPosition(Ray updatedRay, float originDistanceFromCamera, Spell spell, bool ignoreSpawnFromCamera)
        {
            Vector3 origin = updatedRay.GetPoint(originDistanceFromCamera);
            Quaternion lookPosition = Quaternion.identity;

            // If shooting spell but not locked on, use player transform forward to direct the spell
            if (lockOnManager.isLockedOn == false && isAiming == false || spell != null && spell.ignoreSpawnFromCamera || ignoreSpawnFromCamera)
            {
                origin = lookAtConstraint.transform.position;
                updatedRay.direction = characterBaseManager.transform.forward;

                Vector3 lookDir = updatedRay.direction;
                lookDir.y = 0;
                lookPosition = Quaternion.LookRotation(lookDir);
            }

            if (spell != null)
            {
                if (lockOnManager.nearestLockOnTarget != null && spell.spawnOnLockedOnEnemies)
                {
                    origin = lockOnManager.nearestLockOnTarget.transform.position + lockOnManager.nearestLockOnTarget.transform.up;
                }
                else if (spell.spawnAtPlayerFeet)
                {
                    origin = playerFeetRef.transform.position + new Vector3(0, spell.playerFeetOffsetY, 0);
                }
            }

            return (origin, lookPosition, updatedRay);
        }

        void HandleProjectile(GameObject projectile, Vector3 origin, Quaternion lookPosition, Ray updatedRay, Spell spell)
        {
            if (spell != null && spell.statusEffects != null && spell.statusEffects.Length > 0)
            {
                foreach (StatusEffect statusEffect in spell.statusEffects)
                {
                    PlayerManager playerManager = GetPlayerManager();

                    playerManager.statusController.statusEffectInstances.FirstOrDefault(x => x.Key == statusEffect).Value?.onConsumeStart?.Invoke();
                    // For positive effects, we override the status effect resistance to be the duration of the consumable effect
                    playerManager.combatant.statusEffectResistances[statusEffect] = spell.effectsDurationInSeconds;
                    playerManager.statusController.InflictStatusEffect(statusEffect, spell.effectsDurationInSeconds, true);
                }
            }

            if (projectile == null)
            {
                return;
            }

            GameObject projectileInstance = Instantiate(projectile, origin, lookPosition);


            if (spell != null && spell.parentToPlayer)
            {
                projectileInstance.transform.parent = GetPlayerManager().transform;
            }

            IProjectile[] projectileComponents = GetProjectileComponentsInChildren(projectileInstance);

            foreach (IProjectile componentProjectile in projectileComponents)
            {
                componentProjectile.Shoot(characterBaseManager, updatedRay.direction * componentProjectile.GetForwardVelocity(), componentProjectile.GetForceMode());
            }

            HandleProjectileDamageManagers(projectileInstance, spell);
        }

        IProjectile[] GetProjectileComponentsInChildren(GameObject obj)
        {
            List<IProjectile> projectileComponents = new List<IProjectile>();

            if (obj.TryGetComponent(out IProjectile projectile))
            {
                projectileComponents.Add(projectile);
            }

            foreach (Transform child in obj.transform)
            {
                projectileComponents.AddRange(GetProjectileComponentsInChildren(child.gameObject));
            }

            return projectileComponents.ToArray();
        }

        void HandleProjectileDamageManagers(GameObject projectileInstance, Spell currentSpell)
        {
            var playerManager = GetPlayerManager();

            // Process any projectile instance
            if (projectileInstance.TryGetComponent<Projectile>(out var projectile))
            {
                ScaleProjectileDamage(projectile, currentSpell);
            }

            // Process any particle damage instances
            if (projectileInstance.TryGetComponent(out OnDamageCollisionAbstractManager mainManager))
            {
                mainManager.damageOwner = playerManager;

                mainManager.damage = ScaleSpellDamage(mainManager.damage, currentSpell);
            }

            // Process all child damage managers
            var childManagers = GetAllChildOnDamageCollisionManagers(projectileInstance);
            foreach (var childManager in childManagers)
            {
                childManager.damageOwner = playerManager;
                childManager.damage = ScaleSpellDamage(childManager.damage, currentSpell);
            }
        }

        void ScaleProjectileDamage(Projectile projectile, Spell currentSpell)
        {
            var playerManager = GetPlayerManager();

            if (equipmentDatabase.IsBowEquipped())
            {
                projectile.damage = projectile.damage.ScaleProjectile(playerManager.attackStatManager, equipmentDatabase.GetCurrentWeapon());
            }
            else if (equipmentDatabase.IsStaffEquipped())
            {
                projectile.damage = ScaleSpellDamage(projectile.damage, currentSpell);
            }
        }

        Damage ScaleSpellDamage(Damage damage, Spell currentSpell)
        {
            var playerManager = GetPlayerManager();

            if (currentSpell != null)
            {
                var attackStatManager = playerManager.attackStatManager;
                var equipmentDatabase = attackStatManager.equipmentDatabase;
                var currentWeapon = equipmentDatabase.GetCurrentWeapon()?.GetItem();
                var isNightTime = gameSession.IsNightTime();
                var shouldDoubleDamage = currentWeapon != null && (
                    (currentWeapon.doubleDamageDuringNightTime && isNightTime) ||
                    (currentWeapon.doubleDamageDuringDayTime && !isNightTime)
                );
                float multiplier = shouldDoubleDamage ? 2 : 1f;

                if (playerManager.statsBonusController.spellDamageBonusMultiplier > 0)
                {
                    multiplier += playerManager.statsBonusController.spellDamageBonusMultiplier;
                }

                damage = damage.ScaleSpell(attackStatManager, multiplier);
            }

            return damage;
        }

        OnDamageCollisionAbstractManager[] GetAllChildOnDamageCollisionManagers(GameObject obj)
        {
            var managers = new List<OnDamageCollisionAbstractManager>();

            // Include components on the current object
            managers.AddRange(obj.GetComponents<OnDamageCollisionAbstractManager>());

            // Traverse through all children and gather their components
            foreach (Transform child in obj.transform)
            {
                managers.AddRange(child.GetComponentsInChildren<OnDamageCollisionAbstractManager>());
            }

            return managers.ToArray();
        }

        bool CanAim()
        {
            if (GetPlayerManager().IsBusy())
            {
                return false;
            }

            if (GetPlayerManager().thirdPersonController.isSwimming)
            {
                return false;
            }

            return equipmentDatabase.IsBowEquipped() || equipmentDatabase.IsStaffEquipped();
        }

        public override bool CanShoot()
        {
            if (playerStatsDatabase.currentStamina < minimumStaminaToShoot)
            {
                return false;
            }

            if (menuManager.isMenuOpen)
            {
                return false;
            }

            if (uIManager.IsShowingGUI())
            {
                return false;
            }

            if (GetPlayerManager().IsBusy())
            {
                return false;
            }

            if (GetPlayerManager().characterBlockController.isBlocking)
            {
                return false;
            }

            // If not ranged weapons equipped, dont allow shooting
            if (
                !equipmentDatabase.IsBowEquipped()
                && !equipmentDatabase.IsStaffEquipped())
            {
                return false;
            }

            if (GetPlayerManager().thirdPersonController.isSwimming)
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

        PlayerManager GetPlayerManager()
        {
            return characterBaseManager as PlayerManager;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="projectile"></param>
        public void ShootProjectile(GameObject projectile)
        {
            if (lockOnManager.nearestLockOnTarget?.transform != null && lockOnManager.isLockedOn)
            {
                var rotation = lockOnManager.nearestLockOnTarget.transform.transform.position - characterBaseManager.transform.position;
                rotation.y = 0;
                characterBaseManager.transform.rotation = Quaternion.LookRotation(rotation);
            }

            this.queuedProjectile = projectile;

            HandleProjectileShot(true);
            queuedProjectile = null;
            queuedSpell = null;
        }
    }
}
