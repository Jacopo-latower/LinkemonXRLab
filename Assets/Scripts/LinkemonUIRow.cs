using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LinkemonUIRow : MonoBehaviour
{
    public GameObject linkemon;
    public TextMeshProUGUI lName;
    public Image lIcon;
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

    public void OnClick()
    {
        StartCoroutine(BattleManager.instance.ChangeLinkemonAction(linkemon.GetComponent<Linkemon>()));
    }

}
