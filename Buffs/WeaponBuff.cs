namespace AF
{
    using AF.Health;
    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class WeaponBuff : MonoBehaviour
    {
        AudioSource audioSource => GetComponent<AudioSource>();
        public int appliedDuration = 60;

        public Damage damageModifier;

        public WeaponBuffType weaponBuffType;

        Soundbank _soundbank;

        Soundbank GetSoundbank()
        {
            if (_soundbank == null)
            {
                _soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return _soundbank;
        }

    }
}