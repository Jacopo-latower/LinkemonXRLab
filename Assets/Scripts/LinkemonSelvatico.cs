using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkemonSelvatico : MonoBehaviour
{
    public GameObject LinkemonPrefab;

    [SerializeField] private LinkemonScriptable linkemonScriptable;

    public Transform linkemonParent;

    public Linkemon linkemon;
    private bool alreadyMetPlayer = false;

    [SerializeField] private LinkemonTrainer fakeTrainer;

    private void Start()
    {
        GameObject linkemonInstance = Instantiate(LinkemonPrefab, linkemonParent);
        Linkemon lkComponent = linkemonInstance.GetComponent<Linkemon>();
        lkComponent.Init(linkemonScriptable);
        linkemon = linkemonInstance.GetComponent<Linkemon>();
        linkemon.Trainer = fakeTrainer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player") && !alreadyMetPlayer)
            {
                Debug.Log("BATTLE START!");
                alreadyMetPlayer = true;

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<PlayerController2D>().CanMove = false;

                SoundManager.instance.PlayMusic(SoundManager.instance.battleTheme);

                BattleManager.instance.StartBattle(this);
            }
        }

    }


}
