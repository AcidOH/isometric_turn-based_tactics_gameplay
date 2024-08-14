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

    private Cursor cursor;

    void Awake()
    {
        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
    }

    public void SelectAction(string btn) //called by Make Button
    {
        if (cursor.selectedEnemy != null)
        {
            hero = cursor.selectedUnit;
            enemy = cursor.selectedEnemy;

            GameObject victim = hero;

            if (tag == "Hero") //tag of the object the script is attached to
            {
                victim = enemy;
            }
            if (btn.CompareTo("attack") == 0)
            {
                Debug.Log(victim + " was hit by attack!");
                meleePrefab.GetComponent<AttackScript>().Attack(victim);
            }
            else //wait
            {
                Debug.Log(hero + " waited.");
                //hasMoved = true, isSelectable = false, isSelected = false
            }
        }
    }
}