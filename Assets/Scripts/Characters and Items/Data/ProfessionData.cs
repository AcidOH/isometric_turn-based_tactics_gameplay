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

    public float healthPoints;
    public float movement;
    public float strength;
    public float magic;
    public float dexterity;
    public float speed;
    public float luck;
    public float defence;
    public float resistance;
    public float charm;
    public float energy;

    public enum ProfessionTypes
    { 
        None,
        Beginner,
        Intermediate,
        Advanced,
        Master,
    }
}
