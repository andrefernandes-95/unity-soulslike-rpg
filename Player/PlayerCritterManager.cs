namespace AF
{
    using System.Collections;
    using AF.Characters;
    using UnityEngine;
    using System.Linq;
    public class PlayerCritterManager : MonoBehaviour
    {
        public Combatant playerCombatant;
        public GameObject disppearFx;

        public PlayerManager playerManager;

        public void SpawnCritter(CharacterManager character)
        {
            CharacterManager instance = Instantiate(character, playerManager.transform.position + playerManager.transform.forward, Quaternion.identity);
            instance.combatant.friendlies = new Combatant[] { playerCombatant };

            LockOnRef lockOnRef = instance.GetComponentInChildren<LockOnRef>();
            if (lockOnRef != null)
            {
                Destroy(lockOnRef.gameObject.GetComponent<LockOnRef>());
            }

            CharacterManager target = playerManager.lockOnManager.nearestLockOnTarget?.characterManager;
            if (target == null)
            {
                // Get all characters in the scene
                var allCharacters = FindObjectsByType<CharacterManager>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

                // Filter characters by tag "Enemy"
                var enemyCharacters = allCharacters.Where(character => character.CompareTag("Enemy"));

                // Exclude the character that is the same as this character
                var filteredCharacters = enemyCharacters.Where(_character => !_character.combatant.friendlies.Contains(playerCombatant));

                // Sort characters by distance to the player
                var closestCharacter = filteredCharacters.OrderBy(
                    character => Vector3.Distance(playerManager.transform.position, character.transform.position))?.FirstOrDefault();

                if (closestCharacter != null)
                {
                    target = closestCharacter;
                }
            }

            instance.targetManager.SetTarget(target);

            StartCoroutine(RemoveCritter(instance));
        }

        IEnumerator RemoveCritter(CharacterManager instance)
        {
            yield return new WaitForSeconds(30f);

            Instantiate(disppearFx);

            Destroy(instance.gameObject);
        }

    }
}
