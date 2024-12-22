using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private bool isBattling = false;
    private void Update()
    {
        //Debug
       /* if (Input.GetKeyDown(KeyCode.F) && !isBattling)
        {
            isBattling = true;
            StartBattle(testOppo);
        }
       */
    }

    IEnumerator ChangePlayerLinkemon(Linkemon linkemon)
    {
        //Disappear current linkemon
        //Appear current Likemon
        linkeballContainerPl.SetActive(true);
        yield return new WaitForSeconds(1f);
        linkeballContainerPl.SetActive(false);

        playerLinkemonContainer.gameObject.SetActive(true);
        //linkemon.gameObject.transform.parent = playerLinkemonContainer.transform;
        linkemon.gameObject.transform.SetParent(playerLinkemonContainer.transform, false);
        linkemon.OnEnterBattle();

        currentPlayerLinkemon = linkemon;
    }

    IEnumerator ChangeOpponentLinkemon(Linkemon linkemon)
    {
        //Disappear current linkemon
        if (currentOpponentLinkemon != null)
        {
            //TODO:
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
        
        //battlefield appear
        yield return StartCoroutine(FadeIn(battleFieldContainer.GetComponent<CanvasGroup>(), 1.5f));

        //Message
        DialogueManager.instance.ShowMessage(currentOpponent.GetComponent<LinkemonTrainer>().TrainerName + " vuole combattere!");

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
