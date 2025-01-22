namespace AF
{
    using UnityEngine;

    public class CharacterHUD : MonoBehaviour
    {
        public GameObject characterName;

        public GameObject characterHealthbar;

        public void ShowHealthbar()
        {
            characterHealthbar?.SetActive(true);
        }
    }
}
