using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityData : ScriptableObject
{
    //base entity data structure that all entities inherit from.
    //contains all info that all entities are required to have regardless of their type.
    //(Entity contains all methods, rather than info)

    public string entityName;
    public int entityAge;

    //can declare related enums here, such as for levels
}
