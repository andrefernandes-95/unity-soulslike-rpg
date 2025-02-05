using AF.Music;
using UnityEngine;

namespace AF
{
    public class Soundbank : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;

        [Header("UI Sounds")]
        public AudioClip uiDecision;
        public AudioClip uiHover;
        public AudioClip uiCancel;
        public AudioClip uiItemReceived;
        public AudioClip reputationIncreased;
        public AudioClip reputationDecreased;
        public AudioClip mainMenuOpen;
        public AudioClip uiEquip;
        public AudioClip uiLockOn;
        public AudioClip uiLockOnSwitchTarget;
        public AudioClip switchTwoHand;
        public AudioClip puzzleWon;

        [Header("Movement")]
        public AudioClip cloth;
        public AudioClip dodge;
        public AudioClip impact;


        [Header("Misc Sounds")]
        public AudioClip alert;
        public AudioClip bookFlip;
        public AudioClip coin;
        public AudioClip craftSuccess;
        public AudioClip craftError;
        public AudioClip gameOverFanfare;
        public AudioClip activateLever;
        public AudioClip openHeavyDoor;
        public AudioClip companionJoin;
        public AudioClip companionLeave;
        public AudioClip illusionaryWallSound;
        public AudioClip uiDialogue;
        public AudioClip itemLostWithUse;

        [Header("Weapon Effects Loop")]
        public AudioClip weaponFire;
        public AudioClip weaponFrost;
        public AudioClip weaponMagic;
        public AudioClip weaponLightning;
        public AudioClip weaponDarkness;
        public AudioClip weaponWater;
        public AudioClip weaponSharp;
        public AudioClip weaponPoison;
        public AudioClip weaponBleed;

        private float lastUISoundTime = 0f;
        float _uiSoundDelay = 0.1f;

        public void PlaySound(AudioClip sound)
        {
            if (IsUISound(sound) && Time.time - lastUISoundTime < _uiSoundDelay)
            {
                return;
            }

            if (IsUISound(sound))
            {
                lastUISoundTime = Time.time;
            }

            PlaySound(sound, null);
        }

        public void PlaySound(AudioClip sound, AudioSource audioSource)
        {
            bgmManager.PlaySound(sound, audioSource);
        }

        private bool IsUISound(AudioClip sound)
        {
            return sound == uiDecision || sound == uiHover || sound == uiCancel ||
                   sound == uiItemReceived || sound == reputationIncreased || sound == reputationDecreased ||
                   sound == mainMenuOpen || sound == uiEquip || sound == uiLockOn ||
                   sound == uiLockOnSwitchTarget || sound == switchTwoHand || sound == puzzleWon;
        }

    }

}
