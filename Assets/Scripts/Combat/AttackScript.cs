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

    private HumanData attackerStats;
    private HumanData targetStats;

    private float damage;

    public void Attack(GameObject victim) //called in Fighter Action
    {
        attackerStats = owner.GetComponent<HumanEntity>().HumanData; 
        targetStats = victim.GetComponent<HumanEntity>().HumanData; // victim is the thing being attacked

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
            //targetStats.ReceiveDamage(Mathf.CeilToInt(damage));
            //attackerStats.updateEnergyFill(energyCost);
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