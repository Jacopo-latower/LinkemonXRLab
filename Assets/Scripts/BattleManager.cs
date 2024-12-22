using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Linkemon;

public class BattleManager : MonoBehaviour
{
    #region SINGLETON
    public static BattleManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion
    [Header("UI Containers")]
    public GameObject messagePanel;
    public GameObject opponentContainer;
    public GameObject playerContainer;
    public GameObject opponentLinkemonContainer;
    public GameObject playerLinkemonContainer;
    public GameObject battleFieldContainer;
    public GameObject linkemonNumberContainerOpp;
    public GameObject linkemonNumberContainerPl;
    public GameObject linkeballContainerOpp;
    public GameObject linkeballContainerPl;
    public GameObject battleStartGlow;
    public GameObject battleGroup;
    public GameObject attacksMenu;

    [Header("Current Linkémon")]
    public Linkemon currentPlayerLinkemon;
    public Linkemon currentOpponentLinkemon;

    [Header("UI prefabs")]
    public GameObject linkeballSprite;

    [Header("Audios")]
    public AudioSource battleMusic;

    private GameObject player;
    private GameObject currentOpponent;
    private bool isStartingSequenceFinished = false;

    [Header("Debug")]
    public LinkemonTrainer testOppo;
    public void StartBattle(LinkemonTrainer opponent)
    {
        //Set Player Icon
        player = GameObject.FindGameObjectWithTag("Player");
        playerContainer.GetComponentInChildren<Image>().sprite = player.GetComponent<LinkemonTrainer>().TrainerIcon;
        //Set Number of Linkemon into the container (instatiate linkeball for each linkemon of the player)
        int plLmNum = player.GetComponent<LinkemonTrainer>().GetLinkemonList().Count;
        Debug.Log(plLmNum);
       for (int i = 0; i < plLmNum; i++)
        {
            Instantiate(linkeballSprite, linkemonNumberContainerPl.transform);
        }
       
        //Set Opponent Icon
        opponentContainer.GetComponentInChildren<Image>().sprite = opponent.GetComponent<LinkemonTrainer>().TrainerIcon;
        int oppoLmNum = opponent.GetLinkemonList().Count;
        for (int i = 0; i < oppoLmNum; i++)
        {
            Instantiate(linkeballSprite, linkemonNumberContainerOpp.transform);
        }
        currentOpponent = opponent.gameObject;
        //Start Music
        //TODO: starting sequence
        battleGroup.SetActive(true);
        StartCoroutine(BattleStartSequence());
    }

    public void OnPlayerAttack(int attackIndex)
    {
        //Hide Menu
        attacksMenu.SetActive(false);

        //Calculate Speed
        int totalPlayerSpeed = currentPlayerLinkemon.CurrentSpeed + currentPlayerLinkemon.attackList[attackIndex].attackSpeed;
        int totalOppoSpeed = currentOpponentLinkemon.CurrentSpeed + currentOpponentLinkemon.attackList[attackIndex].attackSpeed;

        if (totalOppoSpeed > totalPlayerSpeed) 
        {
            //OppoAttackFirst
            StartCoroutine(HandleBattle(currentOpponentLinkemon, Random.Range(0, 3), currentPlayerLinkemon, attackIndex));
        }
        else
        {
            //PlayerAttackFirst
            StartCoroutine(HandleBattle(currentPlayerLinkemon, attackIndex, currentOpponentLinkemon, Random.Range(0, 3)));
        }

    }

    //Brutto vero... Da cambiare nel tempo che così fa vomitare
    IEnumerator HandleBattle(Linkemon first, int firstAttackIndex, Linkemon second, int secondAttackIndex)
    {
        #region HANDLE ATTACK 1
        //Success Calc
        //TODO:

        DialogueManager.instance.ShowMessage(first.linkemonName + " usa " + first.attackList[firstAttackIndex].attackName + "!");

        //AttackHandling
        yield return StartCoroutine(AttackHandling(first, second, firstAttackIndex));
        #endregion

        #region CHECK LINKEMON
        //Check Oppo Linkemon state: if dead, change linkemon and exit
        if (CheckOpponentLinkemon())
        {
            DialogueManager.instance.ShowMessage(currentOpponentLinkemon.linkemonName + " è esausto!");
            currentOpponentLinkemon.OnDead();
            yield return new WaitForSeconds(0.5f);
            currentOpponentLinkemon.transform.SetParent(currentOpponent.GetComponent<LinkemonTrainer>().linkemonListParent);
            currentOpponentLinkemon = null;
            List<Linkemon> list = currentOpponent.GetComponent<LinkemonTrainer>().GetLinkemonList();
            foreach(Linkemon l in list)
            {
                if (l.CurrentLife >= 0)
                    yield return StartCoroutine(ChangeOpponentLinkemon(l));
                yield return null;
            }
            //Victory if there is not a linkemon available for the opponent
            if (currentOpponentLinkemon == null)
                OnPlayerVictory();

            yield break;
        }
        //Check Player Linkemon state: if dead, change linkemon and exit
        if (CheckPlayerLinkemon())
        {
            DialogueManager.instance.ShowMessage(currentPlayerLinkemon.linkemonName + " è esausto!");
            currentPlayerLinkemon.OnDead();
            yield return new WaitForSeconds(0.5f);
            currentPlayerLinkemon.transform.SetParent(player.GetComponent<LinkemonTrainer>().linkemonListParent);
            currentPlayerLinkemon = null;
            List<Linkemon> list = player.GetComponent<LinkemonTrainer>().GetLinkemonList();
            foreach (Linkemon l in list)
            {
                if (l.CurrentLife >= 0)
                    yield return StartCoroutine(ChangePlayerLinkemon(l));
                yield return null;
            }
            //Victory if there is not a linkemon available for the opponent
            if (currentPlayerLinkemon == null)
                OnPlayerDefeat();

            yield break;
        }
        #endregion

        #region HANDLE ATTACK 2
        DialogueManager.instance.ShowMessage(second.linkemonName + " usa " + second.attackList[secondAttackIndex].attackName + "!");

        //AttackHandling
        yield return StartCoroutine(AttackHandling(second, first, secondAttackIndex));
        #endregion

        #region CHECK LINKEMON
        //Check Oppo Linkemon state: if dead, change linkemon and exit
        if (CheckOpponentLinkemon())
        {
            DialogueManager.instance.ShowMessage(currentOpponentLinkemon.linkemonName + " è esausto!");
            currentOpponentLinkemon.OnDead();
            yield return new WaitForSeconds(0.5f);
            currentOpponentLinkemon.transform.SetParent(currentOpponent.GetComponent<LinkemonTrainer>().linkemonListParent);
            currentOpponentLinkemon = null;
            List<Linkemon> list = currentOpponent.GetComponent<LinkemonTrainer>().GetLinkemonList();
            foreach (Linkemon l in list)
            {
                if (l.CurrentLife >= 0)
                    yield return StartCoroutine(ChangeOpponentLinkemon(l));
                yield return null;
            }
            //Victory if there is not a linkemon available for the opponent
            if (currentOpponentLinkemon == null)
                OnPlayerVictory();

            yield break;
        }
        //Check Player Linkemon state: if dead, change linkemon and exit
        if (CheckPlayerLinkemon())
        {
            DialogueManager.instance.ShowMessage(currentPlayerLinkemon.linkemonName + " è esausto!");
            currentPlayerLinkemon.OnDead();
            yield return new WaitForSeconds(0.5f);
            currentPlayerLinkemon.transform.SetParent(player.GetComponent<LinkemonTrainer>().linkemonListParent);
            currentPlayerLinkemon = null;
            List<Linkemon> list = player.GetComponent<LinkemonTrainer>().GetLinkemonList();
            foreach (Linkemon l in list)
            {
                if (l.CurrentLife >= 0)
                    yield return StartCoroutine(ChangePlayerLinkemon(l));
                yield return null;
            }
            //Victory if there is not a linkemon available for the opponent
            if (currentPlayerLinkemon == null)
                OnPlayerDefeat();

            yield break;
        }
        #endregion

        //Keep going with the next turn
        DialogueManager.instance.DestroyMessage();
        attacksMenu.SetActive(true);
    }

    bool CheckPlayerLinkemon()
    {
        if (currentPlayerLinkemon.CurrentLife <= 0f)
            return true;
        else
            return false;
    }

    bool CheckOpponentLinkemon()
    {
        if (currentOpponentLinkemon.CurrentLife <= 0f)
            return true;
        else
            return false;
    }
    IEnumerator AttackHandling(Linkemon attacker, Linkemon defender, int attackIndex)
    {
        LinkemonType defType = defender.lType;

        LinkemonAttack attack = attacker.attackList[attackIndex];
        LinkemonAttack.LinkemonAttackGenre genre = attack.attackGenre;
        LinkemonType type = attack.attackType;

        if (genre == LinkemonAttack.LinkemonAttackGenre.Damage)
        {
            //Animation
            if(attacker.Trainer.name == "Player")
                attacker.GetComponent<Animator>().SetBool("PlayerAttack", true);
            else
                attacker.GetComponent<Animator>().SetBool("Attacking", true);
            yield return new WaitForSeconds(0.4f);
            if (attacker.Trainer.name == "Player")
                attacker.GetComponent<Animator>().SetBool("PlayerAttack", false);
            else
                attacker.GetComponent<Animator>().SetBool("Attacking", false);

            defender.GetComponent<Animator>().SetBool("Damage", true);
            yield return new WaitForSeconds(0.4f);
            defender.GetComponent<Animator>().SetBool("Damage", false);
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


    void OnPlayerVictory()
    {
        //TODO:reset everything and exit battle
    }
    void OnPlayerDefeat()
    {
        //TODO:game over
    }

    IEnumerator ChangePlayerLinkemon(Linkemon linkemon)
    {
        //Disappear current linkemon
        if (currentPlayerLinkemon != null)
        {
            //TODO:
            currentPlayerLinkemon.transform.SetParent(player.GetComponent<LinkemonTrainer>().linkemonListParent, false);
            currentPlayerLinkemon = null;
        }
        //Appear current Likemon
        linkeballContainerPl.SetActive(true);
        yield return new WaitForSeconds(1f);
        linkeballContainerPl.SetActive(false);

        playerLinkemonContainer.SetActive(true);
        //linkemon.gameObject.transform.parent = playerLinkemonContainer.transform;
        linkemon.gameObject.transform.SetParent(playerLinkemonContainer.transform, false);
        linkemon.OnEnterBattle();

        attacksMenu.GetComponent<BattleMenu>().OnChangeLinkemon(linkemon);

        //Non abbiamo l'icona di spalle dei linkemon perchè troppa sbatta, quindi mirroriamo e ciao
        linkemon.MirrorLinkemonIconGroup();
        currentPlayerLinkemon = linkemon;
    }

    IEnumerator ChangeOpponentLinkemon(Linkemon linkemon)
    {
        //Disappear current linkemon
        if (currentOpponentLinkemon != null)
        {
            //TODO:
            currentOpponentLinkemon.transform.SetParent(currentOpponent.GetComponent<LinkemonTrainer>().linkemonListParent, false);
            currentOpponentLinkemon = null;
        }
        //Appear current Likemon
        linkeballContainerOpp.SetActive(true);
        yield return new WaitForSeconds(1f);
        linkeballContainerOpp.SetActive(false);

        opponentLinkemonContainer.SetActive(true);
        //linkemon.gameObject.transform.parent = opponentLinkemonContainer.transform;
        linkemon.gameObject.transform.SetParent(opponentLinkemonContainer.transform, false);
        linkemon.OnEnterBattle();
        currentOpponentLinkemon = linkemon;
    }

    public void SendUIMessage(string message)
    {
        if (messagePanel == null)
        {
            Debug.Log("Message Panel NULL!");
            return;
        }

        StartCoroutine(SpawnMessageUIForTime(message, 2f));
    }

    IEnumerator BattleStartSequence()
    {
        SoundManager.instance.PlayMusic(battleMusic);

        battleStartGlow.SetActive(true);
        yield return new WaitForSeconds(2f);

        //Message
        DialogueManager.instance.ShowMessage(currentOpponent.GetComponent<LinkemonTrainer>().TrainerName + " vuole combattere!");

        //battlefield appear
        yield return StartCoroutine(FadeIn(battleFieldContainer.GetComponent<CanvasGroup>(), 1.5f));

        //TODO: change this with some animation
        yield return new WaitForSeconds(2f);
        battleStartGlow.SetActive(false);
        //Fade out oppo icon
        yield return StartCoroutine(FadeOut(opponentContainer.GetComponent<CanvasGroup>(), 1f));
        //Send in first linkemon
        DialogueManager.instance.ShowMessage(currentOpponent.GetComponent<LinkemonTrainer>().GetLinkemonList()[0].linkemonName + " è stato scelto da " + currentOpponent.GetComponent<LinkemonTrainer>().TrainerName + "!");
        yield return StartCoroutine(ChangeOpponentLinkemon(currentOpponent.GetComponent<LinkemonTrainer>().GetLinkemonList()[0]));

        linkemonNumberContainerOpp.SetActive(false);

        yield return new WaitForSeconds(2.5f);

        //Fade out player icon
        yield return StartCoroutine(FadeOut(playerContainer.GetComponent<CanvasGroup>(), 1f));
        //Send in first linkemon
        DialogueManager.instance.ShowMessage("VAI! " + player.GetComponent<LinkemonTrainer>().GetLinkemonList()[0].linkemonName + "!");
        yield return StartCoroutine(ChangePlayerLinkemon(player.GetComponent<LinkemonTrainer>().GetLinkemonList()[0]));

        linkemonNumberContainerPl.SetActive(false);
        DialogueManager.instance.DestroyMessage();
        attacksMenu.SetActive(true);

        isStartingSequenceFinished = true;
    }

    IEnumerator SpawnMessageUIForTime(string text, float time)
    {
        messagePanel.SetActive(true);
        messagePanel.GetComponent<UIMessage>().SetText(text);
        yield return new WaitForSeconds(time);
        messagePanel.SetActive(false);
    }
    IEnumerator FadeIn(CanvasGroup group, float duration)
    {
        float elapsedTime = 0f;
        float startingAlpha = 0f;
        while (elapsedTime < duration)
        {
            group.alpha = Mathf.Lerp(startingAlpha, 1f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator FadeOut(CanvasGroup group, float duration)
    {
        float elapsedTime = 0f;
        float startingAlpha = group.alpha;
        while (elapsedTime < duration)
        {
            group.alpha = Mathf.Lerp(startingAlpha, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
