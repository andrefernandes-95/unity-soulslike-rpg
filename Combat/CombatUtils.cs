namespace AF
{
    using UnityEngine;

    public static class CombatUtils
    {

        public static bool IsTargetBehind(CharacterManager characterManager)
        {
            if (characterManager.targetManager == null || characterManager.targetManager.currentTarget == null)
            {
                return false;
            }

            // Calculate vector from enemy to player
            Vector3 toPlayer = characterManager.targetManager.currentTarget.transform.position - characterManager.transform.position;

            // Calculate angle between enemy's forward direction and vector to player
            float angle = Vector3.Angle(characterManager.transform.forward, toPlayer);

            return angle > 90f;
        }
    }
}