using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkemonTrainer : MonoBehaviour
{
    public GameObject LinkemonPrefab;
    public LinkemonScriptable rewardLinkemon;

    [SerializeField] private Sprite trainerIcon;
    [SerializeField] private string trainerName;
    [SerializeField] private GameObject exclamationMarkRef;
    [SerializeField] private List<LinkemonScriptable> startingLinkemonScriptables;
    [SerializeField] private bool isNPC = false;
    [SerializeField] private bool isFinalBoss = false;
    [SerializeField] private float detectingDistance = 10f;

    [TextArea(20, 30)]
    [SerializeField] private string dialogue;
    [TextArea(20, 30)]
    public string endBattledialogue;
    [TextArea(20, 30)]
    public string rewardDialogue;

    public List<Linkemon> currentInstantiatedLinkemons;
    public Transform linkemonListParent;

    public Sprite TrainerIcon { get => trainerIcon; }
    public string TrainerName { get => trainerName; }

    private bool defeated = false;
    private bool alreadyMetPlayer = false;

    public bool Defeated { get => defeated; }
    private void Start()
    {
        foreach(LinkemonScriptable lk in startingLinkemonScriptables)
        {
            GameObject linkemonInstance = Instantiate(LinkemonPrefab, linkemonListParent);
            Linkemon lkComponent = linkemonInstance.GetComponent<Linkemon>();
            lkComponent.Init(lk);
            lkComponent.Trainer = this;
            currentInstantiatedLinkemons.Add(linkemonInstance.GetComponent<Linkemon>());
        }
    }

    private void Update()
    {
        if (isNPC && !defeated && !alreadyMetPlayer)
        {
            if (CheckForPlayer())
            {
                //TODO: move closer to player
                //Start Battle
                Debug.Log("BATTLE START!");
                alreadyMetPlayer = true;
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<PlayerController2D>().CanMove = false;
                StartCoroutine(StartBattle(player));

            }
        }
    }

    public void AddLinkemon(LinkemonScriptable lk)
    {
        GameObject linkemonInstance = Instantiate(LinkemonPrefab, linkemonListParent);
        Linkemon lkComponent = linkemonInstance.GetComponent<Linkemon>();
        lkComponent.Init(lk);
        lkComponent.Trainer = this;
        currentInstantiatedLinkemons.Add(linkemonInstance.GetComponent<Linkemon>());
    }

    public void RemoveLinkemon(string name)
    {
        for (int i = 0; i < currentInstantiatedLinkemons.Count; i++)
        {
            if (currentInstantiatedLinkemons[i].linkemonName == name)
            {
                currentInstantiatedLinkemons.RemoveAt(i);
                break;
            }
        }
    }

    public void RevitalizeAllLinkemon()
    {
        foreach (Linkemon l in GetLinkemonList())
        {
            l.TotalRecharge();
        }
    }
    IEnumerator StartBattle(GameObject player)
    {
        exclamationMarkRef.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        exclamationMarkRef.SetActive(false);

        //Move towards player till they are close
        float distance = 100f;

        while (distance > 1f)
        {
            distance = Vector2.Distance(transform.position, player.transform.position);
            Vector2 dir = (transform.position - player.transform.position).normalized;
            if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                GetComponent<NPCMovement>().Move(-Mathf.Sign(dir.x), 0);
            else
                GetComponent<NPCMovement>().Move(0,- Mathf.Sign(dir.y));


            yield return null;
        }

        DialogueManager.instance.ShowMessage(dialogue);

        //wait user input
        while (!Input.GetKeyDown(KeyCode.F))
            yield return null;

        DialogueManager.instance.DestroyMessage();

        BattleManager.instance.StartBattle(this);
    }

    public void OnDefeat()
    {
        defeated = true;
        StartCoroutine(OnDefeatCoroutine());
    }

    IEnumerator OnDefeatCoroutine()
    {
        DialogueManager.instance.ShowMessage(rewardDialogue);
        while (!Input.GetKeyDown(KeyCode.F))
        {
            yield return null;
        }


        //We reward the player with a Linkemon and a ricarica tot
        LinkemonTrainer plTrainer = GameObject.FindGameObjectWithTag("Player").GetComponent<LinkemonTrainer>();

        if (rewardLinkemon != null)
        {
            plTrainer.AddLinkemon(rewardLinkemon);
            plTrainer.RevitalizeAllLinkemon();
            DialogueManager.instance.ShowMessage("Hai ottenuto " + rewardLinkemon.lkName + "!");
            yield return new WaitForSeconds(0.5f);
            while (!Input.GetKeyDown(KeyCode.F))
            {
                yield return null;
            }
        }
        
        DialogueManager.instance.DestroyMessage();
        if (isFinalBoss)
            GameManager.instance.ShowVictory();
        else
            plTrainer.GetComponent<PlayerController2D>().CanMove = true;
    }
    public List<Linkemon> GetLinkemonList()
    {
        return currentInstantiatedLinkemons;
    }

    private bool CheckForPlayer()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.down, detectingDistance);
        if (hit2D)
        {
            if(hit2D.collider.gameObject.CompareTag("Player"))
                return true;
        }
           

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, detectingDistance * Vector2.down);
    }

}
