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
        this.battleMenu.SetActive(false);
        
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
        battleText.gameObject.SetActive(false);
        battleMenu.SetActive(false);

        GameObject[] currentPhaseUnits = combatPhases[0];

        if (currentPhaseUnits.Length > 0)
        {
            bool completedAllTurns = true;

            if (currentPhaseUnits == enemyUnits)
            {
                EnemyPhase();

                //prepare for player phase
                foreach (GameObject playerUnit in playerUnits)
                {
                    FighterStats playerFighterStats = playerUnit.GetComponent<FighterStats>();
                    
                    playerFighterStats.hasCompletedTurn = false;
                    playerFighterStats.hasMoved = false;
                }
            }

            foreach (GameObject unit in currentPhaseUnits)
            {
                if (!unit.GetComponent<FighterStats>().hasCompletedTurn)
                {
                    completedAllTurns = false;
                    break;
                }
            }
            if (completedAllTurns)
            {
                combatPhases.Remove(currentPhaseUnits);
                combatPhases.Add(currentPhaseUnits);

                //NextPhase();
            }
        }
    }

    public void EnemyPhase()
    {
        foreach (GameObject enemyUnit in enemyUnits)
        {
            CombatTurn(); //there's a better way to do this, and a better way to CombatTurn()
            //makes more sense to construct enemy behaviour here and in enemy class/data.
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

    public void CombatTurn()
    {
        if (cursor.selectedUnitsFighterStats.isSelected)
        {
            //its hero's turn
            this.battleMenu.SetActive(true);
            //as part of attack, determine attack order, then set the attacker's has attacked to true.
            
        }
        else
        {
            //this.battleMenu.SetActive(false);
            string attackType = "attack";
            cursor.selectedEnemy.GetComponent<FighterAction>().SelectAction(attackType);
        }
    }

    public void NextTurn() //calls itself in a loop unless a character dies
    {
        battleText.gameObject.SetActive(false);
        
        FighterStats currentFighterStats = fighterStats[0];
        fighterStats.Remove(currentFighterStats);
        if (!currentFighterStats.GetDead() && !currentFighterStats.hasCompletedTurn)
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
            currentFighterStats.hasCompletedTurn = true;
        }
        /*else //the unit is still alive and has already attacked
        {
            //NextTurn();
        }*/

        currentFighterStats.hasCompletedTurn = true;
        currentFighterStats.isSelected = false;
    }
}