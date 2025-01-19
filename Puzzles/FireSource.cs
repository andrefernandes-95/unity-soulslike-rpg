namespace AF
{
    using AF.Health;
    using UnityEngine;

    [RequireComponent(typeof(DamageReceiver))]
    public class FireSource : MonoBehaviour
    {
        DamageReceiver damageReceiver => GetComponent<DamageReceiver>();


        private void Awake()
        {
            damageReceiver.onDamageEvent += OnDamageEvent;
        }

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage incomingDamage)
        {
            if (incomingDamage == null)
            {
                return incomingDamage;
            }

            // Add fire to attacking weapon
            incomingDamage.fire += 5;

            return incomingDamage;
        }

    }
}
