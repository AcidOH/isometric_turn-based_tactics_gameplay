using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class HumanData : ScriptableObject
{
    //borrowing fe3h stat system for the timebeing -- will change it though
    public CharacterStat healthPoints;
    public CharacterStat movement;
    public CharacterStat strength;
    public CharacterStat magic;
    public CharacterStat dexterity;
    public CharacterStat speed;
    public CharacterStat luck;
    public CharacterStat defence;
    public CharacterStat resistance;
    public CharacterStat charm;
    public CharacterStat energy;

    //anything that comes from an item or the target is added as a modifier
    public CharacterStat attackNonMagic { get { return strength; } } //add item's might
    public CharacterStat attackMagic { get { return magic; } } //add item's might
    public CharacterStat attackSpeed { get { return speed; } } //subtract item's weight
    public CharacterStat avoidanceRate { get { return speed; } } //subtact item's weight
    public CharacterStat criticalHitRate 
    { 
        get 
        { 
            float _value = (dexterity.BaseValue + luck.BaseValue)/2; 
            criticalHitRate.BaseValue = _value;
            return criticalHitRate;
        } 
    }
    public CharacterStat criticalHitAvoidanceRate { get { return luck; } }
    public CharacterStat hitRateNonMagic { get { return dexterity; } } //add item's hit (b/w 65-90), distance to target impacts this as well
    public CharacterStat hitRateMagic //spell hit is typically b/w 65-100
    {
        get
        {
            float _value = (dexterity.BaseValue + luck.BaseValue) / 2;
            hitRateMagic.BaseValue = _value;
            return hitRateMagic;
        }
    }
    public CharacterStat protect { get {  return defence; } } //add item's defence, used for physical attacks
    //range is a propety of items, is the item equivalent of movement
    public CharacterStat resilience { get { return resistance; } } //add item's resistance, used for magic attacks
    //weight is a property of items: for every 5 str, -1 to wt penaly
}
