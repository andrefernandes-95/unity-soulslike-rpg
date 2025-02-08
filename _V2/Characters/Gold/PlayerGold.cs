namespace AFV2
{
    using CI.QuickSave;
    using UnityEngine;

    public class PlayerGold : CharacterGold, ISaveable
    {
        const string GOLD = "gold";
        const string LOST_GOLD = "lostGold";
        const string LOST_GOLD_SCENE = "lostGoldScene";
        const string LOST_GOLD_POSITION = "lostGoldPosition";

        private float lostGold = 0;
        public float LostGold
        {
            get => lostGold;
            private set => lostGold = Mathf.Max(0, value);
        }

        private string lostGoldScene;
        public string LostGoldScene
        {
            get => lostGoldScene;
            private set => lostGoldScene = value;
        }

        private Vector3 lostGoldPosition;
        public Vector3 LostGoldPosition
        {
            get => lostGoldPosition;
            private set => lostGoldPosition = value;
        }

        public void SetLostGold(int lostGold, string scene, Vector3 position)
        {
            LostGold = lostGold;
            LostGoldScene = scene;
            LostGoldPosition = position;
        }

        public void ClearLostGold()
        {
            LostGold = 0;
            LostGoldScene = "";
            LostGoldPosition = Vector3.zero;
        }

        public bool HasLostGold() => LostGold != 0;

        public void LoadData(QuickSaveReader reader)
        {
            if (reader.TryRead(GOLD, out int gold))
                SetGold(gold);

            if (reader.TryRead(LOST_GOLD, out int lostGold)
                && reader.TryRead(LOST_GOLD_SCENE, out string lostGoldScene)
                && reader.TryRead(LOST_GOLD_POSITION, out Vector3 lostGoldPosition))
            {
                SetLostGold(lostGold, lostGoldScene, lostGoldPosition);
            }
        }

        public void SaveData(QuickSaveWriter writer)
        {
            writer.Write(GOLD, Gold);
            writer.Write(LOST_GOLD, LostGold);
            writer.Write(LOST_GOLD_SCENE, LostGoldScene);
            writer.Write(LOST_GOLD_POSITION, LostGoldPosition);
        }
    }
}
