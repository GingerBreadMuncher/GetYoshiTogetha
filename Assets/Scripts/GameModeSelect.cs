using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeSelect : MonoBehaviour
{

    [SerializeField] GameObject chooseYoshiScreen;
    [SerializeField] GameObject gameModeSelectScreen;
    [SerializeField] GameManager gameManager;

    public void CountdownMode()
    {
        gameManager.CountdownMode = true;
        CasualMode();
    }

    public void CasualMode()
    {
        gameModeSelectScreen.SetActive(false);
        chooseYoshiScreen.SetActive(true);
    }
}
