namespace AF.Tests
{
    using NUnit.Framework;
    using UnityEngine;

    public class ItemList_Tests
    {
        private ItemList itemList;
        private EquipmentDatabase equipmentDatabase;

        WeaponInstance weapon;

        [SetUp]
        public void SetUp()
        {
            var scriptableWeapon = ScriptableObject.CreateInstance<Weapon>();
            weapon = new WeaponInstance("", scriptableWeapon, 0, new());
            itemList = new GameObject().AddComponent<ItemList>();
            equipmentDatabase = ScriptableObject.CreateInstance<EquipmentDatabase>();
            itemList.equipmentDatabase = equipmentDatabase;
        }

        [Test]
        public void IsKeyItem_ShouldReturn_FalseForNonKeyItems_AndTrueForKeyItems()
        {
            // Assert
            Assert.IsFalse(itemList.IsKeyItem(ScriptableObject.CreateInstance<Armor>()));
            Assert.IsFalse(itemList.IsKeyItem(ScriptableObject.CreateInstance<Weapon>()));
            Assert.IsFalse(itemList.IsKeyItem(ScriptableObject.CreateInstance<Shield>()));

            Assert.IsTrue(itemList.IsKeyItem(ScriptableObject.CreateInstance<Item>()));
        }

        [Test]
        public void ShouldShowItem_ItemIsNotOfTypeT_ReturnsFalse()
        {
            bool result = itemList.ShouldShowItem<Shield>(weapon, 0, false);
            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldShowItem_ItemIsNotKeyItem_ReturnsFalse()
        {
            bool result = itemList.ShouldShowItem<Weapon>(weapon, 0, true);
            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldShowItem_ItemIsKeyItemAndSlotDoesNotMatch_ReturnsFalse()
        {
            bool result = itemList.ShouldShowItem<Weapon>(weapon, 0, true);
            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldShowItem_ItemIsNotKeyItemAndSlotMatches_ReturnsTrue()
        {
            equipmentDatabase.EquipWeapon(weapon, 0);

            bool result = itemList.ShouldShowItem<Weapon>(weapon, 0, false);

            equipmentDatabase.UnequipWeapon(0);

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldNotShowEquippedWeapon_SlotDoesNotMatch()
        {
            equipmentDatabase.EquipWeapon(weapon, 1);

            bool result1 = itemList.ShouldShowItem<Weapon>(weapon, 0, false);
            bool result2 = itemList.ShouldShowItem<Weapon>(weapon, 1, false);
            bool result3 = itemList.ShouldShowItem<Weapon>(weapon, 2, false);

            equipmentDatabase.UnequipWeapon(1);

            Assert.IsFalse(result1);
            Assert.IsTrue(result2);
            Assert.IsFalse(result3);
        }

        [Test]
        public void ShouldShowItem_ConsumableAndSlotDoesNotMatch_ReturnsFalse()
        {
            bool result = itemList.ShouldShowItem<Consumable>(new ConsumableInstance("", null), 0, true);
            Assert.IsFalse(result);
        }
    }
}
