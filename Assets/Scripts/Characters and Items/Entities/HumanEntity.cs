using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanEntity : Entity
{
    [SerializeField]
    private ProfessionData _professionData;

    [SerializeField]
    private HumanData _humanData;
    
    //description elements
    private string _professionName;
    private string _professionDescription;

    //get + set
    public ProfessionData ProfessionData { get { return _professionData; } }
    public HumanData HumanData { get { return _humanData; } }

    public void InitializeHumanEntity(string name, int age, EntityData entityData, ProfessionData professionData)
    { 
        _name = name;
        _age = age;
        _baseEntityData = entityData;
        _professionData = professionData;
        _professionName = professionData.professionName;
        _professionDescription = professionData.professionDescription;
    }

    //there are some additional scripts that can be created for displaying and editing info
}
