using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    #endregion
    public GameObject victoryUI;

    public void ShowVictory()
    {
        if (victoryUI != null)
            victoryUI.SetActive(true);
        else
            Debug.Log("Victory UI null!");
    }
}
