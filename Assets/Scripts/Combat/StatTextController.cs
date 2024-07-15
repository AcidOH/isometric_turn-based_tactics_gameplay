using TMPro;
using UnityEngine;

public class StatTextController : MonoBehaviour
{
    public GameObject owner;

    private FighterStats fighterStats;
    private AttackScript attackStats;

    public TextMeshProUGUI exp;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI eng;

    public TextMeshProUGUI def;
    public TextMeshProUGUI res;
    public TextMeshProUGUI spd;
    public TextMeshProUGUI skl;

    public TextMeshProUGUI hit;
    public TextMeshProUGUI dmg;
    public TextMeshProUGUI crit;

    private float startHealth;
    private float startEnergy;

    private float experience;
    private float health;
    private float energy;

    private float toHit;
    private float toCrit;
    private float damage;

    public TextMeshProUGUI numOfAttacks; //set in Attack Script

    void Start()
    {
        fighterStats = owner.GetComponent<FighterStats>();
        attackStats = owner.GetComponentInChildren<AttackScript>();

        experience = fighterStats.experience;

        startHealth = fighterStats.health;
        health = startHealth;

        startEnergy = fighterStats.energy;
        energy = startEnergy;
        
        exp.text = experience.ToString();
        hp.text = health.ToString();
        eng.text = energy.ToString();

        def.text = fighterStats.defence.ToString();
        res.text = fighterStats.resistance.ToString();
        spd.text = fighterStats.speed.ToString();
        skl.text = fighterStats.skill.ToString();
    }

    void Update()
    {
        if (experience != fighterStats.experience)
        {
            experience = fighterStats.experience;
            exp.text = experience.ToString();
        }
        if (health != fighterStats.health)
        {
            health = fighterStats.health;
            hp.text = health.ToString() + "/" + startHealth.ToString();
        }
        if (energy != fighterStats.energy)
        {
            energy = fighterStats.energy;
            eng.text = energy.ToString() + "/" + startEnergy.ToString();
        }
    }
}
