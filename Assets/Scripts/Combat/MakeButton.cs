using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeButton : MonoBehaviour
{
    [SerializeField]
    private bool physical;

    private GameObject hero;
    void Start()
    {
        string temp = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(() => AttachCallback(temp));
        hero = GameObject.FindGameObjectWithTag("Hero");
    }

    private void AttachCallback(string btn)
    {
        if (btn.CompareTo("AttackBtn") == 0)
        {
            hero.GetComponent<FighterAction>().SelectAction("attack");
        }
        else //wait
        {
            hero.GetComponent<FighterAction>().SelectAction("wait");
        }
    } //when button pressed, checks who is attacked in Fighter Action, then calls Attack() in Attack Script
}