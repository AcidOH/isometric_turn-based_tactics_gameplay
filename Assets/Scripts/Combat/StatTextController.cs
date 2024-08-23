using TMPro;
using UnityEngine;

public class StatTextController : MonoBehaviour
{
    [SerializeField]
    private GameObject owner;
    
    private GameObject heroStatDisplay;
    private GameObject enemyStatDisplay;
    
    private HumanData humanData;
    private AttackScript attackStats;

    //private string exp;
    private string hp;
    private string eng;

    private string def;
    private string res;
    private string spd;
    //private string skl;

    private string hit;
    private string dmg;
    private string crit;

    private TextMeshProUGUI expText;
    private TextMeshProUGUI hpText;
    private TextMeshProUGUI engText;

    private TextMeshProUGUI defText;
    private TextMeshProUGUI resText;
    private TextMeshProUGUI spdText;
    private TextMeshProUGUI sklText;

    private TextMeshProUGUI hitText;
    private TextMeshProUGUI dmgText;
    private TextMeshProUGUI critText;

    private TextMeshProUGUI numOfAttacks; //set in Attack Script

    private void Awake()
    {
        heroStatDisplay = GameObject.Find("HeroInfo");
        enemyStatDisplay = GameObject.Find("EnemyInfo");
    }

    private void Start()
    {
        humanData = owner.GetComponent<HumanEntity>().HumanData;
        attackStats = owner.GetComponentInChildren<AttackScript>();

        //exp = humanData.experience.Value.ToString();
        hp = humanData.healthPoints.Value.ToString();
        eng = humanData.energy.Value.ToString();

        def = humanData.defence.Value.ToString();
        res = humanData.resistance.Value.ToString();
        spd = humanData.speed.Value.ToString();
        //skl = humanData.skill.Value.ToString();

        //Set stats here, maybe don't need all of them.
        //The ones that are subject to change like hit, dmg, crit should be set every time

        Debug.Log("HeroInfo: " + heroStatDisplay + ", EnemyInfo: " + enemyStatDisplay);

        heroStatDisplay.SetActive(false);
        enemyStatDisplay.SetActive(false);
    }

    public void ShowStatDisplay()
    {
        if (owner.tag == "Hero")
        {
            heroStatDisplay.SetActive(true);
        }
        else if (owner.tag == "Enemy")
        {
            enemyStatDisplay.SetActive(true);
        }
    }
    public void HideStatDisplay()
    {
        if (owner.tag == "Hero")
        {
            heroStatDisplay.SetActive(false);
        }
        else if (owner.tag == "Enemy")
        {
            enemyStatDisplay.SetActive(false);
        }
    }

    public void HideAllStatDisplays()
    {
        heroStatDisplay.SetActive(false);
        enemyStatDisplay.SetActive(false);
    }

    public void UpdateTextInStatDisplay()
    {
        //exp.text = humanData.experience.Value.ToString();
        hpText.text = humanData.healthPoints.Value.ToString();
        engText.text = humanData.energy.Value.ToString();

        defText.text = def;
        resText.text = res;
        spdText.text = spd;
        //skl.text = humanData.skill.Value.ToString();
    }

    /*public void UpdateProfileImage(GameObject owner)
    {
        Sprite fighterImage = owner.GetComponent<HumanEntity>().ProfileImage;
        GameObject ownerInfoUI = GameObject.Find("HeadsUpCanvas").transform.GetChild(3).gameObject;

        if (owner.tag == "Hero")
        {
            ownerInfoUI = GameObject.Find("HeadsUpCanvas").transform.GetChild(2).gameObject;
        }
        
        ownerInfoUI.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = fighterImage;
    }*/

    /*public void UpdateCharacterSprite(GameObject owner)
    {
        owner.GetComponent<SpriteRenderer>().sprite = owner.GetComponent<HumanEntity>().FullCharacterSprite;
    }*/
}
