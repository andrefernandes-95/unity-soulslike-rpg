namespace AFV2
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Weapon Type", menuName = "New Weapon Type", order = 0)]
    public class WeaponType : ScriptableObject
    {
        [SerializeField] string portugueseName;
        [SerializeField] string englishName;

        [HideInInspector]
        public string DisplayName
        {
            get
            {
                return Glossary.IsPortuguese() ? portugueseName : englishName;
            }
        }
    }
}
