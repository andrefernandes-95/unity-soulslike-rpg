namespace AF
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

#if UNITY_EDITOR

    [CustomEditor(typeof(Brazier), editorForChildClasses: true)]
    public class BrazierEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            Brazier brazier = target as Brazier;

            if (GUILayout.Button("Lit"))
            {
                brazier.SetIsLit(true);
            }
        }
    }
#endif

    public class Brazier : MonoBehaviour
    {
        public bool isLit = false;

        [Header("Components")]
        public IgnitableSource ignitableSource;
        public GameObject fireSourceContainer;

        [Header("Events")]
        public UnityEvent onBrazierLit;
        public bool playPuzzleSoundOnLit = false;
        bool hasPlayedPuzzleSound = false;
        Soundbank _soundbank;

        private void Awake()
        {
            if (ignitableSource != null)
            {
                ignitableSource.onIgnited.AddListener(() =>
                {
                    if (isLit)
                    {
                        return;
                    }

                    SetIsLit(true);
                });
            }

            SetIsLit(this.isLit);
        }

        public void SetIsLit(bool value)
        {
            this.isLit = value;

            if (this.isLit)
            {
                fireSourceContainer.gameObject.SetActive(true);

                onBrazierLit?.Invoke();

                if (playPuzzleSoundOnLit && !hasPlayedPuzzleSound)
                {
                    hasPlayedPuzzleSound = true;
                    GetSoundbank().PlaySound(GetSoundbank().puzzleWon);
                }
            }
            else
            {
                fireSourceContainer.gameObject.SetActive(false);
            }
        }

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
