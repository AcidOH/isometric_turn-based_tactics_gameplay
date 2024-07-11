using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    void Start()
    {
        fighterStats = owner.GetComponent<FighterStats>();
        attackStats = owner.GetComponent<AttackScript>();

        exp.text = fighterStats.experience.ToString();
        hp.text = fighterStats.health.ToString();
        eng.text = fighterStats.energy.ToString();
        def.text = fighterStats.defence.ToString();
        res.text = fighterStats.resistance.ToString();
        spd.text = fighterStats.speed.ToString();
        //skl.text = fighterStats.skill.ToString();
    }
}
