using TMPro;
using UnityEngine;

public class StatTextController : ScriptableObject
{
    private GameObject heroStatDisplay;
    private GameObject enemyStatDisplay;
    
    private HumanData fighterStats;
    private AttackScript attackStats;

    //public TextMeshProUGUI exp;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI eng;

    public TextMeshProUGUI def;
    public TextMeshProUGUI res;
    public TextMeshProUGUI spd;
    //public TextMeshProUGUI skl;

    //public TextMeshProUGUI hit;
    //public TextMeshProUGUI dmg;
    //public TextMeshProUGUI crit;

    //public TextMeshProUGUI numOfAttacks; //set in Attack Script

    public void ShowStatDisplay()
    {
        GameObject.Find("HeadsUpCanvas").transform.GetChild(6).gameObject.SetActive(true); //GetChild asks for the index of the child in an array of children
    }
    public void HideStatDisplay()
    {
        GameObject.Find("HeadsUpCanvas").transform.GetChild(6).gameObject.SetActive(false);
    }

    public void UpdateTextInStatDisplay(GameObject owner)
    {
        fighterStats = owner.GetComponent<HumanEntity>().HumanData;
        attackStats = owner.GetComponentInChildren<AttackScript>();

        //exp.text = fighterStats.experience.Value.ToString();
        hp.text = fighterStats.healthPoints.Value.ToString();
        eng.text = fighterStats.energy.Value.ToString();

        def.text = fighterStats.defence.Value.ToString();
        res.text = fighterStats.resistance.Value.ToString();
        spd.text = fighterStats.speed.Value.ToString();
        //skl.text = fighterStats.skill.Value.ToString();
    }

}
