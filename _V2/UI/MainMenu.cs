namespace AFV2
{
    using UnityEngine;

    public class MainMenu : UIScreen
    {
        [SerializeField] UIScreen[] childScreens;

        public override void Show(CharacterApi characterApi)
        {
            base.Show(characterApi);

            if (childScreens != null && childScreens.Length > 0)
            {
                childScreens[0].Show(characterApi);
            }
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
