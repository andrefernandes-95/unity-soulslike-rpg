namespace AF
{
    using UnityEngine;
    public class ApplyWeaponBuff : MonoBehaviour
    {
        public WeaponBuffType weaponBuffName;

        PlayerManager playerManager;

        public Transform grindingWheel;


        public virtual void Apply()
        {
            GetPlayerManager().playerWeaponsManager.ApplyWeaponBuffToWeapon(weaponBuffName);
        }

        PlayerManager GetPlayerManager()
        {
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }

            return playerManager;
        }
    }
}