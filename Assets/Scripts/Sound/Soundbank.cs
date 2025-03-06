namespace AFV2
{
    using UnityEngine;

    public class Soundbank : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] AudioSource uiAudioSource;

        [Header("UI")]
        [SerializeField] AudioClip buttonEnter;
        [SerializeField] AudioClip buttonClick;
        [SerializeField] AudioClip buttonExit;

        public void ButtonEnter() => uiAudioSource.PlayOneShot(buttonEnter);
        public void ButtonClick() => uiAudioSource.PlayOneShot(buttonClick);
        public void ButtonExit() => uiAudioSource.PlayOneShot(buttonExit);
    }
}