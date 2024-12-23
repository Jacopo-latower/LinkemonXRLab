using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LinkemonAttack;

public class Linkemon : MonoBehaviour
{
    public enum LinkemonType { Dark, Psychic, Fire, Water, Electric, Grass, Flying, Metal, Fighting, Normal}

    public string linkemonName;
    public Sprite battleIcon;
    private LinkemonTrainer trainer;

    public LinkemonTrainer Trainer { set => trainer = value; get => trainer; }

    [Header("References")]
    [SerializeField] private Image battleImageUI;
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private Image healthBarUIFill;
    private Color healthBarColor;
    [SerializeField] private GameObject nameUI;
    [SerializeField] private GameObject typeUI;
    [SerializeField] private GameObject lifeNumUI;
    [SerializeField] private Animator lAnimator;
    [SerializeField] private AudioClip linkemonVerse;

    [Header("Stats")]
    public LinkemonType lType;
    public List<LinkemonType> weaknessTypes; //anche questo fa schifo e dovrebbe stare dentro un fantomatico "LinkemonType" object che non esiste  ma troppo lungo
    public List<LinkemonAttack> attackList;
    public Dictionary<string, int> attacksMap; //Fa schifo ma è troppo lungo fare tutto bene -> UPDATE: NON LO USIAMO PIU'

    private int startingLife;
    private int startingSpeed;
    private int startingElusion;
    private int startingAttack;
    private int startingDefense;

    private int currentLife;
    private int currentSpeed;
    private int currentElusion;
    private int currentAttack;
    private int currentDefense;

    private bool isAsleep = false;
    private bool isBurned = false;
    private bool isPoisoned = false;
    private bool isDead = false;

    public int StartingSpeed { get => startingSpeed;  }
    public int StartingLife { get => startingLife;  }
    public int StartingElusion { get => startingElusion;  }
    public int StartingAttack { get => startingAttack; }
    public int StartingDefense { get => startingDefense; }

    public int CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
    public int CurrentLife { get => currentLife; set => currentLife = value; }
    public int CurrentElusion { get => currentElusion; set => currentElusion = value; }
    public int CurrentAttack { get => currentAttack; set => currentAttack = value; }
    public int CurrentDefense { get => currentDefense; set => currentDefense = value; }

    public void Init(LinkemonScriptable ls)
    {
        linkemonName = ls.name;

        startingLife = ls.startingLife;
        currentLife = startingLife;

        startingSpeed = ls.startingSpeed;
        currentSpeed = startingSpeed;

        startingElusion = 0;
        currentElusion = 0;

        startingAttack = ls.startingAttack;
        currentAttack = startingAttack;

        startingDefense = ls.startingDefense;
        currentDefense = startingDefense;

        battleIcon = ls.battleIcon;
        lType = ls.lType;
        weaknessTypes = ls.weaknessTypes;
        linkemonVerse = ls.verse;
        attackList = ls.attacks;

        if (lAnimator == null)
            GetComponent<Animator>();

        if (battleImageUI != null)
        {
            battleImageUI.gameObject.SetActive(false);
            if (battleImageUI.sprite == null)
                battleImageUI.sprite = battleIcon;
        }

        if (healthBarUI != null)
            healthBarUI.SetActive(false);
        healthBarColor = healthBarUIFill.color;


        nameUI.GetComponent<TextMeshProUGUI>().text = linkemonName;
        typeUI.GetComponent<TextMeshProUGUI>().text = lType.ToString();
        lifeNumUI.GetComponent<TextMeshProUGUI>().text = currentLife.ToString() + "/" + startingLife.ToString();
    }

    public (int,int) GetStatByGenre(LinkemonAttack.LinkemonAttackGenre atkGenre)
    {
        int retStarting = 0;
        int retCurrent = 0;
        switch (atkGenre)
        {
            case LinkemonAttackGenre.Attack:
            case LinkemonAttackGenre.OpponentAttack:
                retStarting = startingAttack;
                retCurrent = currentAttack;
                break;
            case LinkemonAttackGenre.Defense:
            case LinkemonAttackGenre.OpponentDefense:
                retStarting = startingDefense;
                retCurrent = currentDefense;
                break;
            case LinkemonAttackGenre.Speed:
            case LinkemonAttackGenre.OpponentSpeed:
                retStarting = startingSpeed;
                retCurrent = currentSpeed;
                break;
            case LinkemonAttackGenre.Elusion:
            case LinkemonAttackGenre.OpponentElusion:
                retStarting = startingElusion;
                retCurrent = currentElusion;
                break;
        }

        return (retStarting,retCurrent);
    }

    public void OnEnterBattle()
    {
        battleImageUI.gameObject.SetActive(true);
        healthBarUI.SetActive(true);
        currentElusion = 0;
        currentSpeed = startingSpeed;

        //Da togliere se si vuole fare poi più elaborato con pozioni, items etc.
        currentLife = startingLife;

        lAnimator.Play("Idle");

        if (linkemonVerse != null)
            SoundManager.instance.PlayAudio(linkemonVerse);
    }

    //Spostato in battlemanager
    public void Attack(Linkemon defender, int attackIndex)
    {
        LinkemonType defType = defender.lType;

        LinkemonAttack attack = attackList[attackIndex];
        LinkemonAttack.LinkemonAttackGenre genre = attack.attackGenre;
        LinkemonType type = attack.attackType;

        if (genre == LinkemonAttack.LinkemonAttackGenre.Damage)
        {
            //DMG Calculation
            int dmg = attack.value;

            if (defType == type) //Non molto efficace...
                dmg /= 2;
            else if (defender.weaknessTypes.Contains(type)) //Superefficace!
                dmg *= 2;

            //Critical Hit doubling
            int num = Random.Range(0, 20);
            if (num == 5)
                dmg *= 2;

            defender.ReceiveDamage(dmg);
        }
    }

    public void RestoreHealth(int hp)
    {
        currentLife += hp;
        if (currentLife > startingLife) currentLife = startingLife;

        float value = (float)currentLife / startingLife;
        Debug.Log("Value of healthbar " + value);
        Debug.Log("Life of " + linkemonName + ": " + currentLife);
        healthBarUI.GetComponent<Slider>().value = value;
        healthBarUIFill.color = (float)currentLife / startingLife > .5f ? healthBarColor : (float)currentLife / startingLife > .2f ? Color.yellow : Color.red;
        lifeNumUI.GetComponent<TextMeshProUGUI>().text = currentLife.ToString() + "/" + startingLife.ToString();
    }

    public void ReceiveDamage(int dmg)
    {
        if(dmg == 0) dmg=1;
        currentLife -= dmg;
        if (currentLife < 0) currentLife = 0;

        float value = (float)currentLife / startingLife;
        Debug.Log("Value of healthbar " + value);
        Debug.Log("Life of " + linkemonName + ": " + currentLife);
        healthBarUI.GetComponent<Slider>().value = value;
        healthBarUIFill.color = (float)currentLife / startingLife > .5f? healthBarColor : (float)currentLife / startingLife > .2f ? Color.yellow : Color.red;
        lifeNumUI.GetComponent<TextMeshProUGUI>().text = currentLife.ToString() + "/" + startingLife.ToString();
    }
    public void OnDead()
    {
        //TODO: ...
        lAnimator.SetBool("Dead", true);
    }

    public void TotalRecharge()
    {
        currentLife = startingLife;
        isAsleep = false;
        isBurned = false;
        isPoisoned = false;
    }

    public void MirrorLinkemonIconGroup()
    {
        Vector3 currentScale = battleImageUI.rectTransform.localScale;
        battleImageUI.rectTransform.localScale = new Vector3(- currentScale.x, currentScale.y, currentScale.z);
    }

}
