using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TriggerEvolution : MonoBehaviour
{
    [Header("UI Containers")] 
    public GameObject evolUI;
    public GameObject lk1NormUI;
    public GameObject lk2NormUI;
    public GameObject lk1EvolUI;
    public GameObject lk2EvolUI;
    public LinkemonScriptable linkemonToAdd;
    public LinkemonScriptable linkemonToRemove;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                StartCoroutine(Evol(collision.gameObject));
            }
        }

    }

    IEnumerator Evol(GameObject player)
    {

        DialogueManager.instance.ShowMessage("Cosa? ...");
        player.GetComponent<PlayerController2D>().CanMove = false;
        lk1NormUI.GetComponent<Image>().sprite = linkemonToRemove.battleIcon;
        lk2NormUI.GetComponent<Image>().sprite = linkemonToAdd.battleIcon;
        lk1EvolUI.GetComponent<Image>().sprite = linkemonToAdd.evolIcon;
        lk2EvolUI.GetComponent<Image>().sprite = linkemonToAdd.evolIcon;
        yield return new WaitForSeconds(2f);

        player.GetComponent<LinkemonTrainer>().RemoveLinkemon(linkemonToRemove.lkName);
        player.GetComponent<LinkemonTrainer>().AddLinkemon(linkemonToAdd);
        evolUI.SetActive(true);
        evolUI.GetComponent<EvolutionUI>().StartEvolution(linkemonToRemove, linkemonToAdd);
        Destroy(this);
    }
}
