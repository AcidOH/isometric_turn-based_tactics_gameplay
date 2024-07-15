using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public GameObject owner;

    //[SerializeField]
    //private string animationName;

    [SerializeField]
    private float energyCost;

    private FighterStats attackerStats;
    private FighterStats targetStats;

    private float damage;

    public void Attack(GameObject victim) //called in Fighter Action
    {
        attackerStats = owner.GetComponent<FighterStats>(); 
        targetStats = victim.GetComponent<FighterStats>(); // victim is the thing being attacked

        if (attackerStats.energy >= energyCost) //checks for enough mana
        {
            damage = attackerStats.melee;

            //owner.GetComponent<Animator>().Play(animationName);
            targetStats.ReceiveDamage(Mathf.CeilToInt(damage));
            attackerStats.updateEnergyFill(energyCost);
        }
        else
        {
            Invoke("SkipTurnContinueGame", 2); //I'm not sure you want a delay...
        }
    }

    void SkipTurnContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }
}