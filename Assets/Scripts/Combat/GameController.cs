using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Transactions;
using UnityEngine.SocialPlatforms;
using TMPro;
using UnityEditor;

public class GameController : MonoBehaviour
{
    private FighterStats[] playerUnits;
    //private GameObject[] allyUnits;
    private FighterStats[] enemyUnits;

    private List<FighterStats[]> combatPhases;
    private List<FighterStats> fighterStats;

    public GameObject currentAttacker;
    public GameObject currentVictim;
    public string currentPhase = "Player Phase";

    private Cursor cursor;
    private GameObject pathfindingObject;

    private GameObject battleMenu;

    public TextMeshProUGUI battleText;
    public TextMeshProUGUI phaseText;

    private void Awake()
    {
        battleMenu = GameObject.Find("ActionMenu");
    }
    void Start() 
    {
        this.battleMenu.SetActive(false);
        
        combatPhases = new List<FighterStats[]>();
        fighterStats = new List<FighterStats>();

        GameObject[] playerUnitObjects = GameObject.FindGameObjectsWithTag("Hero");
        //allyUnits = GameObject.FindGameObjectsWithTag("Ally");
        GameObject[] enemyUnitObjects = GameObject.FindGameObjectsWithTag("Enemy");

        playerUnits = new FighterStats[playerUnitObjects.Length];
        enemyUnits = new FighterStats[enemyUnitObjects.Length];

        for (int i = 0; i < playerUnitObjects.Length; i++)
        {
            playerUnits[i] = playerUnitObjects[i].GetComponent<FighterStats>();
        }

        for (int i = 0; i < enemyUnitObjects.Length; i++)
        {
            enemyUnits[i] = enemyUnitObjects[i].GetComponent<FighterStats>();
        }

        combatPhases.Add(playerUnits);
        combatPhases.Add(enemyUnits);

        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        pathfindingObject = GameObject.Find("A*");
    }


    public bool IsNextPhase()
    {
        Debug.Log("IsNextPhase is called.");
        
        FighterStats[] currentPhaseUnits = combatPhases[0];

        if (currentPhaseUnits.Length > 0)
        {
            foreach (FighterStats unitStats in currentPhaseUnits)
            {
                if (!unitStats.hasCompletedTurn)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void NextPhase()
    {
        Debug.Log("NextPhase is called.");

        FighterStats[] currentPhaseUnits = combatPhases[0];

        combatPhases.Remove(currentPhaseUnits);
        combatPhases.Add(currentPhaseUnits);

        currentPhaseUnits = combatPhases[0];
        foreach (FighterStats unit in currentPhaseUnits)
        {
            unit.hasMoved = false;
            unit.hasCompletedTurn = false;
        }

        if (currentPhaseUnits == playerUnits)
        {
            currentPhase = "Player Phase";
            phaseText.text = currentPhase;

            Debug.Log("PLAYER PHASE");
        }
        else if (currentPhaseUnits == enemyUnits) //or == allyUnits
        {
            currentPhase = "Enemy Phase";
            phaseText.text = currentPhase;

            Debug.Log("ENEMY PHASE");

            foreach (FighterStats unit in currentPhaseUnits)
            {
                int indexOfVictim = Random.Range(0, playerUnits.Length - 1);
                playerUnits[indexOfVictim].SelectUnitAsVictim();

                //b/c enemy units don't get to move yet, set hasMoved to true so it can attack
                unit.hasMoved = true;
                unit.SelectUnitAsAttacker();
            }
        }
    }

    public void DetermineAttackOrder()
    {
        fighterStats.Clear();
        
        FighterStats currentAttackerStats = currentAttacker.GetComponent<FighterStats>();
        FighterStats currentVictimStats = currentVictim.GetComponent<FighterStats>();

        currentAttackerStats.CalculateNextTurn(0);
        currentVictimStats.CalculateNextTurn(0);

        fighterStats.Add(currentAttackerStats);
        fighterStats.Add(currentVictimStats);

        fighterStats.Sort();
    }

}