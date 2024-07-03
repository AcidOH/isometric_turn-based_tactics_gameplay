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
    private GameObject magicFill;

    [Header("Stats")]
    public float health;
    public float magic;
    public float melee;
    public float magicRange;
    public float defense;
    public float speed;
    public float experience;

    private float startHealth;
    private float startMagic;

    [HideInInspector]
    public int nextActTurn; //defined in CalculateNextTurn

    private bool dead = false;

    // Resize health and magic bar
    private Transform healthTransform;
    private Transform magicTransform;

    private Vector2 healthScale; //determined by stats in Awake
    private Vector2 magicScale;

    private float xNewHealthScale; //defined in ReceiveDamage
    private float xNewMagicScale;

    private GameObject GameControllerObj;

    void Awake()
    {
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;

        magicTransform = magicFill.GetComponent<RectTransform>();
        magicScale = magicFill.transform.localScale;

        startHealth = health;
        startMagic = magic;

        GameControllerObj = GameObject.Find("GameControllerObject");
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

    public void updateMagicFill(float cost) //called in Attack Script
    {
        if (cost > 0) //only for the magic fill: there's already a check for enough magic left in Attack Script
        {
            magic = magic - cost;
            xNewMagicScale = magicScale.x * (magic / startMagic);
            magicFill.transform.localScale = new Vector2(xNewMagicScale, magicScale.y);
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