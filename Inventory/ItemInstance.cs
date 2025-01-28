namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Inventory;
    using UnityEngine;

    [System.Serializable]
    public class ItemInstance
    {
        [SerializeField]
        protected string id;
        [SerializeField]
        protected Item item;

        // Weapons
        public int level;
        public List<string> attachedGemstoneIds = new();

        // Consumables
        public bool wasUsed = false;
        public string createdItemThumbnailName;
        public bool isUserCreatedItem;

        public void Clear()
        {
            id = null;
            item = null;
        }

        public bool Exists() => id != null && item != null;
        public bool IsEmpty() => !Exists();

        public bool HasItem(Item item) => Exists() && this.item == item;

        public Item GetItem() => this.item;
        public string GetId() => Exists() ? this.id : "";

        public bool IsId(string id) => Exists() && this.id == id;

        public ItemInstance(string id, Item item)
        {
            this.id = id;
            this.item = item;
        }

        public ItemInstance Clone() => new ItemInstance(this.id, this.item);

        public void IncreaseLevel()
        {
            this.level++;
        }
    }

    [System.Serializable]
    public class WeaponInstance : ItemInstance
    {
        public WeaponInstance(string id, Item item) : base(id, item)
        {
            this.id = id;
            this.item = item;
        }

        public new Weapon GetItem() => base.GetItem() as Weapon;

        public new WeaponInstance Clone() => new(this.id, this.item);

        public bool IsGemstoneEquipped(GemstoneInstance gemstoneInstance)
            => attachedGemstoneIds.Any(
                attachedGemstoneInstance => attachedGemstoneInstance == gemstoneInstance.GetId());

        public void AttachGemstone(GemstoneInstance gemstoneInstance)
        {
            this.attachedGemstoneIds.Add(gemstoneInstance.GetId());
        }

        public void RemoveGemstone(GemstoneInstance gemstoneInstance)
        {
            if (!this.IsGemstoneEquipped(gemstoneInstance))
            {
                return;
            }

            this.attachedGemstoneIds.Remove(gemstoneInstance.GetId());
        }

        public Gemstone[] GetAttachedGemstones(InventoryDatabase inventoryDatabase) =>
            inventoryDatabase
                .FilterByType<GemstoneInstance>()
                .Where(gemstoneInstance => attachedGemstoneIds.Contains(gemstoneInstance.GetId()))
                .Select(gemstoneInstance => gemstoneInstance.GetItem())
                .ToArray();
    }

    [System.Serializable]
    public class ShieldInstance : ItemInstance
    {
        public ShieldInstance(string id, Item item) : base(id, item)
        {
        }

        public new Shield GetItem() => base.GetItem() as Shield;
        public new ShieldInstance Clone() => new(this.id, this.item);
    }

    [System.Serializable]
    public class ArrowInstance : ItemInstance
    {
        public ArrowInstance(string id, Item item) : base(id, item)
        {
        }

        public new Arrow GetItem() => base.GetItem() as Arrow;
        public new ArrowInstance Clone() => new(this.id, this.item);
    }

    [System.Serializable]
    public class SpellInstance : ItemInstance
    {
        public SpellInstance(string id, Item item) : base(id, item)
        {
        }

        public new Spell GetItem() => base.GetItem() as Spell;
        public new SpellInstance Clone() => new(this.id, this.item);
    }

    [System.Serializable]
    public class ConsumableInstance : ItemInstance
    {

        public ConsumableInstance(string id, Item item) : base(id, item)
        {
        }

        public new Consumable GetItem() => base.GetItem() as Consumable;
        public new ConsumableInstance Clone() => new(this.id, this.item);

        public SerializedUserCreatedItem ConvertToSerializedUserCreatedItem()
        {
            return new()
            {
                id = id,
                itemName = item.GetName(),
                itemThumbnailName = this.createdItemThumbnailName,
                effects = GetItem().statusEffectsWhenConsumed.Select(statusEffect => statusEffect.name).ToArray(),
                value = (int)item.value,
            };
        }
    }

    [System.Serializable]
    public class ArmorBaseInstance : ItemInstance
    {
        public ArmorBaseInstance(string id, Item item) : base(id, item)
        {
        }

        public new ArmorBase GetItem() => base.GetItem() as ArmorBase;
        public new ArmorBaseInstance Clone() => new(this.id, this.item);
    }

    [System.Serializable]
    public class HelmetInstance : ArmorBaseInstance
    {
        public HelmetInstance(string id, Item item) : base(id, item)
        {
        }

        public new Helmet GetItem() => base.GetItem() as Helmet;
        public new HelmetInstance Clone() => new(this.id, this.item);
    }

    [System.Serializable]
    public class ArmorInstance : ArmorBaseInstance
    {
        public ArmorInstance(string id, Item item) : base(id, item)
        {
        }

        public new Armor GetItem() => base.GetItem() as Armor;
        public new ArmorInstance Clone() => new(this.id, this.item);
    }

    [System.Serializable]
    public class GauntletInstance : ArmorBaseInstance
    {
        public GauntletInstance(string id, Item item) : base(id, item)
        {
        }

        public new Gauntlet GetItem() => base.GetItem() as Gauntlet;
        public new GauntletInstance Clone() => new(this.id, this.item);
    }


    [System.Serializable]
    public class LegwearInstance : ArmorBaseInstance
    {
        public LegwearInstance(string id, Item item) : base(id, item)
        {
        }

        public new Legwear GetItem() => base.GetItem() as Legwear;
        public new LegwearInstance Clone() => new(this.id, this.item);
    }

    [System.Serializable]
    public class AccessoryInstance : ArmorBaseInstance
    {
        public AccessoryInstance(string id, Item item) : base(id, item)
        {
        }

        public new Accessory GetItem() => base.GetItem() as Accessory;
        public new AccessoryInstance Clone() => new(this.id, this.item);
    }

    [System.Serializable]
    public class CraftingMaterialInstance : ItemInstance
    {
        public CraftingMaterialInstance(string id, Item item) : base(id, item)
        {
        }

        public new CraftingMaterial GetItem() => base.GetItem() as CraftingMaterial;
        public new CraftingMaterialInstance Clone() => new(this.id, this.item);
    }

    [System.Serializable]
    public class UpgradeMaterialInstance : CraftingMaterialInstance
    {
        public UpgradeMaterialInstance(string id, Item item) : base(id, item)
        {
        }

        public new UpgradeMaterial GetItem() => base.GetItem() as UpgradeMaterial;
        public new UpgradeMaterialInstance Clone() => new(this.id, this.item);
    }

    [System.Serializable]
    public class GemstoneInstance : ItemInstance
    {
        public GemstoneInstance(string id, Item item) : base(id, item)
        {
        }

        public new Gemstone GetItem() => base.GetItem() as Gemstone;
        public new GemstoneInstance Clone() => new(this.id, this.item);

    }
}
