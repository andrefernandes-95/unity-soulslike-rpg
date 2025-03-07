namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class DodgeState : State
    {
        [SerializeField] private string HASH_DODGE = "Dodge";
        [SerializeField] float dodgeBlend = 0.05f;

        [Header("Settings")]
        [SerializeField] bool useRootMotion = true;
        [SerializeField] float dodgeSpeed = 1f;

        [Header("Components")]
        public CharacterApi characterApi;

        [Header("Transition State")]
        public FallState fallState;
        public State runState;
        State returnState;

        public override async void OnStateEnter()
        {
            characterApi.characterStamina.UseDodge();

            returnState = this;
            characterApi.animatorManager.BlendTo(HASH_DODGE, dodgeBlend);

            if (useRootMotion)
            {
                characterApi.animatorManager.EnableRootMotion();
            }

            await characterApi.animatorManager.WaitForAnimationToFinish(HASH_DODGE, 0.8f);

            if (useRootMotion)
            {
                characterApi.animatorManager.DisableRootMotion();
            }

            characterApi.characterMovement.EnableRotation();

            returnState = runState;
        }

        public override Task OnStateExit() => Task.CompletedTask;

        public override State Tick()
        {
            characterApi.characterMovement.Move(dodgeSpeed, characterApi.transform.rotation);

            return returnState;
        }
    }
}
