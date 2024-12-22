using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject nameUI;
    [SerializeField] private GameObject typeUI;
    [SerializeField] private Animator lAnimator;
    [SerializeField] private AudioClip linkemonVerse;

    [Header("Stats")]
    public LinkemonType lType;
    public LinkemonType weakness; //anche questo fa schifo e dovrebbe stare dentro un fantomatico "LinkemonType" object che non esiste  ma troppo lungo
    public List<LinkemonAttack> attackList;
    public Dictionary<string, int> attacksMap; //Fa schifo ma è troppo lungo fare tutto bene

    private int startingLife;
    private int currentLife;
    private int currentSpeed;
    private int currentElusion;
    private bool isAsleep = false;
    private bool isBurned = false;
    private bool isPoisoned = false;
    private bool isDead = false;

    public int CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
    public int CurrentLife { get => currentLife; set => currentLife = value; }
    public int CurrentElusion { get => currentElusion; set => currentElusion = value; }

    public void Init(LinkemonScriptable ls)
    {
        linkemonName = ls.name;
        startingLife = ls.startingLife;
        currentLife = startingLife;
        currentSpeed = ls.startingSpeed;
        currentElusion = 0;
        battleIcon = ls.battleIcon;
        lType = ls.lType;
        weakness = ls.weaknessType;
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

       nameUI.GetComponent<TextMeshProUGUI>().text = linkemonName;
       typeUI.GetComponent<TextMeshProUGUI>().text = lType.ToString();
    }

    public void OnEnterBattle()
    {
        battleImageUI.gameObject.SetActive(true);
        healthBarUI.SetActive(true);

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
            else if (defender.weakness == type) //Superefficace!
                dmg *= 2;

            //Critical Hit doubling
            int num = Random.Range(0, 20);
            if (num == 5)
                dmg *= 2;

            defender.ReceiveDamage(dmg);
        }
    }

    public void ReceiveDamage(int dmg)
    {
        float value = currentLife / startingLife;
        healthBarUI.GetComponent<Slider>().value = value;
        currentLife -= dmg;
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
