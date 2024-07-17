using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public ItemTypes itemType;

    [TextArea(10, 5)]
    public string itemDescription;

    public StatModifier[] statModifiers;

    public CharacterStat range;
    public CharacterStat hit;
    public CharacterStat might;
    public CharacterStat weight;
    public CharacterStat defence;
    public CharacterStat resistance;
    public CharacterStat cost;

    public enum ItemTypes
    {
        MeleeWeapon,
        RangeWeapon,
        MagicWeapon,
        NonWeapon,
    }
}
