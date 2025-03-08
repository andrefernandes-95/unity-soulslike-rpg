namespace AFV2
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public CharacterApi characterApi;

        [Header("Components")]
        [SerializeField] InputListener inputListener;
        [SerializeField] CameraController cameraController;

        [Header("Flags")]
        bool hasHeavyAttackQueued = false;
        public bool HasHeavyAttackQueued => hasHeavyAttackQueued;
        bool hasRightAttackQueued = false;
        public bool HasRightAttackQueued => hasRightAttackQueued;
        bool hasLeftAttackQueued = false;
        public bool HasLeftAttackQueued => hasLeftAttackQueued;

        bool isAiming = false;

        MainMenu cachedMainMenu;

        private void Awake()
        {
            AttachListeners();
        }

        #region Attach Listeners to Input Actions
        void AttachListeners()
        {
            inputListener.onRightAttack.AddListener(() =>
            {
                if (CanControlPlayer())
                {
                    hasRightAttackQueued = true;
                }
            });

            inputListener.onBlockOrAim_Start.AddListener(() =>
            {
                if (CanControlPlayer())
                {
                    if (characterApi.characterWeapons.HasRangeWeapon())
                    {
                        isAiming = true;
                    }
                    else
                    {
                        hasLeftAttackQueued = true;
                    }
                }
            });
            inputListener.onBlockOrAim_End.AddListener(() =>
            {
                if (CanControlPlayer())
                {
                    isAiming = false;
                }
            });

            inputListener.onHeavyAttack.AddListener(() =>
            {
                if (CanControlPlayer())
                {
                    hasHeavyAttackQueued = true;
                }
            });

            inputListener.onChangeCombatStance.AddListener(() =>
            {
                if (CanControlPlayer())
                {
                    characterApi.characterWeapons.ToggleTwoHanding();
                }
            });

            inputListener.onMenu.AddListener(ToggleMenu);

            inputListener.onSwitchRightWeapon.AddListener(characterApi.characterWeapons.SwitchRightWeapon);
            inputListener.onSwitchLeftWeapon.AddListener(characterApi.characterWeapons.SwitchLeftWeapon);
            inputListener.onSwitchSpell.AddListener(characterApi.characterArchery.SwitchArrow);
        }
        #endregion


        #region Getters
        public Quaternion GetPlayerRotation() => Quaternion.Euler(0.0f, cameraController.TargetRotation, 0.0f);
        #endregion

        #region Is Booleans
        public bool IsMoving() => CanControlPlayer() && inputListener.Move != Vector2.zero;
        public bool IsSprinting() => IsMoving() && inputListener.Sprint && characterApi.characterStamina.CanSprint();
        public bool IsDodging() => inputListener.Dodge && characterApi.characterStamina.CanDodge();
        public bool IsJumping() => CanControlPlayer() && inputListener.Jump && characterApi.characterGravity.Grounded && characterApi.characterStamina.CanJump();
        public bool IsLightAttacking() => CanControlPlayer() && (IsLeftAttackQueueded() || hasRightAttackQueued) && characterApi.characterGravity.Grounded;
        public bool IsJumpAttacking() => CanControlPlayer() && (IsLeftAttackQueueded() || hasRightAttackQueued) && !characterApi.characterGravity.Grounded;
        public bool IsHeavyAttacking() => CanControlPlayer() && hasHeavyAttackQueued && characterApi.characterGravity.Grounded;

        bool IsLeftAttackQueueded()
        {
            if (IsAiming())
            {
                return false;
            }

            return HasLeftAttackQueued;
        }

        public bool IsAiming()
        {
            if (characterApi.characterWeapons.HasRangeWeapon())
            {
                return isAiming;
            }

            return false;
        }
        #endregion

        #region Can Booleans
        public bool CanRotatePlayer() => CanControlPlayer();

        public bool CanControlPlayer() => TryGetMainMenu(out MainMenu mainMenu) && mainMenu.IsActive() == false;
        #endregion


        #region Main Menu
        void ToggleMenu()
        {
            if (TryGetMainMenu(out MainMenu menu))
            {
                if (menu.IsActive())
                {
                    menu.Hide();
                }
                else
                {
                    menu.Show();
                }
            }
        }

        bool TryGetMainMenu(out MainMenu localValue)
        {
            localValue = cachedMainMenu;

            if (localValue == null)
                localValue = FindAnyObjectByType<MainMenu>(FindObjectsInactive.Include);

            return localValue != null;
        }
        #endregion

        #region Combat Flags
        public void ResetCombatFlags()
        {
            hasRightAttackQueued = false;
            hasLeftAttackQueued = false;
            hasHeavyAttackQueued = false;
        }
        #endregion

        public Vector3 GetLook() => inputListener.Look;
    }
}
