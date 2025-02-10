namespace AFV2
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public CharacterApi characterApi;

        [Header("Components")]
        [SerializeField] InputListener inputListener;
        [SerializeField] CameraController cameraController;

        private void Awake()
        {
            inputListener.onChangeCombatStance.AddListener(characterApi.characterEquipment.characterWeapons.ToggleTwoHanding);
        }

        public Quaternion GetPlayerRotation() => Quaternion.Euler(0.0f, cameraController.TargetRotation, 0.0f);

        public bool IsMoving() => inputListener.Move != Vector2.zero;
        public bool IsSprinting() => IsMoving() && inputListener.Sprint;
        public bool IsJumping() => inputListener.Jump && characterApi.characterGravity.Grounded;
        public bool IsLightAttacking() => inputListener.LightAttack && characterApi.characterGravity.Grounded;
    }
}
