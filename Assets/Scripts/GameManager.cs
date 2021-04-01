using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public float rescuedFriends;

    private void Awake()
    {
        Instance = this;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
