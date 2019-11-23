using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{

    public void startGame()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("levelNo"));
    }
    public void quitGame()
    {
        Application.Quit();
    }

}
