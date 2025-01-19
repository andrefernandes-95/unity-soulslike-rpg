namespace AF
{
    using UnityEngine;

    public class BranchCoveredChest : MonoBehaviour
    {
        public IgnitableSource[] ignitableSources;
        int ignitedBranches = 0;

        public GenericTrigger chestTrigger;

        bool hasPlayedPuzzleSound = false;
        Soundbank _soundbank;

        private void Awake()
        {
            foreach (var ignitableSource in ignitableSources)
            {
                ignitableSource.onIgnited.AddListener(() =>
                {
                    UpdateCounter();
                });
            }

            chestTrigger.DisableCapturable();
        }

        void UpdateCounter()
        {
            ignitedBranches++;

            if (ignitedBranches >= ignitableSources.Length)
            {
                chestTrigger.TurnCapturable();


                if (!hasPlayedPuzzleSound)
                {
                    hasPlayedPuzzleSound = true;
                    GetSoundbank().PlaySound(GetSoundbank().puzzleWon);
                }
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
