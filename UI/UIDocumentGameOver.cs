using System.Collections;
using AF.Events;
using AF.Music;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentGameOver : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;
        public Soundbank soundbank;
        public PlayerManager playerManager;
        public SaveManager saveManager;
        public UIDocumentPlayerGold uIDocumentPlayerGold;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("Settings")]
        float GAME_OVER_DURATION = 3.5f;

        [Header("You Died Texts")]
        public LocalizedString[] youDiedText;

        [Header("Edge Cases")]
        [SerializeField]
        private Moment gameOverOverride;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisplayGameOver()
        {
            if (gameOverOverride != null)
            {
                gameOverOverride.Trigger();
                gameOverOverride = null;
                return;
            }

            playerManager.uIDocumentPlayerHUDV2.HideHUD();
            this.gameObject.SetActive(true);
            StartCoroutine(GameOver_Coroutine());
        }

        IEnumerator GameOver_Coroutine()
        {
            Label youDiedLabel = GetComponent<UIDocument>().rootVisualElement.Q<Label>("YouDiedText");
            youDiedLabel.text = youDiedText[Random.Range(0, youDiedText.Length)].GetLocalizedString();
            UIUtils.PlayPopAnimation(youDiedLabel);

            bgmManager.StopMusic();
            soundbank.PlaySound(soundbank.gameOverFanfare);

            playerManager.playerComponentManager.DisableCharacterController();
            playerManager.playerComponentManager.DisableComponents();

            if (playerStatsDatabase.HasLostGoldToRecover())
            {
                playerStatsDatabase.ClearLostGold();
            }
            else
            {
                playerStatsDatabase.SetLostGold(playerManager.thirdPersonController.lastGroundedPosition);
            }

            uIDocumentPlayerGold.LoseGold(playerStatsDatabase.gold);

            yield return new WaitForSeconds(GAME_OVER_DURATION);

            saveManager.LoadLastSavedGame(true);
        }

        public void SetGameOverOverride(Moment gameOverOverride)
        {
            this.gameOverOverride = gameOverOverride;
        }

        public void ClearGameOverOverride()
        {
            this.gameOverOverride = null;
        }
    }
}
