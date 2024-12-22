using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMessage : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    public void SetText(string text)
    {
        if(messageText!=null)
            messageText.text = text;
    }

}
