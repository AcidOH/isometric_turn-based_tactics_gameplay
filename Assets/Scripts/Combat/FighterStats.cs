using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using TMPro;

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

    private Overlay overlay;
    private Pathfinding pathfinder;
    
    public bool isSelected = false;
    public bool lookForPath = false;
    public bool hasMoved = false;
    public bool hasCompletedTurn = false;

    public Vector3 lastWorldPos;

    private Cursor cursor;
    private GameObject actionMenu;

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

        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        actionMenu = GameObject.Find("ActionMenu");
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
            }
            else
            {
                hasMoved = true;
                lookForPath = false;
            }
            
            if (!hasMoved && owner.tag == "Hero")
            {
                ShowTilesInRange();

                lookForPath = true;

                //I don't want Move to be called by cursor, but it needs to be linked to Update

            }
            else if (hasMoved && !hasCompletedTurn)
            {
                if (owner.tag == "Hero")
                {
                    actionMenu.SetActive(true);
                }
                else
                {
                    string attackType = "attack";
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
        }

        gameController.currentAttacker = null;
        gameController.currentVictim = null;

        Invoke("HideBattleText", 1.5f);

        if (gameController.IsNextPhase())
        {
            gameController.NextPhase();
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
        else
        {
            owner.transform.position = lastWorldPos;
            isSelected = false;
            hasMoved = false;
        }
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

    public void ShowTilesInRange()
    {
        if (overlay.grid != null)
        {
            Vector3 origin = owner.transform.position;
            Node originNode = overlay.NodeFromWorldPoint(origin);
            
            float _playerRangeOfMovement = owner.GetComponent<HumanEntity>().HumanData.movement.Value * 10;

            foreach (Node node in overlay.grid)
            {
                if (!pathfinder.isInRange(originNode, node, _playerRangeOfMovement))
                {
                    node.overlayTile.GetComponent<SpriteRenderer>().color = Color.red;
                }

            }
        }
    }

}