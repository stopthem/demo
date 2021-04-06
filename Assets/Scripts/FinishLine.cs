using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public static FinishLine Instance;

    [HideInInspector] public bool playerPassed;

    [HideInInspector] public float howManyFriendsPassed;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (howManyFriendsPassed == GameManager.Instance.rescuedFriends && playerPassed)
        {
            UIHandler.Instance.ShowWinScreen(true);
        }
        if (PlayerController.Instance == null && !playerPassed)
        {
            UIHandler.Instance.ShowFailScreen(true);
        }
    }
}
