namespace AF
{
    using System.Collections;
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

        public void PlaySounds()
        {
            audioSource.loop = false;

            AudioClip startClip = GetStartClip();
            if (startClip != null)
            {
                audioSource.PlayOneShot(startClip); ;
            }

            StartCoroutine(PlayLoop(startClip?.length ?? 0));
        }

        IEnumerator PlayLoop(float delay)
        {
            yield return new WaitForSeconds(delay);

            AudioClip loopClip = GetLoopClip();

            if (loopClip != null)
            {
                audioSource.loop = true;
                audioSource.PlayOneShot(loopClip);
            }
        }

        public void StopSounds()
        {
            audioSource.Stop();
        }

        AudioClip GetStartClip()
        {
            Soundbank soundbank = GetSoundbank();

            return weaponBuffType switch
            {
                WeaponBuffType.FIRE => soundbank.weaponFireStart,
                WeaponBuffType.FROST => soundbank.weaponFrostStart,
                WeaponBuffType.LIGHTNING => soundbank.weaponLightningStart,
                WeaponBuffType.MAGIC => soundbank.weaponMagicStart,
                WeaponBuffType.DARKNESS => soundbank.weaponDarknessStart,
                WeaponBuffType.WATER => soundbank.weaponWaterStart,
                WeaponBuffType.BLOOD => soundbank.weaponBleedStart,
                WeaponBuffType.POISON => soundbank.weaponPoisonStart,
                WeaponBuffType.SHARPNESS => soundbank.weaponSharpStart,
                _ => null,
            };
        }

        AudioClip GetLoopClip()
        {
            Soundbank soundbank = GetSoundbank();

            return weaponBuffType switch
            {
                WeaponBuffType.FIRE => soundbank.weaponFireLoop,
                WeaponBuffType.FROST => soundbank.weaponFrostLoop,
                WeaponBuffType.LIGHTNING => soundbank.weaponLightningLoop,
                WeaponBuffType.MAGIC => soundbank.weaponMagicLoop,
                WeaponBuffType.DARKNESS => soundbank.weaponDarknessLoop,
                WeaponBuffType.WATER => soundbank.weaponWaterLoop,
                WeaponBuffType.BLOOD => soundbank.weaponBleedLoop,
                WeaponBuffType.POISON => soundbank.weaponPoisonLoop,
                WeaponBuffType.SHARPNESS => soundbank.weaponSharpLoop,
                _ => null,
            };
        }

    }
}