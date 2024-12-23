using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionUI : MonoBehaviour
{
    public Animator anim;

    public void StartEvolution()
    {
        StartCoroutine(EvolCoroutine());
    }

    IEnumerator EvolCoroutine()
    {
        DialogueManager.instance.ShowMessage("BITIDILE si sta evolvendo!");

        yield return new WaitForSeconds(2f);

        anim.SetBool("Start", true);

        yield return new WaitForSeconds(5f);

        DialogueManager.instance.ShowMessage("Bitidile si è evoluto in CROCOBITE!");

        yield return new WaitForSeconds(2f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>().CanMove = true;

        DialogueManager.instance.DestroyMessage();
        gameObject.SetActive(false);
    }
}
