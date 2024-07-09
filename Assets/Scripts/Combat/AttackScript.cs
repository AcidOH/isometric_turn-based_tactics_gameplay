using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// hello
public class AttackScript : MonoBehaviour
{
    public GameObject owner;

    //[SerializeField]
    //private string animationName;

    [SerializeField]
    private float energyCost;

    [SerializeField]
    private float minAttackMultiplier;

    [SerializeField]
    private float maxAttackMultiplier;

    [SerializeField]
    private float minDefenseMultiplier;

    [SerializeField]
    private float maxDefenseMultiplier;

    private FighterStats attackerStats;
    private FighterStats targetStats;
    private float damage = 0.0f;
    private bool critHit = false;

    public void Attack(GameObject victim) //called in Fighter Action
    {
        attackerStats = owner.GetComponent<FighterStats>();
        targetStats = victim.GetComponent<FighterStats>();
        if (attackerStats.energy >= energyCost) //checks for enough mana
        {
            float chanceToHit = attackerStats.hit / targetStats.hit;
            bool doesHit = HitFromPercent(chanceToHit);
            if (doesHit == true)
            {
                //if you want to include modifiers, do so here.

                damage = attackerStats.melee + attackerStats.skill - targetStats.defence;

                float crit = attackerStats.skill / targetStats.skill;
                critHit = HitFromPercent(crit);

                if (critHit == true)
                {
                    float critDamage = Mathf.Round(damage * crit);
                    damage = damage + critDamage;

                    Debug.Log("Critical hit! Extra damage done: " + critDamage);
                }
            }
            else
            {
                damage = 0.0f;
            }

            //owner.GetComponent<Animator>().Play(animationName);
            targetStats.ReceiveDamage(Mathf.CeilToInt(damage));
            attackerStats.updateEnergyFill(energyCost);
        }
        else
        {
            Invoke("SkipTurnContinueGame", 2); //I'm not sure you want a delay...
        }
    }

    public bool HitFromPercent(float percent)
    {
        int num = Random.Range(0, 101);
        if (num <= percent * 100)
        {
            return true;
        }
        return false;
    }

    void SkipTurnContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }
}