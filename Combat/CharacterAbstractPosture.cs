namespace AF
{
    using System.Collections;
    using AF.Health;
    using UnityEngine;
    using UnityEngine.Events;

    public abstract class CharacterAbstractPosture : MonoBehaviour
    {
        [Header("Posture")]
        public float currentPostureDamage;
        public readonly float POSTURE_BREAK_BONUS_MULTIPLIER = 2.5f;

        [Header("Damage")]
        public float damageMultiplierWhenStunned = 3f;

        [Header("Unity Events")]
        public UnityEvent onPostureBreakDamage;
        public UnityEvent onDamageWhileStunned;

        [Header("Animations")]
        public string postureBreakAnimationClip = "Posture Break";
        public string postureBreakExitAnimationClip = "Posture Break Exit";

        [Header("Components")]
        public CharacterBaseHealth health;

        [Header("Optional AI Components")]

        public UnityEngine.UI.Slider postureBarSlider;
        public CharacterBaseManager characterBaseManager;
        public bool isStunned = false;
        private bool isDecreasingPosture = false;

        private void Start()
        {
            InitializePostureHUD();
        }

        private void Update()
        {
            UpdatePosture();
        }

        public void ResetStates()
        {
            isStunned = false;
        }

        public abstract int GetMaxPostureDamage();

        public void InitializePostureHUD()
        {
            if (postureBarSlider != null)
            {
                postureBarSlider.maxValue = GetMaxPostureDamage() * 0.01f;
                postureBarSlider.value = currentPostureDamage * 0.01f;
                postureBarSlider.gameObject.SetActive(false);
            }
        }

        void UpdatePosture()
        {
            if (health.GetCurrentHealth() <= 0)
            {
                return;
            }

            if (postureBarSlider != null)
            {
                postureBarSlider.maxValue = GetMaxPostureDamage() * 0.01f;
                postureBarSlider.value = currentPostureDamage * 0.01f;
                postureBarSlider.gameObject.SetActive(currentPostureDamage > 0);
            }

            if (isDecreasingPosture)
            {
                if (currentPostureDamage > 0)
                {
                    currentPostureDamage -= Time.deltaTime * GetPostureDecreateRate();
                }
                else
                {
                    isDecreasingPosture = false;
                }
            }
        }

        public virtual bool TakePostureDamage(int extraPostureDamage)
        {
            var postureDamage = 0;
            if (extraPostureDamage != 0)
            {
                postureDamage = extraPostureDamage;
            }

            currentPostureDamage = Mathf.Clamp(currentPostureDamage + postureDamage, 0, GetMaxPostureDamage());
            StartCoroutine(BeginDecreasingPosture());

            if (currentPostureDamage >= GetMaxPostureDamage())
            {
                BreakPosture();
                return true;
            }

            return false;
        }

        public abstract bool CanPlayPostureDamagedEvent();

        IEnumerator BeginDecreasingPosture()
        {
            isDecreasingPosture = false;
            yield return new WaitForSeconds(1);
            isDecreasingPosture = true;
        }

        public void BreakPosture()
        {
            if (CanPlayPostureDamagedEvent())
            {
                onPostureBreakDamage?.Invoke();

                characterBaseManager.PlayBusyAnimationWithRootMotion(postureBreakAnimationClip);
            }

            HandlePostureBreak();
        }

        public void HandlePostureBreak()
        {
            currentPostureDamage = 0f;
            isStunned = true;
            characterBaseManager.combatant.PlayKnockdown(characterBaseManager.combatAudioSource);
        }

        public void RecoverFromStunned()
        {
            if (characterBaseManager is CharacterManager character && character.characterBackstabController.isBeingBackstabbed)
            {
                return;
            }

            isStunned = false;
            onDamageWhileStunned?.Invoke();
            characterBaseManager.PlayCrossFadeBusyAnimationWithRootMotion(postureBreakExitAnimationClip, .5f);
        }

        public abstract float GetPostureDecreateRate();

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage incomingDamage)
        {
            if (isStunned)
            {
                RecoverFromStunned();

                if (incomingDamage == null) return incomingDamage;

                // Apply stunned bonus damage
                return incomingDamage.ApplyMultiplier(damageMultiplierWhenStunned);
            }

            if (incomingDamage == null)
            {
                return incomingDamage;
            }

            if (receiver is PlayerManager playerManager && playerManager.playerBlockController.isBlocking && playerManager.playerBlockController.CanParry(incomingDamage))
            {
                return incomingDamage;
            }

            bool isPostureBroken = TakePostureDamage(incomingDamage.postureDamage);

            if (isPostureBroken)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * POSTURE_BREAK_BONUS_MULTIPLIER);
            }

            return incomingDamage;
        }
    }

}
