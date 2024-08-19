using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FighterAction : MonoBehaviour
{
    //private GameObject hero;
    //private GameObject enemy;

    [SerializeField]
    private GameObject meleePrefab;

    //[SerializeField]
    //private Sprite faceIcon;

    private GameObject currentAttack;

    private Cursor cursor;
    private GameController gameController;

    void Awake()
    {
        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        gameController = GameObject.Find("GameControllerObject").GetComponent<GameController>();
    }

    public void SelectAction(string btn) //called by Make Button
    {
        Debug.Log("SelectAction is called.");

        if (btn.CompareTo("attack") == 0)
        {
            Debug.Log(gameController.currentVictim + " was hit by attack!");
            meleePrefab.GetComponent<AttackScript>().Attack(gameController.currentVictim);
        }
        else //wait
        {
            Debug.Log(gameController.currentAttacker + " waited.");
            meleePrefab.GetComponent<AttackScript>().Wait();
        }
    }
}