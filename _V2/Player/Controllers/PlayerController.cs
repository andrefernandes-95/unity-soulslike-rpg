namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class PlayerController : MonoBehaviour
    {
        public CharacterApi characterApi;

        [Header("Components")]
        [SerializeField] InputListener inputListener;
        [SerializeField] CameraController cameraController;

        [Header("Flags")]
        bool hasRightAttackQueued = false;
        public bool HasRightAttackQueued => hasRightAttackQueued;
        bool hasLeftAttackQueued = false;
        public bool HasLeftAttackQueued => hasLeftAttackQueued;

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
                    hasRightAttackQueued = true;
            });
            inputListener.onLeftAttack.AddListener(() =>
            {
                if (CanControlPlayer())
                    hasLeftAttackQueued = true;
            });

            inputListener.onChangeCombatStance.AddListener(() =>
            {
                if (CanControlPlayer())
                    characterApi.characterEquipment.characterWeapons.ToggleTwoHanding();
            });

            inputListener.onMenu.AddListener(ToggleMenu);
        }
        #endregion


        #region Getters
        public Quaternion GetPlayerRotation() => Quaternion.Euler(0.0f, cameraController.TargetRotation, 0.0f);
        #endregion

        #region Is Booleans
        public bool IsMoving() => CanControlPlayer() && inputListener.Move != Vector2.zero;
        public bool IsSprinting() => IsMoving() && inputListener.Sprint && characterApi.characterStats.CharacterStamina.CanSprint();
        public bool IsJumping() => CanControlPlayer() && inputListener.Jump && characterApi.characterGravity.Grounded;
        public bool IsLightAttacking() => CanControlPlayer() && (hasLeftAttackQueued || hasRightAttackQueued) && characterApi.characterGravity.Grounded;
        public bool IsJumpAttacking() => CanControlPlayer() && (hasLeftAttackQueued || hasRightAttackQueued) && !characterApi.characterGravity.Grounded;
        #endregion

        #region Can Booleans
        public bool CanRotatePlayer() => CanControlPlayer();

        public bool CanControlPlayer() => TryGetMainMenu(out MainMenu mainMenu) && mainMenu.IsActive == false;
        #endregion


        #region Main Menu
        void ToggleMenu()
        {
            if (TryGetMainMenu(out MainMenu menu))
            {
                if (menu.IsActive)
                    menu.Hide();
                else
                    menu.Show();
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
        }
        #endregion
    }
}
