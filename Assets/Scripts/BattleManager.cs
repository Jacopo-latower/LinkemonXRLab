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
    public GameObject linkemonListContainerPl;

    [Header("Current Linkémon")]
    public Linkemon currentPlayerLinkemon;
    public Linkemon currentOpponentLinkemon;

    [Header("UI prefabs")]
    public GameObject linkeballSprite;
    public GameObject linkemonUIRowPrefab;

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
            Linkemon lk = player.GetComponent<LinkemonTrainer>().GetLinkemonList()[i];
            GameObject row = Instantiate(linkemonUIRowPrefab, linkemonListContainerPl.transform);
            row.GetComponent<LinkemonUIRow>().SetIcon(lk.battleIcon);
            row.GetComponent<LinkemonUIRow>().SetName(lk.linkemonName);
            row.GetComponent<LinkemonUIRow>().linkemon = lk.gameObject;
        }
       
        //Set Opponent Icon
        opponentContainer.GetComponentInChildren<Image>().sprite = opponent.GetComponent<LinkemonTrainer>().TrainerIcon;
        int oppoLmNum = opponent.GetLinkemonList().Count;
        for (int i = 0; i < oppoLmNum; i++)
        {
            Instantiate(linkeballSprite, linkemonNumberContainerOpp.transform);
            
        }
        currentOpponent = opponent.gameObject;

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
    public IEnumerator ChangeLinkemonAction(Linkemon lk)
    {
        linkemonListContainerPl.SetActive(false);
        if (currentPlayerLinkemon.linkemonName == lk.linkemonName)
            yield break;

        yield return StartCoroutine(ChangePlayerLinkemon(lk));
        yield return HandleBattle(currentOpponentLinkemon, Random.Range(0, 3), currentPlayerLinkemon);

    }
    //Brutto vero... Da cambiare nel tempo che così fa vomitare
    IEnumerator HandleBattle(Linkemon first, int firstAttackIndex, Linkemon second, int secondAttackIndex)
    {
        #region HANDLE ATTACK 1
        //Success Calc
        //TODO:
        bool success = CalculateSuccess(first, firstAttackIndex, second);
        DialogueManager.instance.ShowMessage(first.linkemonName + " usa " + first.attackList[firstAttackIndex].attackName + "!");

        yield return new WaitForSeconds(1.5f);
        if (success)
        {
            int strikes = 1;
            int hit = 0;
            if (first.attackList[firstAttackIndex].strikesNum > 1)
            {
                strikes = Random.Range(1, first.attackList[firstAttackIndex].strikesNum);

                DialogueManager.instance.ShowMessage(first.attackList[firstAttackIndex].attackName + " colpisce per " + strikes + " volte.");
                
                yield return new WaitForSeconds(1.5f);
            }

            while (hit < strikes)
            {
                //AttackHandling
                yield return StartCoroutine(AttackHandling(first, second, firstAttackIndex));
                if (CheckOpponentLinkemon()) hit = strikes;
                yield return new WaitForSeconds(.5f);
                hit++;
            }
        }
        else
        {
            DialogueManager.instance.ShowMessage(first.linkemonName + " fallisce!");
            yield return new WaitForSeconds(1.5f);
        }
        #endregion

        #region CHECK LINKEMON
        //Check Oppo Linkemon state: if dead, change linkemon and exit
        if (CheckOpponentLinkemon())
        {
            DialogueManager.instance.ShowMessage(currentOpponentLinkemon.linkemonName + " è esausto!");
            currentOpponentLinkemon.OnDead();
            yield return new WaitForSeconds(1f);
            currentOpponentLinkemon.transform.SetParent(currentOpponent.GetComponent<LinkemonTrainer>().linkemonListParent);
            currentOpponentLinkemon = null;
            List<Linkemon> list = currentOpponent.GetComponent<LinkemonTrainer>().GetLinkemonList();
            foreach(Linkemon l in list)
            {
                if (l.CurrentLife > 0)
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

        yield return new WaitForSeconds(1f);

        #region HANDLE ATTACK 2
        DialogueManager.instance.ShowMessage(second.linkemonName + " usa " + second.attackList[secondAttackIndex].attackName + "!");
        yield return new WaitForSeconds(1.5f);
        bool success2 = CalculateSuccess(second, secondAttackIndex, first);
        if (success2)
        {
            int strikes = 1;
            int hit = 0;
            if (second.attackList[secondAttackIndex].strikesNum > 1)
            {
                strikes = Random.Range(1, second.attackList[secondAttackIndex].strikesNum);

                DialogueManager.instance.ShowMessage(second.attackList[secondAttackIndex].attackName + " colpisce per " + strikes + " volte.");

                yield return new WaitForSeconds(1.5f);
            }

            while (hit < strikes)
            {
                //AttackHandling
                yield return StartCoroutine(AttackHandling(second, first, secondAttackIndex));
                if (CheckOpponentLinkemon()) hit = strikes;
                yield return new WaitForSeconds(.5f);
                hit++;
            }
        }
        else
        {
            DialogueManager.instance.ShowMessage(second.linkemonName + " fallisce! ");
            yield return new WaitForSeconds(1.5f);
        }
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
        Debug.Log("Turn End");
        DialogueManager.instance.DestroyMessage();
        attacksMenu.SetActive(true);
    }

    //IF Player does not attack, only the opponent attacks
    IEnumerator HandleBattle(Linkemon first, int firstAttackIndex, Linkemon second)
    {
        #region HANDLE ATTACK 1
        //Success Calc
        //TODO:
        bool success = CalculateSuccess(first, firstAttackIndex, second);
        DialogueManager.instance.ShowMessage(first.linkemonName + " usa " + first.attackList[firstAttackIndex].attackName + "!");
        yield return new WaitForSeconds(1f);
        if (success)
        {
            //AttackHandling
            yield return StartCoroutine(AttackHandling(first, second, firstAttackIndex));
        }
        else
        {
            DialogueManager.instance.ShowMessage(first.linkemonName + " fallisce!");
            yield return new WaitForSeconds(1.5f);
        }
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
                if (l.CurrentLife > 0)
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

        yield return new WaitForSeconds(0.7f);
        

        //Keep going with the next turn
        Debug.Log("Turn End");
        DialogueManager.instance.DestroyMessage();
        attacksMenu.SetActive(true);
    }

    bool CalculateSuccess(Linkemon first, int atkIndex, Linkemon second)
    {
        int elusion = second.CurrentElusion;
        LinkemonAttack atk = first.attackList[atkIndex];

        float prob = atk.successValue - ((float)elusion / 100f);

        float random = Random.Range(0f, 1f);

        return random < prob;
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

        switch (genre)
        {
            case LinkemonAttack.LinkemonAttackGenre.Damage:
                //Animation
                if (attacker.Trainer.name == "Player")
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

                string message = "";
                string message2 = "";

                if (defType == type)
                { //Non molto efficace...
                    dmg /= 2;
                    message = "Non è molto efficace...";
                }
                else if (defender.weakness == type)
                { //Superefficace!
                    dmg *= 2;
                    message = "È superefficace!";
                }

                //Critical Hit doubling
                int num = Random.Range(0, 20);
                if (num == 5)
                {
                    message2 = "Brutto Colpo!";
                    dmg *= 2;
                }


                dmg *= attacker.CurrentAttack/defender.CurrentDefense;

                defender.ReceiveDamage(dmg);

                if (message != "")
                {
                    DialogueManager.instance.ShowMessage(message);
                    yield return new WaitForSeconds(1.5f);
                }

                if (message2 != "")
                {
                    DialogueManager.instance.ShowMessage(message2);
                    yield return new WaitForSeconds(1.5f);
                }
            break;

            case LinkemonAttack.LinkemonAttackGenre.Attack:
                attacker.CurrentAttack += attack.value;
                DialogueManager.instance.ShowMessage("Attacco di " + attacker.linkemonName + " aumenta!");
                yield return new WaitForSeconds(1.5f);
            break;

            case LinkemonAttack.LinkemonAttackGenre.Defense:
                attacker.CurrentDefense += attack.value;
                DialogueManager.instance.ShowMessage("Difesa di " + attacker.linkemonName + " aumenta!");
                yield return new WaitForSeconds(1.5f);
            break;

            case LinkemonAttack.LinkemonAttackGenre.Elusion:
                attacker.CurrentElusion += attack.value;
                DialogueManager.instance.ShowMessage("Elusione di " + attacker.linkemonName + " aumenta!");
                yield return new WaitForSeconds(1.5f);
            break;

            case LinkemonAttack.LinkemonAttackGenre.Speed:
                attacker.CurrentSpeed += attack.value;
                DialogueManager.instance.ShowMessage("Velocità di " + attacker.linkemonName + " aumenta!");
                yield return new WaitForSeconds(1.5f);
            break;

            case LinkemonAttack.LinkemonAttackGenre.OpponentAttack:
                defender.CurrentAttack -= attack.value;
                if (defender.CurrentAttack < 0) defender.CurrentAttack = 1;
                DialogueManager.instance.ShowMessage("Attacco di " + defender.linkemonName + " cala!");
                yield return new WaitForSeconds(1.5f);
                break;

            case LinkemonAttack.LinkemonAttackGenre.OpponentDefense:
                defender.CurrentDefense -= attack.value;
                if (defender.CurrentDefense < 0) defender.CurrentDefense = 1;
                DialogueManager.instance.ShowMessage("Difesa di " + defender.linkemonName + " cala!");
                yield return new WaitForSeconds(1.5f);
                break;

            case LinkemonAttack.LinkemonAttackGenre.OpponentElusion:
                defender.CurrentElusion -= attack.value;
                if (defender.CurrentElusion < 0) defender.CurrentElusion = 1;
                DialogueManager.instance.ShowMessage("Elusione di " + defender.linkemonName + " cala!");
                yield return new WaitForSeconds(1.5f);
                break;

            case LinkemonAttack.LinkemonAttackGenre.OpponentSpeed:
                defender.CurrentSpeed -= attack.value;
                if (defender.CurrentSpeed < 0) defender.CurrentSpeed = 1;
                DialogueManager.instance.ShowMessage("Velocità di " + defender.linkemonName + " cala!");
                yield return new WaitForSeconds(1.5f);
                break;

        }
        Debug.Log("Attack Finished");
    }

    IEnumerator ChangePlayerLinkemon(Linkemon linkemon)
    {
        //Disappear current linkemon
        if (currentPlayerLinkemon != null)
        {
            //TODO:
            currentPlayerLinkemon.transform.SetParent(player.GetComponent<LinkemonTrainer>().linkemonListParent, false);
            DialogueManager.instance.ShowMessage("Rientra " + currentPlayerLinkemon.linkemonName + "!");
            currentPlayerLinkemon = null;
        }
        yield return new WaitForSeconds(0.5f);
        //Appear current Likemon
        DialogueManager.instance.ShowMessage("VAI! " + linkemon.linkemonName + "!");
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

    IEnumerator BattleEndSequence()
    {
        //Play Victory Music
        //TODO:
        DialogueManager.instance.ShowMessage(currentOpponent.GetComponent<LinkemonTrainer>().endBattledialogue);
        yield return StartCoroutine(FadeIn(opponentContainer.GetComponent<CanvasGroup>(), 1f));
        yield return new WaitForSeconds(1.5f);
        attacksMenu.SetActive(false);
        yield return StartCoroutine(FadeOut(battleFieldContainer.GetComponent<CanvasGroup>(), 1.5f));
        battleGroup.SetActive(false);
        linkemonNumberContainerPl.SetActive(true);
        linkemonNumberContainerOpp.SetActive(true);
        currentOpponent.GetComponent<LinkemonTrainer>().OnDefeat();
    }

    void OnPlayerVictory()
    {
        //TODO:reset everything and exit battle

        Debug.Log("PLAYER WINS!");
        StartCoroutine(BattleEndSequence());
    }
    void OnPlayerDefeat()
    {
        //TODO:game over
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
