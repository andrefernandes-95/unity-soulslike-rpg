namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class FallState : State
    {
        [Header("Transition")]
        [SerializeField] private string HASH_FALL = "Fall";
        [SerializeField] private float fallBlendTime = 0.3f;
        [SerializeField] private string HASH_LAND = "Land";
        [SerializeField] private float landBlendTime = 0.15f;
        [SerializeField] private float minimumFallHeightToPlayLandAnimation = 3f;

        [Header("Components")]
        public CharacterApi characterApi;

        [Header("Transition State")]
        public State groundedState;
        public AirAttackState airAttackState;

        public override void OnStateEnter()
        {
            characterApi.characterGravity.UpdateFallBegin();

            characterApi.animatorManager.BlendTo(HASH_FALL, fallBlendTime);
        }

        public override async Task OnStateExit()
        {
            // If fall height is too small, skip the landing animation entirely
            if (characterApi.characterGravity.GetFallHeight() < minimumFallHeightToPlayLandAnimation)
            {
                return; // Just return without waiting for animation
            }

            // Play the landing animation
            characterApi.animatorManager.BlendTo(HASH_LAND, landBlendTime);

            // Wait until the animation fully plays out
            await characterApi.animatorManager.WaitForAnimationToFinish(HASH_LAND);
        }

        public override State Tick()
        {
            if (characterApi.characterGravity.Grounded)
                return groundedState;

            return this;
        }

    }
}
