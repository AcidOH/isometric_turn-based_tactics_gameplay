using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanEntity : Entity
{
    [SerializeField]
    private HumanData _humanData;

    //[SerializeField]
    //private Sprite _profileImage;

    //[SerializeField]
    //private Sprite _fullCharacterSprite;

    public HumanData HumanData { get { return _humanData; } }
    //public Sprite ProfileImage { get { return _profileImage; } }
    //public Sprite FullCharacterSprite { get { return _profileImage; } }

    public void InitializeHumanEntity(string name, int age, EntityData entityData)
    { 
        _name = name;
        _age = age;
        _baseEntityData = entityData;
    }

    //there are some additional scripts that can be created for displaying and editing info
}
