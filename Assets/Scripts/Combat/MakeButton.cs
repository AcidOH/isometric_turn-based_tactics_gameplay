using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeButton : MonoBehaviour
{
    [SerializeField]
    private bool physical;

    private GameController gameController;

    void Start()
    {
        string temp = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(() => AttachCallback(temp));
        gameController = GameObject.Find("GameControllerObject").GetComponent<GameController>();
    }

    private void AttachCallback(string btn)
    {
        GameObject hero = gameController.currentAttacker;
        
        if (btn.CompareTo("AttackBtn") == 0)
        {
            hero.GetComponent<FighterAction>().SelectAction("attack");
        }
        else 
        {
            hero.GetComponent<FighterAction>().SelectAction("wait");
        }
    }
}