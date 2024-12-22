using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    #region SINGLETON
    public static DialogueManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    #endregion

    public GameObject messageContainerPrefab;
    public GameObject canvasReference;

    private GameObject currentMessage;

    //FA CAGARE MA SERVE PER IL PROTOTIPO, Da cambiare poi in qualcosa di più decente
    public void ShowMessage(string text)
    {
        if (currentMessage != null)
            DestroyMessage();

        GameObject newMessage = Instantiate(messageContainerPrefab, canvasReference.transform);
        newMessage.GetComponent<UIMessage>().SetText(text);
        newMessage.SetActive(true);

        currentMessage = newMessage;
    }

    public void DestroyMessage()
    {
        Destroy(currentMessage);
    }
}
