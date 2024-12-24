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
    bool antiSpamFlag = false;
    private void Start()
    {
        currentLinkemon = BattleManager.instance.currentPlayerLinkemon;
        backpackMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ricarica Tot (ancora " + BattleManager.instance.CurrentRicaricaTot + ")";
    }

    public void OnChangeLinkemon(Linkemon linkemon)
    {
        List<LinkemonAttack> atks = linkemon.attackList;
        int i = 0;
        foreach (LinkemonAttack at in atks)
        {
            int capturedIndex = i; // Crea una copia locale
            attacksUIButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = at.attackName + " (" + linkemon.CurrentPpPerAttack[i] + "/" + at.ppValue +")";
            attacksUIButtons[i].GetComponent<Button>().onClick.AddListener(delegate { OnPlayerAttack(capturedIndex); });
            Debug.Log(i);
            i++;
        }
        currentLinkemon = linkemon;
    }

    public void OnMovePPChange(int index)
    {
        attacksUIButtons[index].GetComponentInChildren<TextMeshProUGUI>().text = currentLinkemon.attackList[index].attackName + 
            " (" + currentLinkemon.CurrentPpPerAttack[index] + "/" + currentLinkemon.attackList[index].ppValue + ")";
    }

    public void OnTotalRecharge()
    {
        BattleManager.instance.OnPlayerRicaricaTot();
        backpackMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ricarica Tot (ancora " + BattleManager.instance.CurrentRicaricaTot + ")";
    }

    public void OnPlayerAttack(int attackIndex)
    {
        //UI On Click
        Debug.Log("Attacking");
        BattleManager.instance.OnPlayerAttack(attackIndex);       
    }
}
