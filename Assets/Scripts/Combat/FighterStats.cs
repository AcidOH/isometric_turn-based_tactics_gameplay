using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FighterStats : MonoBehaviour, IComparable
{
    //[SerializeField]
    //private Animator animator;

    [SerializeField]
    private GameObject healthFill;

    [SerializeField]
    private GameObject energyFill;

    [Header("Stats")]
    public float experience;
    public float health;
    public float defence;
    public float resistance;
    public float speed;
    public float skill;
    public float energy; //(previous the "magic" stat)
    public float melee; //a placeholder stat for an attack's inherent damage

    [HideInInspector]
    public float hit;

    [HideInInspector]
    public int movement;

    private float startHealth;
    private float startEnergy;

    [HideInInspector]
    public int nextActTurn; //defined in CalculateNextTurn

    private bool dead = false;

    // Resize health and energy bar
    private Transform healthTransform;
    private Transform energyTransform;

    private Vector2 healthScale; //determined by stats in Awake
    private Vector2 energyScale;

    private float xNewHealthScale; //defined in ReceiveDamage
    private float xNewEnergyScale;

    private GameObject GameControllerObj;

    void Awake()
    {
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;

        energyTransform = energyFill.GetComponent<RectTransform>();
        energyScale = energyFill.transform.localScale;

        startHealth = health;
        startEnergy = energy;

        GameControllerObj = GameObject.Find("GameControllerObject");

        hit = speed + skill;
        movement = Mathf.CeilToInt(speed / 2);
    }

    public void ReceiveDamage(float damage) //called in Attack Script (Attack)
    {
        health = health - damage;
        Debug.Log("Damage = " + damage);
        Debug.Log("New health: " + health);
        //animator.Play("Damage");

        // Set damage text

        if (health <= 0)
        {
            dead = true;
            gameObject.tag = "Dead";
            Debug.Log("Someone died.");

            Destroy(healthFill);
            Destroy(gameObject);
        }
        else if (damage > 0)
        {
            xNewHealthScale = healthScale.x * (health / startHealth);
            healthFill.transform.localScale = new Vector2(xNewHealthScale, healthScale.y);
        }
        if (damage > 0) //for battleText, could be part of previous else if
        {
            GameControllerObj.GetComponent<GameController>().battleText.gameObject.SetActive(true);
            GameControllerObj.GetComponent<GameController>().battleText.text = damage.ToString();
        }
        Invoke("ContinueGame", 2);
    }

    public void updateEnergyFill(float cost) //called in Attack Script
    {
        if (cost > 0) //only for the energy fill: there's already a check for enough energy left in Attack Script
        {
            energy = energy - cost;
            xNewEnergyScale = energyScale.x * (energy / startEnergy);
            energyFill.transform.localScale = new Vector2(xNewEnergyScale, energyScale.y);
        }
    }

    public bool GetDead() //called in Game Controller
    {
        return dead;
    }

    void ContinueGame() //invoked in ReceiveDamage
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }
    public void CalculateNextTurn(int currentTurn) //called in Game Controller
    {
        nextActTurn = currentTurn + Mathf.CeilToInt(100f / speed);
    }

    public int CompareTo(object otherStats) //be mindful that both the fcn and the method are called CompareTo
    {
        int nex = nextActTurn.CompareTo(((FighterStats)otherStats).nextActTurn);
        return nex;
    }

}