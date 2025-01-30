namespace AF.Health
{
    using UnityEngine;
    using UnityEngine.Events;

    public abstract class CharacterBaseHealth : MonoBehaviour
    {
        public readonly string hashDeath = "Dying";

        [Header("Events")]
        public UnityEvent onStart;
        public UnityEvent onTakeDamage;
        public UnityEvent onRestoreHealth;
        public UnityEvent onDeath;
        public UnityEvent onDamageFromPlayer;
        [HideInInspector] public UnityEvent onHealthChanged;

        [Header("Quests")]
        public Weapon weaponRequiredToKill;
        public bool hasBeenHitWithRequiredWeapon = false;
        public UnityEvent onKilledWithRightWeapon;
        public UnityEvent onKilledWithWrongWeapon;

        [Header("Status")]
        public bool hasHealthCutInHalf = false;

        private void Start()
        {
            onStart?.Invoke();
        }

        public abstract void RestoreHealth(float value);
        public abstract void RestoreFullHealth();

        public float GetCurrentHealthPercentage()
        {
            return GetCurrentHealth() * 100 / GetMaxHealth();
        }

        public abstract void TakeDamage(float value);

        public abstract float GetCurrentHealth();
        public abstract void SetCurrentHealth(float value);

        public abstract int GetMaxHealth();

        public void CheckIfHasBeenKilledWithRightWeapon()
        {
            if (weaponRequiredToKill == null)
            {
                return;
            }

            if (hasBeenHitWithRequiredWeapon)
            {
                onKilledWithRightWeapon?.Invoke();
            }
            else
            {
                onKilledWithWrongWeapon?.Invoke();
            }
        }

        public virtual void SetHasHealthCutInHealth(bool value)
        {
            hasHealthCutInHalf = value;

        }
    }

}
