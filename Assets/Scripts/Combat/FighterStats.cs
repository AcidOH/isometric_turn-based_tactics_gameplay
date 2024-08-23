using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class FighterStats : MonoBehaviour, IComparable
{
    [SerializeField]
    private GameObject owner;

    //[SerializeField]
    //private Animator animator;

    [SerializeField]
    private GameObject healthFill;

    [SerializeField]
    private GameObject energyFill;

    [HideInInspector]
    public int nextActTurn; //defined in CalculateNextTurn

    private bool dead = false;

    private Transform healthTransform;
    private Transform energyTransform;

    private Vector2 healthScale; //determined by stats in Awake
    private Vector2 energyScale;

    private float xNewHealthScale; //defined in ReceiveDamage
    private float xNewEnergyScale;

    private GameObject GameControllerObj;
    private GameController gameController;
    private TextMeshProUGUI battleText;

    private float health;
    private float energy;
    private float movement;

    public float attackRange;

    private Overlay overlay;
    private Pathfinding pathfinder;
    
    public bool isSelected = false;
    public bool lookForPath = false;
    public bool hasMoved = false;
    public bool hasCompletedTurn = false;

    public Vector3 lastWorldPos;
    public Vector3 currentTilePos;

    private Cursor cursor;
    private GameObject actionMenu;

    private StatTextController statTextController;

    void Awake()
    {
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;

        energyTransform = energyFill.GetComponent<RectTransform>();
        energyScale = energyFill.transform.localScale;

        HumanEntity humanentity = owner.GetComponent<HumanEntity>();
        HumanData humandata = owner.GetComponent<HumanEntity>().HumanData;
        CharacterStat characterstathealth = owner.GetComponent<HumanEntity>().HumanData.healthPoints;

        health = owner.GetComponent<HumanEntity>().HumanData.healthPoints.BaseValue;
        energy = owner.GetComponent<HumanEntity>().HumanData.energy.BaseValue;
        movement = owner.GetComponent<HumanEntity>().HumanData.movement.Value;

        GameControllerObj = GameObject.Find("GameControllerObject");
        gameController = GameControllerObj.GetComponent<GameController>();
        battleText = GameControllerObj.GetComponent<GameController>().battleText;

        overlay = GameObject.Find("A*").GetComponent<Overlay>();
        pathfinder = GameObject.Find("A*").GetComponent<Pathfinding>();

        lastWorldPos = owner.transform.position;
        currentTilePos = lastWorldPos + new Vector3(0, 0, -1);
        //^^This isn't always going to be true, it just works for now!!

        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        actionMenu = GameObject.Find("ActionMenu");
        statTextController = owner.GetComponent<StatTextController>();
    }

    
    public void SelectUnitAsAttacker()
    {
        Debug.Log("A unit is selected as an attacker.");

        if (!hasCompletedTurn)
        {
            if (!isSelected)
            {
                isSelected = true;
                gameController.currentAttacker = owner;
                if (owner.tag == "Hero") statTextController.ShowStatDisplay();
            }
            else
            {
                hasMoved = true;
                lookForPath = false;
            }
            
            if (!hasMoved && owner.tag == "Hero")
            {
                ShowTilesInRange(movement*10);
                lookForPath = true;

            }
            else if (hasMoved && !hasCompletedTurn)
            {
                if (owner.tag == "Hero")
                {
                    ShowTilesInRange(attackRange*10);
                    actionMenu.SetActive(true);
                }
                else
                {
                    string attackType = "wait";

                    if (isVictimInRange(gameController.currentVictim))
                    {
                        attackType = "attack";
                    }
                    
                    owner.GetComponent<FighterAction>().SelectAction(attackType);
                }
            }
        }
    }

    public void SelectUnitAsVictim()
    {
        Debug.Log("A unit is selected as a victim.");

        isSelected = true;
        gameController.currentVictim = owner;

        if (owner.tag == "Enemy") statTextController.ShowStatDisplay();
    }

    public void UnselectCompletedTurnUnit()
    {
        Debug.Log("Units were unselected.");
        
        isSelected = false;
        hasMoved = true;
        hasCompletedTurn = true;

        if (owner.tag == "Hero")
        {
            gameController.SetAttackButtonAsInteractable(false);
            actionMenu.SetActive(false);
            statTextController.HideAllStatDisplays();
        }

        gameController.currentAttacker = null;
        gameController.currentVictim = null;

        Invoke("HideBattleText", 1.5f);

        if (gameController.IsNextPhase())
        {
            gameController.NextPhase();
        }
    }

    public void UnselectIncompleteTurnUnit()
    {
        owner.transform.position = lastWorldPos;
        isSelected = false;
        hasMoved = false;

        if (owner.tag == "Hero")
        {
            actionMenu.SetActive(false);
            statTextController.HideStatDisplay();
        } 
    }

    public void Move(Vector3 newWorldPos)
    {
        if (pathfinder.isInRange(overlay.NodeFromWorldPoint(owner.transform.position), overlay.NodeFromWorldPoint(newWorldPos), movement * 10) && !hasMoved)
        {
            lastWorldPos = owner.transform.position;
            owner.transform.position = newWorldPos;

            cursor.ShowSelectedTile();

            hasMoved = true;
            lookForPath = false;

            SelectUnitAsAttacker();
        }
        else if (!hasMoved)
        {
            lookForPath = false;
            isSelected = false;
        }
        /*else
        {
            owner.transform.position = lastWorldPos;
            isSelected = false;
            hasMoved = false;
        }*/
    }

    private void HideBattleText()
    {
        battleText.gameObject.SetActive(false);
    }


    public void ReceiveDamage(float damage) //called in Attack Script (Attack)
    {
        health = health - damage;
        //animator.Play("Damage");

        if (health <= 0)
        {
            dead = true;
            gameObject.tag = "Dead";

            Destroy(healthFill);
            Destroy(gameObject);
        }
        else if (damage > 0)
        {
            xNewHealthScale = healthScale.x * (health / owner.GetComponent<HumanEntity>().HumanData.healthPoints.BaseValue);
            healthFill.transform.localScale = new Vector2(xNewHealthScale, healthScale.y);
        }
        if (damage > 0) //for battleText, could be part of previous else if
        {
            battleText.gameObject.SetActive(true);
            battleText.text = damage.ToString();
        }

    }

    public void updateEnergyFill(float cost) //called in Attack Script
    {
        if (cost > 0) //only for the energy fill: there's already a check for enough energy left in Attack Script
        {
            energy = energy - cost;
            xNewEnergyScale = energyScale.x * (energy / owner.GetComponent<HumanEntity>().HumanData.energy.BaseValue);
            energyFill.transform.localScale = new Vector2(xNewEnergyScale, energyScale.y);
        }
    }

    public bool GetDead() //called in Game Controller
    {
        return dead;
    }

    public void CalculateNextTurn(int currentTurn) //called in Game Controller
    {
        nextActTurn = currentTurn + Mathf.CeilToInt(100f / owner.GetComponent<HumanEntity>().HumanData.speed.Value);
    }

    public int CompareTo(object otherStats) //fcn and the method are called CompareTo
    {
        int nex = nextActTurn.CompareTo(((FighterStats)otherStats).nextActTurn);
        return nex;
    }

    public void ShowTilesInRange(float range)
    {
        if (overlay.grid != null)
        {
            Vector3 origin = owner.transform.position;
            Node originNode = overlay.NodeFromWorldPoint(origin);
           
            foreach (Node node in overlay.grid)
            {
                if (!pathfinder.isInRange(originNode, node, range))
                {
                    node.overlayTile.GetComponent<SpriteRenderer>().color = Color.red;
                }

            }
        }
    }

    public bool isVictimInRange(GameObject victim)
    {
        Vector3 attackerPos = owner.transform.position;
        Node attackerNode = overlay.NodeFromWorldPoint(attackerPos);

        Vector3 victimPos = victim.transform.position;
        Node victimNode = overlay.NodeFromWorldPoint(victimPos);

        return pathfinder.isInRange(attackerNode, victimNode, attackRange*10);
        
    }

    public void FindNearestVictim()
    {
        
    }

    public Vector3 FindPosClosestToVictim()
    {
        Node victimNode = pathfinder.FindNodeWithinRange(owner.transform.position, gameController.currentVictim.transform.position, false, movement);
        
        if (victimNode != null)
        {
            Debug.Log("Closest pos to victim = " + victimNode.worldPosition);
            return victimNode.worldPosition;
        }
        Debug.Log("victimNode == null, returned owner pos");
        return owner.transform.position;
    }
}