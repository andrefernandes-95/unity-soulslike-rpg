using System.Collections;
using UnityEngine;

namespace AF
{
    public class CharacterPoise : CharacterAbstractPoise
    {
        [HideInInspector] public bool hasHyperArmor = false;
        bool ignorePoiseDamage = false;
        readonly float recoverPoiseCooldown = 3f;

        Coroutine ResetIgnorePoiseDamageCoroutine;

        public override void ResetStates()
        {
            hasHyperArmor = false;
        }

        public override bool CanCallPoiseDamagedEvent()
        {
            if (hasHyperArmor)
            {
                return false;
            }

            return true;
        }

        public override int GetMaxPoiseHits()
        {
            return (characterManager as CharacterManager)?.combatant?.poise ?? 1;
        }

        public override bool TakePoiseDamage(int poiseDamage)
        {
            bool result = ignorePoiseDamage == false && base.TakePoiseDamage(poiseDamage);

            if (result)
            {
                currentPoiseHitCount = 0;

                if (CanCallPoiseDamagedEvent())
                {
                    onPoiseDamagedEvent?.Invoke();
                    characterManager.PlayBusyAnimationWithRootMotion(takingDamageAnimationClip);
                }

                characterManager.health.PlayPostureHit();

                ignorePoiseDamage = true;

                if (ResetIgnorePoiseDamageCoroutine != null)
                {
                    StopCoroutine(ResetIgnorePoiseDamageCoroutine);
                }

                ResetIgnorePoiseDamageCoroutine = StartCoroutine(ResetIgnorePoiseDamage_Coroutine());
            }

            return result;
        }

        IEnumerator ResetIgnorePoiseDamage_Coroutine()
        {
            yield return new WaitForSeconds(recoverPoiseCooldown);
            ignorePoiseDamage = false;
        }
    }
}
