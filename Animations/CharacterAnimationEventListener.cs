namespace AF.Animations
{
    using AYellowpaper.SerializedCollections;
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterAnimationEventListener : MonoBehaviour, IAnimationEventListener
    {

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Animator Settings")]
        public string speedParameter = "Speed";
        public float animatorSpeed = 1f;
        public bool ignoreAnimatorSpeed = false;
        public float overrideChaseSpeed = -1f;

        [Header("Animation Clip Overrides")]
        public SerializedDictionary<string, AnimationClip> clipOverrides;

        [Header("Unity Events")]
        public UnityEvent onLeftFootstep;
        public UnityEvent onRightFootstep;
        public UnityEvent onBuff;
        public UnityEvent onCloth;
        public UnityEvent onImpact;
        public UnityEvent onBlood;

        float defaultAnimatorSpeed;

        private void Awake()
        {
            characterManager.animator.speed = animatorSpeed;
            defaultAnimatorSpeed = animatorSpeed;
        }

        private void Start()
        {
            OverrideAnimatorClips();

            if (ignoreAnimatorSpeed)
            {
                characterManager.animator.SetFloat(speedParameter, 0f);
            }
        }

        void OverrideAnimatorClips()
        {
            characterManager.UpdateAnimatorOverrideControllerClips(clipOverrides);
        }

        private void OnAnimatorMove()
        {
            if (ignoreAnimatorSpeed || characterManager.isCuttingDistanceToTarget)
            {
                return;
            }

            if (characterManager.isBusy)
            {
                characterManager.animator.SetFloat(speedParameter, 0f);
                return;
            }

            if (overrideChaseSpeed >= 0 && characterManager.agent.speed > 0)
            {
                characterManager.animator.SetFloat(speedParameter, overrideChaseSpeed);
            }
            else
            {
                characterManager.animator.SetFloat(speedParameter, Mathf.Clamp01(characterManager.agent.speed / characterManager.chaseSpeed));
            }
        }

        public void OnLeftFootstep()
        {
            onLeftFootstep?.Invoke();
        }

        public void OnRightFootstep()
        {
            onRightFootstep?.Invoke();
        }

        public void OpenHeadWeaponHitbox()
        {
            characterManager.characterWeaponsManager.OpenHeadWeaponHitbox();
            characterManager.characterCombatController.OnAttack_HitboxOpen();
        }

        public void CloseHeadWeaponHitbox()
        {
            characterManager.characterWeaponsManager.CloseAllWeaponHitboxes();
        }

        public void OpenLeftWeaponHitbox()
        {
            characterManager.characterWeaponsManager.OpenLeftHandWeaponHitbox();
            characterManager.characterCombatController.OnAttack_HitboxOpen();
        }

        public void CloseLeftWeaponHitbox()
        {
            characterManager.characterWeaponsManager.CloseAllWeaponHitboxes();
        }

        public void OpenRightWeaponHitbox()
        {
            characterManager.characterWeaponsManager.OpenRightHandWeaponHitbox();
            characterManager.characterCombatController.OnAttack_HitboxOpen();
        }

        public void CloseRightWeaponHitbox()
        {
            characterManager.characterWeaponsManager.CloseAllWeaponHitboxes();
        }

        public void OpenLeftFootHitbox()
        {
            characterManager.characterWeaponsManager.OpenLeftFootWeaponHitbox();
            characterManager.characterCombatController.OnAttack_HitboxOpen();
        }

        public void CloseLeftFootHitbox()
        {
            characterManager.characterWeaponsManager.CloseAllWeaponHitboxes();
        }

        public void OpenRightFootHitbox()
        {
            characterManager.characterWeaponsManager.OpenRightFootWeaponHitbox();
            characterManager.characterCombatController.OnAttack_HitboxOpen();
        }

        public void CloseRightFootHitbox()
        {
            characterManager.characterWeaponsManager.CloseAllWeaponHitboxes();
        }

        public void EnableRotation()
        {
        }

        public void DisableRotation()
        {
        }

        public void FaceTarget()
        {
            if (characterManager.targetManager.currentTarget == null)
            {
                return;
            }

            characterManager.FaceTarget();
        }

        public void EnableRootMotion()
        {
            characterManager.animator.applyRootMotion = true;
        }

        public void DisableRootMotion()
        {
            characterManager.animator.applyRootMotion = false;
        }

        public void OnSpellCast()
        {
            characterManager.characterBaseShooter.CastSpell();
        }

        public void OnFireArrow()
        {
            characterManager.characterBaseShooter.FireArrow();
        }

        public void OnCloth()
        {
            onCloth?.Invoke();
        }

        public void OnImpact()
        {
            onImpact?.Invoke();
        }

        public void OnBuff()
        {
            onBuff?.Invoke();
        }

        public void OnThrow()
        {
        }

        public void OnBlood()
        {
            onBlood?.Invoke();
        }

        public void RestoreDefaultAnimatorSpeed()
        {
            this.animatorSpeed = defaultAnimatorSpeed;
            characterManager.animator.speed = animatorSpeed;
        }

        public void SetAnimatorSpeed(float speed)
        {
            this.animatorSpeed = speed;
            characterManager.animator.speed = animatorSpeed;
        }

        public void OnShakeCamera()
        {
        }

        public void DropIKHelper()
        {
        }

        public void UseIKHelper()
        {
        }

        public void SetCanTakeDamage_False()
        {
            if (characterManager == null || characterManager.damageReceiver == null)
            {
                return;
            }
            characterManager.damageReceiver.SetCanTakeDamage(false);
        }

        public void OnFireMultipleArrows()
        {
            characterManager.characterBaseShooter.FireArrow();

        }

        public void OnWeaponSpecial()
        {
        }

        public void MoveTowardsTarget()
        {
            characterManager.isCuttingDistanceToTarget = true;
        }

        public void StopMoveTowardsTarget()
        {
            characterManager.isCuttingDistanceToTarget = false;
        }

        public void OnSwim()
        {

        }

        public void PauseAnimation()
        {
            // Allow a chance to not do the slow down
            if (Random.Range(0, 1f) > 0.5)
            {
                return;
            }

            SetAnimatorSpeed(Random.Range(0.1f, 0.3f));
        }

        public void ResumeAnimation()
        {
            RestoreDefaultAnimatorSpeed();
        }

        public bool ShouldResetAnimationSpeed()
        {
            return defaultAnimatorSpeed != characterManager.animator.speed;
        }

        public void StopIframes()
        {
        }
        public void EnableIframes()
        {

        }
        public void OnCard()
        {

        }

        public void ShowShield()
        {
        }

        public void OnExecuted()
        {
            characterManager.executionManager.OnExecuted();
        }

        public void OnExecuting()
        {
        }

        public void ShowRifleWeapon()
        {
            characterManager.characterBaseShooter.ShowRifleWeapon();
        }
        public void HideRifleWeapon()
        {
            characterManager.characterBaseShooter.HideRifleWeapon();
        }

        public void OnPushObject()
        {
        }
    }
}
