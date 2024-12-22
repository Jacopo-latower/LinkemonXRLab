using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Linkemon : MonoBehaviour
{
    public enum LinkemonType { Dark, Psychic, Fire, Water, Electric, Grass, Flying, Metal, Fighting}

    public string linkemonName;
    public Sprite battleIcon;

    [Header("References")]
    [SerializeField] private Image battleImageUI;
    [SerializeField] private GameObject healthBarGroup;
    [SerializeField] private Animator lAnimator;
    [SerializeField] private AudioClip linkemonVerse;

    [Header("Stats")]
    public LinkemonType lType;
    public LinkemonType weakness; //anche questo fa schifo e dovrebbe stare dentro un fantomatico "LinkemonType" object che non esiste  ma troppo lungo
    public Dictionary<string, int> attacksMap; //Fa schifo ma è troppo lungo fare tutto bene

    private int startingLife;
    private int currentLife;
    private int currentSpeed;
    private bool isAsleep = false;
    private bool isBurned = false;
    private bool isPoisoned = false;
    private bool isDead = false;


    public void Init(LinkemonScriptable ls)
    {
        linkemonName = ls.name;
        startingLife = ls.startingLife;
        currentLife = startingLife;
        currentSpeed = ls.startingSpeed;
        battleIcon = ls.battleIcon;
        lType = ls.lType;
        weakness = ls.weaknessType;
        linkemonVerse = ls.verse;

        if (lAnimator == null)
            GetComponent<Animator>();

        if (battleImageUI != null)
        {
            battleImageUI.gameObject.SetActive(false);
            if (battleImageUI.sprite == null)
                battleImageUI.sprite = battleIcon;
        }

        if (healthBarGroup != null)
            healthBarGroup.SetActive(false);

        healthBarGroup.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = linkemonName;
        healthBarGroup.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = lType.ToString();
    }

    public void OnEnterBattle()
    {
        battleImageUI.gameObject.SetActive(true);
        healthBarGroup.SetActive(true);

        if (linkemonVerse != null)
            SoundManager.instance.PlayAudio(linkemonVerse);
    }

    public void Attack(string name, Linkemon defender)
    {
        LinkemonType defType = defender.lType;
        int dmg = attacksMap[name];

        if (defType == lType) //Non molto efficace...
            dmg /= 2;
        else if (defender.weakness == lType) //Superefficace!
            dmg *= 2;

        //Critical Hit doubling
        int num = Random.Range(0, 20);
        if (num == 5)
            dmg *= 2;

        defender.ReceiveDamage(dmg);
    }
    public void ReceiveDamage(int dmg)
    {
        currentLife -= dmg;
        if (currentLife <= 0)
            OnDead();
    }
    public void OnDead()
    {
        //TODO: ...
    }

    public void TotalRecharge()
    {
        currentLife = startingLife;
        isAsleep = false;
        isBurned = false;
        isPoisoned = false;
    }

}
