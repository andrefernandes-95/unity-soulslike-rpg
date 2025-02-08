namespace AF
{
    using AF.Health;
    using UnityEngine;

    public class WeaponHitbox : Hitbox
    {
        [Header("Weapon")]
        public Weapon weapon;

        public override Damage GetDamage()
        {
            throw new System.NotImplementedException();
        }

        public override float GetImpulseForce()
        {
            throw new System.NotImplementedException();
        }
    }
}
