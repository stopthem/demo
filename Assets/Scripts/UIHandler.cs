using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    public GameObject failedScreen;
    public GameObject winScreen;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowWinScreen(bool status)
    {
        winScreen.SetActive(status);
    }

    public void ShowFailScreen(bool status)
    {
        failedScreen.SetActive(status);
    }
}
