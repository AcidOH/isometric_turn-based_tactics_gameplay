using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    [SerializeField]
    private GameObject owner;

    //[SerializeField]
    //private string animationName;

    [SerializeField]
    private float energyCost;

    private HumanData attackerStats;
    private HumanData targetStats;

    private FighterStats attackerFighterStats;
    private FighterStats targetFighterStats;

    private float damage;

    public void Attack(GameObject victim) //called in Fighter Action
    {
        attackerStats = owner.GetComponent<HumanEntity>().HumanData; 
        targetStats = victim.GetComponent<HumanEntity>().HumanData; // victim is the thing being attacked

        attackerFighterStats = owner.GetComponent<FighterStats>();
        targetFighterStats = victim.GetComponent<FighterStats>();

        if (attackerStats.energy.Value >= energyCost) //checks for enough mana
        {
            if (owner.GetComponent<ItemEntity>() != null && owner.GetComponent<ItemEntity>().ItemData.itemType.ToString() == "MagicWeapon")
            {
                damage = attackerStats.attackMagic.Value - targetStats.resilience.Value;
            }
            else
            {
                damage = attackerStats.attackNonMagic.Value - targetStats.protect.Value;
            }

            //owner.GetComponent<Animator>().Play(animationName);
            targetFighterStats.ReceiveDamage(Mathf.CeilToInt(damage));
            attackerFighterStats.updateEnergyFill(energyCost);
            
            attackerFighterStats.hasCompletedTurn = true;
            attackerFighterStats.isSelected = false;
        }
        else
        {
            //Invoke("SkipTurnContinueGame", 2); //I'm not sure you want a delay...
        }
    }

    void SkipTurnContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }
}