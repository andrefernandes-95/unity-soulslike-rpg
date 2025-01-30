
namespace AF
{
    using AF.Animations;
    using AF.Combat;
    using AF.Equipment;
    using AF.Events;
    using AF.Health;
    using AF.Shooting;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.Events;
    using AF.Companions;
    using UnityEngine.AI;
    using AF.Dialogue;
    using System.Collections;

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
        public float ACCELERATION_RECOVER_SPEED = 1f;

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

        public void UpdateAnimatorOverrideControllerClips(string animationName, AnimationClip animationClip)
        {
            if (animatorOverrideController == null)
            {
                SetupAnimatorOverrides();
            }

            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);
            clipOverrides[animationName] = animationClip;
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
            agent.acceleration = 0f;
        }

        public void SetAgentSpeed(float speed)
        {
            agent.speed = speed;

            RestoreAccelerationCoroutine = StartCoroutine(RestoreAcceleration());
        }

        IEnumerator RestoreAcceleration()
        {
            while (agent.acceleration < defaultAcceleration)
            {
                agent.acceleration += Time.deltaTime * ACCELERATION_RECOVER_SPEED;
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

        public void TeleportNearPlayer()
        {
            Vector3 desiredPosition = GetPlayerManager().transform.position + (GetPlayerManager().transform.forward * -4.5f);
            NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 15f, NavMesh.AllAreas);

            if (IsValidPosition(hit.position))
            {
                characterController.enabled = false;
                agent.enabled = false;
                transform.position = hit.position;
                agent.nextPosition = hit.position;
                agent.enabled = true;
                characterController.enabled = true;
            }
        }

        public void Teleport(Vector3 desiredPosition)
        {
            NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

            if (IsValidPosition(hit.position))
            {
                characterController.enabled = false;
                agent.enabled = false;
                transform.position = hit.position;
                agent.nextPosition = hit.position;
                agent.enabled = true;
                characterController.enabled = true;
            }
        }

        public void Teleport(Vector3 desiredPosition, Quaternion desiredRotation)
        {
            characterController.enabled = false;
            agent.enabled = false;
            transform.position = desiredPosition;
            transform.rotation = desiredRotation;
            agent.nextPosition = desiredPosition;
            agent.enabled = true;
            characterController.enabled = true;
        }

        private bool IsValidPosition(Vector3 position)
        {
            // Check for Infinity or NaN values
            return !float.IsInfinity(position.x) && !float.IsInfinity(position.y) && !float.IsInfinity(position.z) &&
                   !float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z);
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
