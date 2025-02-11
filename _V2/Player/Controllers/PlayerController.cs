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
        bool hasRightAttackQueued = false;
        public bool HasRightAttackQueued => hasRightAttackQueued;
        bool hasLeftAttackQueued = false;
        public bool HasLeftAttackQueued => hasLeftAttackQueued;

        MainMenu cachedMainMenu;

        private void Awake()
        {
            inputListener.onRightAttack.AddListener(() =>
            {
                hasRightAttackQueued = true;
            });
            inputListener.onLeftAttack.AddListener(() =>
            {
                hasLeftAttackQueued = true;
            });

            inputListener.onChangeCombatStance.AddListener(characterApi.characterEquipment.characterWeapons.ToggleTwoHanding);

            inputListener.onMenu.AddListener(ToggleMenu);
        }

        public Quaternion GetPlayerRotation() => Quaternion.Euler(0.0f, cameraController.TargetRotation, 0.0f);

        public bool IsMoving() => inputListener.Move != Vector2.zero;
        public bool IsSprinting() => IsMoving() && inputListener.Sprint;
        public bool IsJumping() => inputListener.Jump && characterApi.characterGravity.Grounded;
        public bool IsLightAttacking() => (hasLeftAttackQueued || hasRightAttackQueued) && characterApi.characterGravity.Grounded;

        public void ResetCombatFlags()
        {
            hasRightAttackQueued = false;
            hasLeftAttackQueued = false;
        }

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
                    menu.Show(characterApi);
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
    }
}
