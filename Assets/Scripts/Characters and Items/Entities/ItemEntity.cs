using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : Entity
{
    [SerializeField]
    private ItemData _itemData;

    //description elements
    private string _itemName;
    private string _itemDescription;

    //get + set
    public ItemData ItemData { get { return _itemData; } }

    public void InitializeItemEntity(string name, int age, EntityData entityData, ItemData itemData)
    {
        _name = name;
        _age = age;
        _baseEntityData = entityData;
        _itemData = itemData;
        _itemName = itemData.itemName;
        _itemDescription = itemData.itemDescription;
    }

    //there are some additional scripts that can be created for displaying and editing info

    public void Equip(HumanEntity humanEntity)
    {
        //might
        if (ItemData.itemType == ItemData.ItemTypes.MeleeWeapon || ItemData.itemType == ItemData.ItemTypes.RangeWeapon)
        {
            humanEntity.HumanData.attackNonMagic.AddModifier(new StatModifier(ItemData.might.Value, StatModType.Flat, this));
        }
        else if (ItemData.itemType == ItemData.ItemTypes.MagicWeapon)
        {
            humanEntity.HumanData.attackMagic.AddModifier(new StatModifier(ItemData.might.Value, StatModType.Flat, this));
        }

        //hit
        if (ItemData.itemType == ItemData.ItemTypes.MeleeWeapon || ItemData.itemType == ItemData.ItemTypes.RangeWeapon)
        {
            humanEntity.HumanData.hitRateNonMagic.AddModifier(new StatModifier(ItemData.hit.Value, StatModType.Flat, this));
        }
        else if (ItemData.itemType == ItemData.ItemTypes.MagicWeapon)
        {
            humanEntity.HumanData.hitRateMagic.AddModifier(new StatModifier(ItemData.hit.Value, StatModType.Flat, this));
        }

        //defence
        humanEntity.HumanData.protect.AddModifier(new StatModifier(ItemData.defence.Value, StatModType.Flat, this));

        //resistance
        humanEntity.HumanData.resilience.AddModifier(new StatModifier(ItemData.resistance.Value, StatModType.Flat, this));

        //weight
        float _strength = humanEntity.HumanData.strength.Value; //maybe you want this to be BaseValue?? je ne sais pas
        float _weightReduction = Mathf.Round(_strength / 5);
        float _weight = ItemData.weight.Value > _weightReduction? ItemData.weight.Value - _weightReduction : 0;

        humanEntity.HumanData.attackSpeed.AddModifier(new StatModifier(_weight, StatModType.Flat, this));
        humanEntity.HumanData.avoidanceRate.AddModifier(new StatModifier(_weight, StatModType.Flat, this));
    }

    public void Unequip(HumanEntity humanEntity)
    { 
        humanEntity.HumanData.attackNonMagic.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.attackMagic.RemoveAllModifiersFromSource(this);

        humanEntity.HumanData.hitRateNonMagic.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.hitRateMagic.RemoveAllModifiersFromSource(this);

        humanEntity.HumanData.protect.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.resilience.RemoveAllModifiersFromSource(this);

        humanEntity.HumanData.attackSpeed.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.avoidanceRate.RemoveAllModifiersFromSource(this);
    }
}
