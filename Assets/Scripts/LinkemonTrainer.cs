using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkemonTrainer : MonoBehaviour
{
    public GameObject LinkemonPrefab;

    [SerializeField] private Sprite trainerIcon;
    [SerializeField] private string trainerName;
    [SerializeField] private GameObject exclamationMarkRef;
    [SerializeField] private List<LinkemonScriptable> startingLinkemonScriptables;
    [SerializeField] private bool isNPC = false;

    [TextArea(20, 30)]
    [SerializeField] private string dialogue;

    public List<Linkemon> currentInstantiatedLinkemons;
    public Transform linkemonListParent;

    public Sprite TrainerIcon { get => trainerIcon; }
    public string TrainerName { get => trainerName; }

    private bool defeated = false;
    private bool alreadyMetPlayer = false;
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
    public List<Linkemon> GetLinkemonList()
    {
        return currentInstantiatedLinkemons;
    }

    private bool CheckForPlayer()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.down);
        if (hit2D.collider.gameObject.CompareTag("Player"))
            return true;

        return false;
    }


}
