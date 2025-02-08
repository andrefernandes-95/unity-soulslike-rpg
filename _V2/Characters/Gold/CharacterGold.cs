namespace AFV2
{
    using UnityEngine;

    public class CharacterGold : MonoBehaviour
    {
        [SerializeField] private float gold = 0;

        public float Gold
        {
            get => gold;
            private set => gold = Mathf.Max(0, value);
        }

        public void SetGold(int value) => Gold = value;
        public void IncreaseGold(int value) => Gold += value;
        public void DecreaseGold(int value) => Gold -= value;
    }
}
