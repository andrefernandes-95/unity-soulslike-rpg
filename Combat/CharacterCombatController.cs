namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Animations;
    using AF.Events;
    using AF.Health;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterCombatController : MonoBehaviour
    {
        public float crossFade = 0.2f;

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Combat Actions")]
        public Transform reactionActionsContainer;
        public readonly List<CombatAction> reactionsToTarget = new();
        public Transform combatActionsContainer;
        public readonly List<CombatAction> combatActions = new();
        public Transform chaseActionsContainer;
        public readonly List<CombatAction> chaseActions = new();

        [Header("Directional")]
        public CombatAction reactionToTargetBehindBack;

        [Header("Combat Options")]
        [Range(0, 100f)] public float chanceToReact = 90f;

        public List<CombatAction> usedCombatActions = new();

        [Header("Animation Settings")]
        public string ANIMATION_CLIP_TO_OVERRIDE_NAME = "Cacildes - Light Attack - 1";
        public string PRE_PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK = "Cacildes - Pre Pre Combo Attack";
        public string PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK = "Cacildes - Pre Combo Attack";
        public string COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK = "Cacildes - Combo Attack";
        public string COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK = "Cacildes - Combo Attack - Follow Up";
        public string hashLightAttack1 = "Light Attack 1";
        public string hashComboAttack = "Combo Attack Initiator";
        public string hashPreComboAttack = "Pre Combo Attack Initiator";
        public string hashPrePreComboAttack = "Pre Pre Combo Attack Initiator";


        [Header("Unity Events")]
        public UnityEvent onResetState;

        public const string AttackSpeedHash = "AttackSpeed";

        [Header("Dodge Counter")]

        public bool listenForDodgeInput = false;
        CharacterAnimationEventListener characterAnimationEventListener;
        public CombatAction combatActionToRespondToDodgeInput;
        public float chanceToReactToDodgeInput = 0.75f;

        [Header("Current Combat")]
        public CombatAction currentCombatAction = null;
        public string currentAnimationToPlay = "";

        private void Awake()
        {
            characterManager.animator.SetFloat(AttackSpeedHash, 1f);

            if (characterManager.characterCombatController.listenForDodgeInput)
            {
                EventManager.StartListening(EventMessages.ON_PLAYER_DODGING_FINISHED, OnPlayerDodgeFinished);
            }

            PopulateCombatActions();
        }

        void PopulateCombatActions()
        {
            PopulateActions(reactionActionsContainer, reactionsToTarget);
            PopulateActions(combatActionsContainer, combatActions);
            PopulateActions(chaseActionsContainer, chaseActions);
        }

        void PopulateActions(Transform container, List<CombatAction> actionList)
        {
            foreach (Transform child in container)
            {
                if (child.gameObject.activeSelf && child.TryGetComponent(out CombatAction combatActionToAdd))
                {
                    actionList.Add(combatActionToAdd);
                }
            }
        }

        public void ResetStates()
        {
            characterManager.animator.SetFloat(AttackSpeedHash, 1f);

            onResetState?.Invoke();

            OnAttackEnd();
        }

        bool CanReact()
        {
            if (reactionsToTarget.Count <= 0)
            {
                return false;
            }

            if (Random.Range(0, 100) > chanceToReact)
            {
                return false;
            }

            return characterManager.targetManager.IsTargetBusy() || characterManager.targetManager.IsTargetShooting();
        }

        bool IsTargetBehind()
        {
            if (characterManager.targetManager == null || characterManager.targetManager.currentTarget == null)
            {
                return false;
            }

            // Calculate vector from enemy to player
            Vector3 toPlayer = characterManager.targetManager.currentTarget.transform.position - characterManager.transform.position;

            // Calculate angle between enemy's forward direction and vector to player
            float angle = Vector3.Angle(characterManager.transform.forward, toPlayer);

            return angle > 90f;
        }

        CombatAction GetCombatAction()
        {
            if (CanReact())
            {
                var shuffledReactions = Randomize(reactionsToTarget.ToArray());

                foreach (CombatAction possibleReaction in shuffledReactions)
                {
                    if (possibleReaction.CanUseCombatAction())
                    {
                        return possibleReaction;
                    }
                }
            }

            if (reactionToTargetBehindBack != null && IsTargetBehind())
            {
                return reactionToTargetBehindBack;
            }

            if (combatActions.Count > 0)
            {
                var shuffledCombatActions = Randomize(combatActions.ToArray());

                foreach (CombatAction possibleCombatAction in shuffledCombatActions)
                {
                    if (possibleCombatAction != null && possibleCombatAction.CanUseCombatAction())
                    {
                        return possibleCombatAction;
                    }
                }
            }

            return null;
        }

        public void ChooseNextCombatAction()
        {
            CombatAction newCombatAction = GetCombatAction();
            if (newCombatAction == null)
            {
                return;
            }

            SetCombatAction(newCombatAction);
        }

        public void UseCombatAction()
        {
            if (currentCombatAction != null && string.IsNullOrEmpty(currentAnimationToPlay) == false)
            {
                this.usedCombatActions.Add(currentCombatAction);
                characterManager.PlayCrossFadeBusyAnimationWithRootMotion(currentAnimationToPlay, crossFade);
                OnAttackStart();

                StartCoroutine(ClearCombatActionFromCooldownList(currentCombatAction));
            }
        }

        public void UseChaseAction()
        {
            CombatAction newCombatAction = null;

            // If target is aiming, let us try to dodge the aim
            if (reactionsToTarget.Count > 0 && characterManager.targetManager.IsTargetShooting())
            {
                var shuffledReactions = Randomize(reactionsToTarget.ToArray());

                foreach (CombatAction possibleReaction in shuffledReactions)
                {
                    if (possibleReaction.CanUseCombatAction())
                    {
                        newCombatAction = possibleReaction;
                        break;
                    }
                }
            }
            else if (chaseActions.Count > 0)
            {
                var shuffledChaseActions = Randomize(chaseActions.ToArray());

                foreach (CombatAction possibleChaseAction in shuffledChaseActions)
                {
                    if (possibleChaseAction.CanUseCombatAction())
                    {
                        newCombatAction = possibleChaseAction;
                        break;
                    }
                }
            }

            if (newCombatAction != null)
            {
                SetCombatAction(newCombatAction);

                UseCombatAction();
            }
        }


        void OnPlayerDodgeFinished()
        {
            if (combatActionToRespondToDodgeInput == null)
            {
                return;
            }

            if (Random.Range(0, 1f) < chanceToReactToDodgeInput)
            {
                return;
            }

            if (characterAnimationEventListener == null)
            {
                characterAnimationEventListener = characterManager.GetComponent<CharacterAnimationEventListener>();
            }

            characterManager.FaceTarget();
            characterAnimationEventListener.RestoreDefaultAnimatorSpeed();

            this.currentCombatAction = combatActionToRespondToDodgeInput;
            SetCombatAction(currentCombatAction);
        }

        IEnumerator ClearCombatActionFromCooldownList(CombatAction combatActionToClear)
        {
            yield return new WaitForSeconds(combatActionToClear.maxCooldown);

            if (usedCombatActions.Contains(combatActionToClear))
            {
                usedCombatActions.Remove(combatActionToClear);
            }
        }

        public void OnAttackStart()
        {
            if (currentCombatAction != null)
            {
                currentCombatAction.onAttack_Start?.Invoke();
            }
        }
        public void OnAttack_HitboxOpen()
        {
            if (currentCombatAction != null)
            {
                currentCombatAction.onAttack_HitboxOpen?.Invoke();
            }
        }
        public void OnAttackEnd()
        {
            if (currentCombatAction != null)
            {
                currentCombatAction.onAttack_End?.Invoke();
                currentCombatAction = null;
                currentAnimationToPlay = "";
            }
        }

        public IEnumerable<CombatAction> Randomize(CombatAction[] source)
        {
            System.Random rnd = new System.Random();
            return source.OrderBy((item) => rnd.Next());
        }

        public void SetCombatAction(CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
            SetupCombatAction();
        }

        public void SetupCombatAction()
        {
            if (currentCombatAction != reactionToTargetBehindBack)
            {
                characterManager.FaceTarget();
            }

            if (currentCombatAction.hasHyperArmor)
            {
                (characterManager.characterPoise as CharacterPoise).hasHyperArmor = true;
            }

            if (currentCombatAction.animationSpeed != 1f)
            {
                characterManager.animator.SetFloat(AttackSpeedHash, currentCombatAction.animationSpeed);
            }

            string animationToPlay = "";

            if (currentCombatAction.attackAnimationClip != null)
            {
                if (currentCombatAction.comboClip3 != null)
                {
                    characterManager.UpdateAnimatorOverrideControllerClips(PRE_PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.attackAnimationClip);
                    characterManager.UpdateAnimatorOverrideControllerClips(PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.comboClip);
                    characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.comboClip2);
                    characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK, currentCombatAction.comboClip3);
                    animationToPlay = hashPrePreComboAttack;
                }
                else if (currentCombatAction.comboClip2 != null)
                {
                    characterManager.UpdateAnimatorOverrideControllerClips(PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.attackAnimationClip);
                    characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.comboClip);
                    characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK, currentCombatAction.comboClip2);
                    animationToPlay = hashPreComboAttack;
                }
                else if (currentCombatAction.comboClip != null)
                {
                    characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.attackAnimationClip);
                    characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK, currentCombatAction.comboClip);
                    animationToPlay = hashComboAttack;
                }
                else
                {
                    characterManager.UpdateAnimatorOverrideControllerClips(ANIMATION_CLIP_TO_OVERRIDE_NAME, currentCombatAction.attackAnimationClip);
                    animationToPlay = hashLightAttack1;
                }
            }
            else if (!string.IsNullOrEmpty(currentCombatAction.attackAnimationName))
            {
                animationToPlay = currentCombatAction.attackAnimationName;
            }

            this.currentAnimationToPlay = animationToPlay;
        }


        public Damage GetCurrentDamage()
        {
            if (characterManager.characterWeaponsManager.currentAttackingWeapon != null)
            {
                Damage attackingWeaponDamage = characterManager.characterWeaponsManager.currentAttackingWeapon.enemyWeaponDamage.Clone();

                return attackingWeaponDamage.Combine(currentCombatAction?.damage);
            }

            return currentCombatAction?.damage?.Copy();
        }
    }
}
