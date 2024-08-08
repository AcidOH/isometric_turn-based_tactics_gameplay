using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessionEntity : Entity
{
    [SerializeField]
    private ProfessionData _professionData;

    //description elements
    private string _professionName;
    private string _professionDescription;

    //get + set
    public ProfessionData ProfessionData { get { return _professionData; } }

    public void InitializeItemEntity(string name, int age, EntityData entityData, ProfessionData professionData)
    {
        _name = name;
        _age = age;
        _baseEntityData = entityData;
        _professionData = professionData;
        _professionName = professionData.professionName;
        _professionDescription = professionData.professionDescription;
    }

    public void Equip(HumanEntity humanEntity)
    {
        humanEntity.HumanData.healthPoints.AddModifier(new StatModifier(ProfessionData.healthPoints, StatModType.Flat, this));
        humanEntity.HumanData.movement.AddModifier(new StatModifier(ProfessionData.movement, StatModType.Flat, this));
        humanEntity.HumanData.strength.AddModifier(new StatModifier(ProfessionData.strength, StatModType.Flat, this));
        humanEntity.HumanData.magic.AddModifier(new StatModifier(ProfessionData.magic, StatModType.Flat, this));
        humanEntity.HumanData.dexterity.AddModifier(new StatModifier(ProfessionData.dexterity, StatModType.Flat, this));
        humanEntity.HumanData.speed.AddModifier(new StatModifier(ProfessionData.speed, StatModType.Flat, this));
        humanEntity.HumanData.luck.AddModifier(new StatModifier(ProfessionData.luck, StatModType.Flat, this));
        humanEntity.HumanData.defence.AddModifier(new StatModifier(ProfessionData.defence, StatModType.Flat, this));
        humanEntity.HumanData.resistance.AddModifier(new StatModifier(ProfessionData.resistance, StatModType.Flat, this));
        humanEntity.HumanData.charm.AddModifier(new StatModifier(ProfessionData.charm, StatModType.Flat, this));
        humanEntity.HumanData.energy.AddModifier(new StatModifier(ProfessionData.energy, StatModType.Flat, this));   
    }

    public void Unequip(HumanEntity humanEntity)
    {
        humanEntity.HumanData.healthPoints.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.movement.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.strength.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.magic.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.dexterity.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.speed.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.luck.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.defence.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.resistance.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.charm.RemoveAllModifiersFromSource(this);
        humanEntity.HumanData.energy.RemoveAllModifiersFromSource(this);
    }
}
