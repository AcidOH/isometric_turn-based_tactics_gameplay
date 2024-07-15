using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FighterAction : MonoBehaviour
{
    private GameObject hero;
    private GameObject enemy;

    [SerializeField]
    private GameObject meleePrefab;

    //[SerializeField]
    //private Sprite faceIcon;

    private GameObject currentAttack;

    void Awake()
    {
        hero = GameObject.FindGameObjectWithTag("Hero");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
    }
    public void SelectAttack(string btn) //called by Make Button
    {
        GameObject victim = hero;
        if (tag == "Hero")
        {
            victim = enemy;
        }
        if (btn.CompareTo("melee") == 0)
        {
            Debug.Log(victim + " was hit by melee attack!");
            meleePrefab.GetComponent<AttackScript>().Attack(victim);

        }
        else if (btn.CompareTo("range") == 0)
        {
            Debug.Log(victim + " was hit by range attack!");
        }
        else
        {
            Debug.Log("Run");
        }
    }
}