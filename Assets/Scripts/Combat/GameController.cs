using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Transactions;
using UnityEngine.SocialPlatforms;
using TMPro;

public class GameController : MonoBehaviour
{
    private GameObject[] playerUnits;
    //private GameObject[] allyUnits;
    private GameObject[] enemyUnits;

    private List<GameObject[]> combatPhases;
    private List<FighterStats> fighterStats;

    private Cursor cursor;
    private GameObject pathfindingObject;

    private GameObject battleMenu;

    public TextMeshProUGUI battleText; //from Fighter Stats, shows damage done

    private void Awake()
    {
        battleMenu = GameObject.Find("ActionMenu");
    }
    void Start() 
    {
        combatPhases = new List<GameObject[]>();
        fighterStats = new List<FighterStats>();

        playerUnits = GameObject.FindGameObjectsWithTag("Hero");
        //allyUnits = GameObject.FindGameObjectsWithTag("Ally");
        enemyUnits = GameObject.FindGameObjectsWithTag("Enemy");

        combatPhases.Add(playerUnits);
        combatPhases.Add(enemyUnits);

        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        pathfindingObject = GameObject.Find("A*");
    }

    public void NextPhase()
    {
        GameObject[] currentPhaseUnits = combatPhases[0];

        if (currentPhaseUnits.Length > 0)
        {
            bool completedAllTurns = true;

            foreach (GameObject unit in currentPhaseUnits)
            {
                if (unit.GetComponent<FighterStats>().isSelectable)
                {
                    completedAllTurns = false;
                    return;
                }
            }
            if (completedAllTurns)
            {
                combatPhases.Remove(currentPhaseUnits);
                combatPhases.Add(currentPhaseUnits);
            }
        }
    }


    public void FirstTurn() //previously on Start() //adds turn speed to list, sorts to determine turn order
    {
        GameObject hero = cursor.selectedUnit;
        FighterStats currentFighterStats = hero.GetComponent<FighterStats>();
        currentFighterStats.CalculateNextTurn(0);
        fighterStats.Add(currentFighterStats);

        GameObject enemy = cursor.selectedEnemy;
        FighterStats currentEnemyStats = enemy.GetComponent<FighterStats>();
        currentEnemyStats.CalculateNextTurn(0);
        fighterStats.Add(currentEnemyStats);

        fighterStats.Sort();


        NextTurn();
    }

    public void NextTurn() //calls itself in a loop unless a character dies
    {
        battleText.gameObject.SetActive(false);
        
        FighterStats currentFighterStats = fighterStats[0];
        fighterStats.Remove(currentFighterStats);
        if (!currentFighterStats.GetDead() && !currentFighterStats.hasAttacked)
        {
            GameObject currentUnit = currentFighterStats.gameObject;
            currentFighterStats.CalculateNextTurn(currentFighterStats.nextActTurn);
            fighterStats.Add(currentFighterStats);
            fighterStats.Sort();
            if (currentUnit.tag == "Hero")
            {
                this.battleMenu.SetActive(true);
            }
            else //choose enemy action
            {
                this.battleMenu.SetActive(false);
                //string attackType = Random.Range(0, 2) == 1 ? "melee" : "range";
                string attackType = "attack";
                currentUnit.GetComponent<FighterAction>().SelectAction(attackType);
            }
            currentFighterStats.hasAttacked = true;
        }
        /*else //the unit is still alive and has already attacked
        {
            //NextTurn();
        }*/

        currentFighterStats.isSelectable = false;
        currentFighterStats.isSelected = false;
    }
}