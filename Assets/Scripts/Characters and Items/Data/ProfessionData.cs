using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ProfessionData", menuName = "Data/ProfessionData")]
public class ProfessionData : ScriptableObject
{
    public string professionName;
    public ProfessionTypes professionType;

    [TextArea(10,5)]
    public string professionDescription;

    public StatModifier[] statModifiers;

    public enum ProfessionTypes
    { 
        noClass,
    }
}
