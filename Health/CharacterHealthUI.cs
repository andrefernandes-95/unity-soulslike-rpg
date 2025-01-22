namespace AF
{
    using AF.Health;
    using UnityEngine;

    public class CharacterHealthUI : MonoBehaviour
    {
        public UnityEngine.UI.Slider healthSlider;
        public CharacterHealth characterHealth;

        private void Start()
        {
            InitializeHUD();

            if (characterHealth != null)
            {
                characterHealth.onHealthSettingsChanged.AddListener(UpdateUIDueToSettings);
            }
        }

        void HideRegularHUD()
        {
            characterHealth.characterManager.characterHUD.characterName.gameObject.SetActive(false);
            healthSlider.gameObject.SetActive(false);
        }

        void UpdateBossHUD()
        {
            characterHealth.characterManager.characterBossController.UpdateUI();
        }

        public void InitializeHUD()
        {
            if (characterHealth.characterManager.characterBossController.IsBoss())
            {
                UpdateBossHUD();
                HideRegularHUD();
                return;
            }

            if (healthSlider != null)
            {
                healthSlider.maxValue = characterHealth.GetMaxHealth() * 0.01f;
                healthSlider.value = characterHealth.GetCurrentHealth() * 0.01f;
            }
        }

        public void UpdateUI()
        {
            if (characterHealth.characterManager.characterBossController.IsBoss())
            {
                UpdateBossHUD();
                HideRegularHUD();
                return;
            }

            gameObject.SetActive(characterHealth.GetCurrentHealth() > 0);

            healthSlider.value = characterHealth.GetCurrentHealth() * 0.01f;
            healthSlider.maxValue = characterHealth.GetMaxHealth() * 0.01f;
        }

        void UpdateUIDueToSettings()
        {
            if (characterHealth.characterManager.characterBossController.IsBoss())
            {
                UpdateBossHUD();
                HideRegularHUD();
                return;
            }

            healthSlider.value = characterHealth.GetCurrentHealth() * 0.01f;
            healthSlider.maxValue = characterHealth.GetMaxHealth() * 0.01f;
        }
    }
}
