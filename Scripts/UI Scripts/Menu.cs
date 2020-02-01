using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //Continues game from last saved level
    public void ContinueGame()
    {
        SceneManager.LoadScene("Main");
    }

    //Close the application
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
