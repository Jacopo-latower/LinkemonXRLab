using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionUI : MonoBehaviour
{
    public Animator anim;

    public void StartEvolution(LinkemonScriptable l1, LinkemonScriptable l2)
    {
        StartCoroutine(EvolCoroutine(l1, l2));
    }

    IEnumerator EvolCoroutine(LinkemonScriptable l1, LinkemonScriptable l2)
    {
        DialogueManager.instance.ShowMessage( l1.lkName + " si sta evolvendo!");

        yield return new WaitForSeconds(2f);

        anim.SetBool("Start", true);

        yield return new WaitForSeconds(5f);

        DialogueManager.instance.ShowMessage(l1.lkName + " si è evoluto in " + l2.lkName + "!");

        yield return new WaitForSeconds(3f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>().CanMove = true;

        DialogueManager.instance.DestroyMessage();
        gameObject.SetActive(false);
    }
}
