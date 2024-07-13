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

    private float damage;
    private GameObject GameControllerObj;
    
    public float damageBeforeCrit = 0.0f;
    public float chanceToHit = 0.0f;
    public float critChance = 0.0f;

    void Start()
    {
        GameControllerObj = GameObject.Find("GameControllerObject");
    }


    public void Attack(GameObject victim) //called in Fighter Action
    {
        attackerStats = owner.GetComponent<FighterStats>(); 
        targetStats = victim.GetComponent<FighterStats>(); // victim is the thing being attacked

        int numOfAttacks = 0;

        if (attackerStats.energy >= energyCost) //checks for enough mana
        {
            float hits = targetStats.hit;

            while (hits > 0)
            {
                numOfAttacks++;

                chanceToHit = attackerStats.hit / targetStats.hit; //could be hits instead of targetstats if it does too much damage
                bool doesHit = HitFromPercent(chanceToHit);
                if (doesHit == true)
                {
                    //if you want to include modifiers, do so here.

                    damageBeforeCrit = attackerStats.melee + attackerStats.skill - targetStats.defence;

                    critChance = attackerStats.skill / targetStats.skill > 1 ? 1 : attackerStats.skill / targetStats.skill;
                    bool critHit = HitFromPercent(critChance);

                    if (critHit == true)
                    {
                        float critDamage = Mathf.Round(damageBeforeCrit * critChance);
                        damage = damageBeforeCrit + critDamage;

                        GameControllerObj.GetComponent<GameController>().doesCrit.gameObject.SetActive(true);

                        Debug.Log("Critical hit! Extra damage done: " + critDamage);
                    }
                }
                else
                {
                    damageBeforeCrit = 0.0f;
                }

                //owner.GetComponent<Animator>().Play(animationName);
                targetStats.ReceiveDamage(Mathf.CeilToInt(damage));
                attackerStats.updateEnergyFill(energyCost);


                hits = attackerStats.hit - targetStats.hit - hits;
            }

            owner.GetComponentInChildren<StatTextController>().numOfAttacks.text = "x" + numOfAttacks.ToString();
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