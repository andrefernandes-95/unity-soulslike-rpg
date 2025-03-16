namespace AFV2
{
    using System.Threading.Tasks;
    using TMPro;
    using UnityEngine;

    public class AlertManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TextMeshProUGUI alertMessage;

        bool isDisplaying = false;

        void Awake()
        {
            canvasGroup.alpha = 0;
        }

        public async void DisplayAlert(string message, float duration = 2)
        {
            if (isDisplaying)
            {
                alertMessage.text = message;
                return;
            }

            isDisplaying = true;
            canvasGroup.alpha = 1;
            alertMessage.text = message;
            await Task.Delay((int)duration * 1000);
            canvasGroup.alpha = 0;
            isDisplaying = false;
        }
    }
}
