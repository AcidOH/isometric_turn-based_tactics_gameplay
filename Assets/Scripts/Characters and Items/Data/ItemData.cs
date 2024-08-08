using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ItemData", menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemTypes itemType;

    [TextArea(10, 5)]
    public string itemDescription;

    public StatModifier[] statModifiers;

    public float range;
    public float hit;
    public float might;
    public float weight;
    public float defence;
    public float resistance;
    public float cost;

    public enum ItemTypes
    {
        MeleeWeapon,
        RangeWeapon,
        MagicWeapon,
        NonWeapon,
    }
}
