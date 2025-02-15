namespace AFV2
{
    using UnityEngine;

    public class PlayerHUD : MonoBehaviour
    {
        bool isActive = false;
        public bool IsActive => isActive;

        public void Show()
        {
            isActive = true;
            UIUtils.FadeIn(this.gameObject);
        }

        public void Hide()
        {
            isActive = false;
            UIUtils.FadeOut(this.gameObject);
        }
    }
}
