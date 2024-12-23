using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvolution : MonoBehaviour
{
    public GameObject evolUI;
    public LinkemonScriptable linkemonToAdd;
    public LinkemonScriptable linkemonToRemove;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Evol(collision.gameObject));
        }

    }

    IEnumerator Evol(GameObject player)
    {
        DialogueManager.instance.ShowMessage("Cosa? ...");
        player.GetComponent<PlayerController2D>().CanMove = false;
        yield return new WaitForSeconds(2f);

        player.GetComponent<LinkemonTrainer>().RemoveLinkemon(linkemonToRemove.lkName);
        player.GetComponent<LinkemonTrainer>().AddLinkemon(linkemonToAdd);
        evolUI.SetActive(true);
        evolUI.GetComponent<EvolutionUI>().StartEvolution();
    }
}
