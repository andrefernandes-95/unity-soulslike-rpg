namespace AF
{
    using AF.Health;
    using UnityEngine;

    public class UnarmedHitbox : Hitbox
    {
        [Header("Hitbox Damage")]
        public Damage damage;

        public override Damage GetDamage()
        {
            return damage;
        }

        public override float GetImpulseForce()
        {
            return damage.pushForce * impactForce;
        }
    }
}
