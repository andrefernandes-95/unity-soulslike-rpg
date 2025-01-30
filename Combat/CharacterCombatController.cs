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

        private void OnEnable()
        {
            usedCombatActions.Clear();
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

                characterManager.PlayBusyAnimationWithRootMotion(currentAnimationToPlay);

                OnAttackStart();

                StartCoroutine(ClearCombatActionFromCooldownList(currentCombatAction));
            }
        }

        public void UseChaseAction()
        {
            SetupChaseAction();
            UseCombatAction();
        }

        void SetupChaseAction()
        {
            CombatAction newCombatAction = null;

            // If target is aiming, let us try to dodge the aim
            if (reactionsToTarget.Count > 0 && characterManager.targetManager.IsTargetShooting())
            {
                newCombatAction = GetReactionAction();
            }
            else if (chaseActions.Count > 0)
            {
                newCombatAction = GetChaseAction();
            }

            if (newCombatAction != null)
            {
                SetCombatAction(newCombatAction);
            }
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
                    Dictionary<string, AnimationClip> clips = new() {
                        { PRE_PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.attackAnimationClip },
                        { PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.comboClip },
                        { COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.comboClip2 },
                        { COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK, currentCombatAction.comboClip3 },
                    };

                    characterManager.UpdateAnimatorOverrideControllerClips(clips);
                    animationToPlay = hashPrePreComboAttack;
                }
                else if (currentCombatAction.comboClip2 != null)
                {
                    Dictionary<string, AnimationClip> clips = new() {
                        { PRE_COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.attackAnimationClip },
                        { COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.comboClip },
                        { COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK, currentCombatAction.comboClip2 },
                    };

                    characterManager.UpdateAnimatorOverrideControllerClips(clips);
                    animationToPlay = hashPreComboAttack;
                }
                else if (currentCombatAction.comboClip != null)
                {
                    Dictionary<string, AnimationClip> clips = new() {
                        { COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.attackAnimationClip },
                        { COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK, currentCombatAction.comboClip },
                    };

                    characterManager.UpdateAnimatorOverrideControllerClips(clips);
                    animationToPlay = hashComboAttack;
                }
                else
                {
                    Dictionary<string, AnimationClip> clips = new() {
                        { ANIMATION_CLIP_TO_OVERRIDE_NAME, currentCombatAction.attackAnimationClip },
                    };

                    characterManager.UpdateAnimatorOverrideControllerClips(clips);
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

        public void OnAttackStart()
        {
            if (currentCombatAction != null)
            {
                currentCombatAction.onAttack_Start?.Invoke();
                currentCombatAction.PlayGruntSfx();
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

        IEnumerable<CombatAction> Randomize(CombatAction[] source)
        {
            System.Random rnd = new();
            return source.OrderBy((item) => rnd.Next());
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

        CombatAction GetReactionAction()
        {
            var shuffledReactions = Randomize(reactionsToTarget.ToArray());

            foreach (CombatAction possibleReaction in shuffledReactions)
            {
                if (possibleReaction.CanUseCombatAction())
                {
                    return possibleReaction;
                }
            }

            return null;
        }

        CombatAction GetAttackAction()
        {
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

        CombatAction GetCombatAction()
        {
            if (CanReact())
            {
                return GetReactionAction();
            }

            if (reactionToTargetBehindBack != null && CombatUtils.IsTargetBehind(characterManager))
            {
                return reactionToTargetBehindBack;
            }

            return GetAttackAction();
        }

        CombatAction GetChaseAction()
        {
            var shuffledChaseActions = Randomize(chaseActions.ToArray());

            foreach (CombatAction possibleChaseAction in shuffledChaseActions)
            {
                if (possibleChaseAction.CanUseCombatAction())
                {
                    return possibleChaseAction;
                }
            }

            return null;
        }
    }
}
