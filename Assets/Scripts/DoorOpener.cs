using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public LinkemonTrainer[] trainersInRoom;

    private bool playerInRange = false;
    private bool interactionFlag = false;
    private void Update()
    {
        GameObject pl = GameObject.FindGameObjectWithTag("Player");

        float distance = Vector3.Distance(pl.transform.position, transform.position);
        if (distance < 1f)
        {
            playerInRange = true;

            if(!interactionFlag)
                DialogueManager.instance.ShowMessage("Premi F per aprire");

            bool allDefeated = true;
            if (Input.GetKeyDown(KeyCode.F))
            {
                interactionFlag = true;
                foreach(LinkemonTrainer trainer in trainersInRoom)
                {
                    if (!trainer.Defeated)
                    {
                        DialogueManager.instance.ShowMessage("Devi sconfiggere tutti quelli nella stanza per passare!");
                        allDefeated = false;
                        break;
                    }
                }

                if (allDefeated)
                {
                    DialogueManager.instance.DestroyMessage();
                    Destroy(gameObject);
                }

            }
        }
        else
        {
            if(playerInRange)
                DialogueManager.instance.DestroyMessage();
            interactionFlag = false;
            playerInRange = false;
        }

    }

}
