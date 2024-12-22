using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> attacksUIButtons;
    [SerializeField] private GameObject linkemonMenuButton;
    [SerializeField] private GameObject backpackMenuButton;

    Linkemon currentLinkemon;
    private void Start()
    {
        currentLinkemon = BattleManager.instance.currentPlayerLinkemon;
    }

    public void OnChangeLinkemon(Linkemon linkemon)
    {
        List<LinkemonAttack> atks = linkemon.attackList;
        int i = 0;
        foreach (LinkemonAttack at in atks)
        {
            int capturedIndex = i; // Crea una copia locale
            attacksUIButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = at.attackName;
            attacksUIButtons[i].GetComponent<Button>().onClick.AddListener(delegate { OnPlayerAttack(capturedIndex); });
            Debug.Log(i);
            i++;
        }
    }

    public void OnPlayerAttack(int attackIndex)
    {
        //UI On Click
        Debug.Log(attackIndex);
        BattleManager.instance.OnPlayerAttack(attackIndex);
        
    }
}
