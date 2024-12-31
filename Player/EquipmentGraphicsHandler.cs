namespace AF
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using AF.Stats;
    using TigerForge;
    using AF.Events;
    using AF.Health;
    using UnityEngine.Localization.Settings;

    public class EquipmentGraphicsHandler : MonoBehaviour
    {

        [Header("Components")]
        public StatsBonusController statsBonusController;
        public PlayerManager playerManager;

        readonly List<string> _helmetNakedParts = new()
        {
            "HairContainer",
            "HeadContainer",
            "EyebrowContainer",
            "BeardContainer",
        };

        readonly List<string> _armorNakedParts = new()
        {
            "TorsoContainer",
            "UpperRightArmContainer",
            "UpperLeftArmContainer",
        };

        readonly List<string> _gauntletsNakedParts = new()
        {
            "LeftLowerArmContainer",
            "RightLowerArmContainer",
            "LeftHandContainer",
            "RightHandContainer"
        };

        readonly List<string> _legwearNakedParts = new()
        {
            "HipContainer",
            "LeftLegContainer",
            "RightLegContainer"
        };

        [Header("UI Systems")]
        public NotificationManager notificationManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Transform References")]
        public Transform playerEquipmentRoot;

        private void Awake()
        {
            playerManager.damageReceiver.onDamageEvent += OnDamageEvent;
        }

        void Start()
        {
            InitializeEquipment();
        }

        public void InitializeEquipment()
        {
            DrawCharacterGraphics();

            if (equipmentDatabase.helmet.Exists())
            {
                EquipHelmet(equipmentDatabase.helmet);
            }

            if (equipmentDatabase.armor.Exists())
            {
                EquipArmor(equipmentDatabase.armor);
            }

            if (equipmentDatabase.legwear.Exists())
            {
                EquipLegwear(equipmentDatabase.legwear);
            }

            if (equipmentDatabase.gauntlet.Exists())
            {
                EquipGauntlet(equipmentDatabase.gauntlet);
            }

            for (int i = 0; i < equipmentDatabase.accessories.Length; i++)
            {
                EquipAccessory(equipmentDatabase.accessories[i], i);
            }
        }

        #region Helmet
        public void EquipHelmet(HelmetInstance helmetToEquip)
        {
            if (helmetToEquip.IsEmpty())
            {
                return;
            }

            UnequipHelmet();

            if (helmetToEquip != equipmentDatabase.helmet)
            {
                equipmentDatabase.EquipHelmet(helmetToEquip);
            }

            DrawCharacterGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }

        public void UnequipHelmet()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                if (equipmentDatabase.helmet.Exists())
                {
                    if (equipmentDatabase.helmet.GetItem()?.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (equipmentDatabase.helmet.GetItem()?.graphicNamesToHide?.Contains(t.gameObject.name) ?? false)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            equipmentDatabase.UnequipHelmet();

            DrawCharacterGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }
        #endregion

        #region Armor
        public void EquipArmor(ArmorInstance armorToEquip)
        {
            if (armorToEquip.IsEmpty())
            {
                return;
            }

            UnequipArmor();

            if (armorToEquip != equipmentDatabase.armor)
            {
                equipmentDatabase.EquipArmor(armorToEquip);
            }

            DrawCharacterGraphics();

            statsBonusController.RecalculateEquipmentBonus();

            EventManager.EmitEvent(EventMessages.ON_BODY_TYPE_CHANGED);
        }

        public void UnequipArmor()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                if (equipmentDatabase.armor.Exists())
                {
                    if (equipmentDatabase.armor.GetItem()?.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (equipmentDatabase.armor.GetItem()?.graphicNamesToHide?.Contains(t.gameObject.name) ?? false)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            equipmentDatabase.UnequipArmor();

            DrawCharacterGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }
        #endregion

        #region Gauntlets
        public void EquipGauntlet(GauntletInstance gauntletToEquip)
        {
            if (gauntletToEquip.IsEmpty())
            {
                return;
            }

            UnequipGauntlet();

            if (gauntletToEquip != equipmentDatabase.gauntlet)
            {
                equipmentDatabase.EquipGauntlet(gauntletToEquip);
            }

            DrawCharacterGraphics();
            statsBonusController.RecalculateEquipmentBonus();
        }

        public void UnequipGauntlet()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                if (equipmentDatabase.gauntlet.Exists())
                {
                    if (equipmentDatabase.gauntlet.GetItem()?.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (equipmentDatabase.gauntlet.GetItem()?.graphicNamesToHide?.Contains(t.gameObject.name) ?? false)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            equipmentDatabase.UnequipGauntlet();
            DrawCharacterGraphics();
            statsBonusController.RecalculateEquipmentBonus();
        }
        #endregion

        #region Legwear
        public void EquipLegwear(LegwearInstance legwearToEquip)
        {
            if (legwearToEquip.IsEmpty())
            {
                return;
            }

            UnequipLegwear();

            if (legwearToEquip != equipmentDatabase.legwear)
            {
                equipmentDatabase.EquipLegwear(legwearToEquip);
            }

            DrawCharacterGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }

        public void UnequipLegwear()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                if (equipmentDatabase.legwear.Exists())
                {
                    if (equipmentDatabase.legwear.GetItem()?.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (equipmentDatabase.legwear.GetItem()?.graphicNamesToHide?.Contains(t.gameObject.name) ?? false)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            equipmentDatabase.UnequipLegwear();
            DrawCharacterGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }
        #endregion

        #region Accessories


        public void EquipAccessory(AccessoryInstance accessoryToEquip, int slotIndex)
        {
            if (accessoryToEquip.IsEmpty())
            {
                return;
            }

            equipmentDatabase.EquipAccessory(accessoryToEquip, slotIndex);

            statsBonusController.RecalculateEquipmentBonus();
        }
        #endregion

        public void UnequipAccessory(int slotIndex)
        {
            equipmentDatabase.UnequipAccessory(slotIndex);

            statsBonusController.RecalculateEquipmentBonus();
        }

        void DrawCharacterGraphics()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                // HELMET
                var helmet = equipmentDatabase.helmet;

                if (helmet.IsEmpty())
                {
                    if (_helmetNakedParts.IndexOf(t.gameObject.name) != -1)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (helmet.GetItem()?.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(true);
                    }

                    foreach (string graphicNameToHide in helmet.GetItem()?.graphicNamesToHide)
                    {
                        if (t.gameObject.name == graphicNameToHide)
                        {
                            t.gameObject.SetActive(false);
                        }
                    }
                }

                // ARMOR
                var chest = equipmentDatabase.armor;
                if (chest.IsEmpty())
                {
                    if (_armorNakedParts.IndexOf(t.gameObject.name) != -1)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (chest.GetItem()?.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(true);
                    }

                    foreach (string graphicNameToHide in chest.GetItem()?.graphicNamesToHide)
                    {
                        if (t.gameObject.name == graphicNameToHide)
                        {
                            t.gameObject.SetActive(false);
                        }
                    }
                }

                // GAUNTLETS
                var gauntlets = equipmentDatabase.gauntlet;
                if (gauntlets.IsEmpty())
                {
                    if (_gauntletsNakedParts.IndexOf(t.gameObject.name) != -1)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (gauntlets.GetItem()?.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(true);
                    }

                    foreach (string graphicNameToHide in gauntlets.GetItem()?.graphicNamesToHide)
                    {
                        if (t.gameObject.name == graphicNameToHide)
                        {
                            t.gameObject.SetActive(false);
                        }
                    }
                }

                // LEGWEAR
                var legwear = equipmentDatabase.legwear;
                if (legwear.IsEmpty())
                {
                    if (_legwearNakedParts.IndexOf(t.gameObject.name) != -1)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (legwear.GetItem()?.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(true);
                    }

                    foreach (string graphicNameToHide in legwear.GetItem()?.graphicNamesToHide)
                    {
                        if (t.gameObject.name == graphicNameToHide)
                        {
                            t.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        public float GetHeavyWeightThreshold()
        {

            return 0.135f + GetStrengthWeightLoadBonus();
        }

        public float GetMidWeightThreshold()
        {

            return 0.05f + GetStrengthWeightLoadBonus();
        }

        float GetStrengthWeightLoadBonus()
        {
            float bonus = playerStatsDatabase.strength + statsBonusController.strengthBonus;

            bonus *= 0.0025f;

            if (bonus > 0f)
            {
                return bonus;
            }

            return 0f;
        }

        public bool IsLightWeight()
        {
            return statsBonusController.weightPenalty <= GetMidWeightThreshold();
        }

        public bool IsMidWeight()
        {
            return statsBonusController.weightPenalty < GetHeavyWeightThreshold() && statsBonusController.weightPenalty > GetMidWeightThreshold();
        }

        public bool IsHeavyWeight()
        {
            return statsBonusController.weightPenalty >= GetHeavyWeightThreshold();
        }

        public bool IsLightWeightForGivenValue(float givenWeightPenalty)
        {
            return givenWeightPenalty <= GetMidWeightThreshold();
        }

        public bool IsMidWeightForGivenValue(float givenWeightPenalty)
        {
            return givenWeightPenalty < GetHeavyWeightThreshold() && givenWeightPenalty > GetMidWeightThreshold();
        }

        public bool IsHeavyWeightForGivenValue(float givenWeightPenalty)
        {
            return givenWeightPenalty >= GetHeavyWeightThreshold();
        }

        public float GetEquipLoad()
        {
            return statsBonusController.weightPenalty;
        }

        public string GetWeightLoadLabel(float givenWeightLoad)
        {
            if (IsLightWeightForGivenValue(givenWeightLoad))
            {
                return LocalizationSettings.SelectedLocale.Identifier.Code == "en" ? "Light Load" : "Leve";
            }
            if (IsMidWeightForGivenValue(givenWeightLoad))
            {
                return LocalizationSettings.SelectedLocale.Identifier.Code == "en" ? "Medium Load" : "Médio";
            }
            if (IsHeavyWeightForGivenValue(givenWeightLoad))
            {
                return LocalizationSettings.SelectedLocale.Identifier.Code == "en" ? "Heavy Load" : "Pesado";
            }

            return "";
        }


        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage damage)
        {
            if (receiver is PlayerManager)
            {

                if (playerManager.equipmentDatabase.helmet.Exists() && (playerManager.equipmentDatabase.helmet.GetItem()?.canDamageEnemiesUponAttack ?? false))
                {
                    playerManager.equipmentDatabase.helmet.GetItem()?.AttackEnemy(attacker as CharacterManager);
                }
                if (playerManager.equipmentDatabase.armor.Exists() && (playerManager.equipmentDatabase.armor.GetItem()?.canDamageEnemiesUponAttack ?? false))
                {
                    playerManager.equipmentDatabase.armor.GetItem()?.AttackEnemy(attacker as CharacterManager);
                }
                if (playerManager.equipmentDatabase.gauntlet.Exists() && (playerManager.equipmentDatabase.gauntlet.GetItem()?.canDamageEnemiesUponAttack ?? false))
                {
                    playerManager.equipmentDatabase.gauntlet.GetItem()?.AttackEnemy(attacker as CharacterManager);
                }
                if (playerManager.equipmentDatabase.legwear.Exists() && (playerManager.equipmentDatabase.legwear.GetItem()?.canDamageEnemiesUponAttack ?? false))
                {
                    playerManager.equipmentDatabase.legwear.GetItem()?.AttackEnemy(attacker as CharacterManager);
                }
            }

            return damage;
        }
    }
}
