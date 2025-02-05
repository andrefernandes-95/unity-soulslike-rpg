
namespace AF
{
    using AF.Combat;
    using AF.Equipment;
    using AF.Events;
    using AF.Health;
    using AF.Shooting;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.Events;
    using AF.Companions;
    using AF.Dialogue;
    using System.Collections;
    using System.Collections.Generic;

    public class CharacterManager : CharacterBaseManager
    {
        public CompanionID companionID;
        public CharacterCombatController characterCombatController;
        public TargetManager targetManager;

        public CharacterBaseShooter characterBaseShooter;
        public CharacterWeaponsManager characterWeaponsManager;
        public CharacterBossController characterBossController;
        public ExecutionManager executionManager;
        public CharacterBackstabController characterBackstabController;
        public StateManager stateManager;
        public CharacterLoot characterLoot;
        public LockOnRef characterLockOnRef;
        public CharacterHUD characterHUD;
        public GreetingMessageController greetingMessageController;
        public CharacterTeleportManager characterTeleportManager;

        // Animator Overrides
        [HideInInspector] public AnimatorOverrideController animatorOverrideController;

        Vector3 initialPosition;
        Quaternion initialRotation;

        [Header("Settings")]
        public float patrolSpeed = 2f;
        public float chaseSpeed = 4.5f;
        public float cutDistanceToTargetSpeed = 12f;
        public float rotationSpeed = 6f;
        float defaultAcceleration;
        float _ACCELERATION_RECOVER_SPEED = 2f;

        Coroutine RestoreAccelerationCoroutine;
        [HideInInspector] public bool isCuttingDistanceToTarget = false;

        [Header("Settings")]
        public bool canRevive = true;
        public bool shouldReturnToInitialPositionOnRevive = true;

        [Header("Face Target Settings")]
        public bool faceTarget = false;
        public float faceTargetDuration = 0.25f;
        public bool alwaysFaceTarget = false;

        [Header("Partners")]
        public CharacterManager[] partners;
        public int partnerOrder = 0;

        [Header("Events")]
        public UnityEvent onResetStates;
        public UnityEvent onForceAgressionTowardsPlayer;

        // Scene Reference
        PlayerManager playerManager;

        int defaultAnimationHash;

        public GameSession gameSession;

        private void Awake()
        {
            SetupAnimatorOverrides();

            initialPosition = transform.position;
            initialRotation = transform.rotation;

            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, Revive);

            damageReceiver.onDamageEvent += OnDamageEvent;

            defaultAcceleration = agent.acceleration;
        }

        void SetupAnimatorOverrides()
        {
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        private void Start()
        {
            defaultAnimationHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        }

        public override void ResetStates()
        {
            isCuttingDistanceToTarget = false;
            animator.applyRootMotion = false;
            isBusy = false;

            characterPosture.ResetStates();
            characterCombatController.ResetStates();
            characterWeaponsManager.ResetStates();
            damageReceiver?.ResetStates();
            onResetStates?.Invoke();

            characterBlockController.ResetStates();

            characterPoise.ResetStates();

            executionManager.ResetStates();

            characterBackstabController.ResetStates();
        }

        public void UpdateAnimatorOverrideControllerClips(Dictionary<string, AnimationClip> animationUpdates)
        {
            if (animatorOverrideController == null)
            {
                SetupAnimatorOverrides();
            }

            AnimationClipOverrides clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);

            foreach (var kvp in animationUpdates)
            {
                string animationName = kvp.Key;
                AnimationClip animationClip = kvp.Value;

                if (animationClip == null)
                {
                    Debug.LogWarning($"Provided animation clip for '{animationName}' is null. Skipping update.");
                    continue;
                }

                clipOverrides[animationName] = animationClip;
            }

            animatorOverrideController.ApplyOverrides(clipOverrides);
        }

        private void OnAnimatorMove()
        {
            if ((faceTarget || alwaysFaceTarget) && targetManager?.currentTarget != null)
            {
                var lookPos = targetManager.currentTarget.transform.position - transform.position;
                lookPos.y = 0;
                var lookRotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }

            if (animator.applyRootMotion)
            {
                Quaternion rootMotionRotation = animator.deltaRotation;
                transform.rotation *= rootMotionRotation;

                // Extract root motion position and rotation from the Animator
                Vector3 rootMotionPosition = animator.deltaPosition + new Vector3(0.0f, -9, 0.0f) * Time.deltaTime;


                if (isCuttingDistanceToTarget && targetManager.currentTarget != null)
                {
                    agent.updatePosition = false;

                    // Move the character towards the target based on root motion and glide speed
                    Vector3 targetPosition = targetManager.currentTarget.transform.position;
                    Vector3 directionToTarget = (targetPosition - transform.position).normalized;

                    if (Vector3.Distance(agent.transform.position, targetPosition) >= agent.stoppingDistance)
                    {
                        rootMotionPosition += directionToTarget * cutDistanceToTargetSpeed * Time.deltaTime;
                    }
                }
                else
                {
                    agent.updatePosition = true;
                    agent.Warp(characterController.transform.position);
                }

                // Apply root motion to the NavMesh Agent
                if (characterController.enabled)
                {
                    characterController.Move(rootMotionPosition);
                }
            }
        }

        public override Damage GetAttackDamage()
        {
            return characterCombatController.GetCurrentDamage();
        }

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage damage)
        {
            targetManager.SetTarget(attacker);
            ResetFaceTargetFlag();
            return damage;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FaceTarget()
        {
            faceTarget = true;
            Invoke(nameof(ResetFaceTargetFlag), faceTargetDuration);
        }

        public void ResetFaceTargetFlag()
        {
            faceTarget = false;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FacePlayer()
        {
            var lookPos = GetPlayerManager().transform.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }

        PlayerManager GetPlayerManager()
        {
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }

            return playerManager;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FaceInitialRotation()
        {
            transform.rotation = initialRotation;
        }

        public void StopAgentSpeed()
        {
            if (RestoreAccelerationCoroutine != null)
            {
                StopCoroutine(RestoreAccelerationCoroutine);
            }

            agent.speed = 0f;
            agent.acceleration = defaultAcceleration;
        }

        public void SetAgentSpeed(float speed)
        {
            if (speed == agent.speed)
            {
                return;
            }

            agent.speed = speed;
            RestoreAccelerationCoroutine = StartCoroutine(RestoreAcceleration());
        }

        IEnumerator RestoreAcceleration()
        {
            agent.acceleration = 0f;

            float multiplierSpeed = defaultAcceleration - agent.acceleration;

            while (agent.acceleration < defaultAcceleration)
            {
                agent.acceleration += Time.deltaTime * multiplierSpeed;

                multiplierSpeed = defaultAcceleration - agent.acceleration;

                if (multiplierSpeed <= 2)
                {
                    multiplierSpeed = 2;
                }

                yield return null;
            }

            agent.acceleration = defaultAcceleration;
        }

        public void Revive()
        {
            if (characterBossController.IsBoss() || !canRevive)
            {
                return;
            }

            StopAgentSpeed();

            targetManager.ClearTarget();

            if (health is CharacterHealth characterHealth)
            {
                characterHealth.Revive();

                if (IsCompanion() == false)
                {
                    if (shouldReturnToInitialPositionOnRevive)
                    {
                        agent.Warp(initialPosition);
                        characterController.enabled = false;
                        transform.SetPositionAndRotation(initialPosition, initialRotation);
                        characterController.enabled = true;
                    }
                }

                ResetStates();

                characterPosture.currentPostureDamage = 0;

                if (defaultAnimationHash != -1)
                {
                    animator.Play(defaultAnimationHash);
                }
            }

            stateManager.ResetDefaultState();
        }

        public string GetCharacterID()
        {
            return companionID.GetCompanionID();
        }

        public bool IsCompanion()
        {
            return companionID != null;
        }

        public void EnableComponents()
        {
            stateManager.enabled = true;
            characterHUD.gameObject.SetActive(true);
            agent.enabled = true;
            HandleCollisions(true);
        }

        public void DisableComponents()
        {
            stateManager.enabled = false;
            characterHUD.gameObject.SetActive(false);
            agent.enabled = false;
            HandleCollisions(false);
        }

        void HandleCollisions(bool activate)
        {
            characterController.enabled = activate;

            if (characterLockOnRef != null && characterLockOnRef.TryGetComponent<SphereCollider>(out var sphereCollider))
            {
                sphereCollider.enabled = activate;
            }
        }

        public void ForceCombatWithPlayer()
        {
            targetManager.SetPlayerAsTarget();

            stateManager.ScheduleState(stateManager.combatState);

            if (characterBossController.isBoss)
            {
                characterBossController.BeginBossBattle();
            }
        }
    }
}
