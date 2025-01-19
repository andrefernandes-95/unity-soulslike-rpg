namespace AF
{
    using AF.Health;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(DamageReceiver))]
    public class IgnitableSource : MonoBehaviour
    {
        DamageReceiver damageReceiver => GetComponent<DamageReceiver>();

        public UnityEvent onIgnited;

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

            if (incomingDamage.fire > 0)
            {
                onIgnited?.Invoke();
            }

            return incomingDamage;
        }

    }
}
