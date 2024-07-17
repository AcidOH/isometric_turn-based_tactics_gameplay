using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    protected string _name;

    [SerializeField]
    protected int _age;

    protected EntityData _baseEntityData;

    public EntityData BaseEntityData
    { get { return _baseEntityData; } }
    
    public string EntityName
    { get { return _name; } }

    public int Age
    { get { return _age; } }

    //any virtual methods inherited by all entities should go here
    //(EntityData contains info, rather than methods)
}
