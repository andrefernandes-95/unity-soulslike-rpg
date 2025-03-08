namespace AFV2
{
    using UnityEngine;

    public class PlayerBowAimingState : PlayerAimState
    {
        [SerializeField] private string HASH_AIM_IDLE = "Aim Idle";
        [SerializeField] private string HASH_AIM_WALK = "Aim Walk";
        [SerializeField] private string HASH_AIM_SHOT = "Aim Shot";

        [Header("Archery Settings")]
        [SerializeField] float aimWalkSpeed = 2f;

        public override string GetHashAimIdle()
        {
            return HASH_AIM_IDLE;
        }

        public override string GetHashAimWalk()
        {
            return HASH_AIM_WALK;
        }

        public override string GetHashAimShot()
        {
            return HASH_AIM_SHOT;
        }

        public override float GetMoveSpeed()
        {
            return aimWalkSpeed;
        }

        public override void OnLightAttackInput()
        {
            if (!characterApi.characterArchery.CanShoot)
            {
                return;
            }

            characterApi.characterArchery.Shoot();

            characterApi.characterArchery.HideArrowWorld();

            HandleShotAnimation();
        }

        public override void OnEnter()
        {
            characterApi.characterArchery.ShowArrowWorld();
        }

        public override void OnExit()
        {
            characterApi.characterArchery.HideArrowWorld();
        }
    }
}
