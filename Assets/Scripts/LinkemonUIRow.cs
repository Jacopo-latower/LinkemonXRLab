using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LinkemonUIRow : MonoBehaviour
{
    private TextMeshProUGUI lName;
    private Image lIcon;
    private TMP_InputField lOrder;

    public string LOrder { get => lOrder.text; set => lOrder.text = value; }

    public void SetName(string name)
    {
        lName.text = name;
    }

    public void SetIcon(Sprite sprite)
    {
        lIcon.sprite = sprite;
    }

}
