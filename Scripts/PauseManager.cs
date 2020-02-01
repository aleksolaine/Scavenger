using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//PauseManager handles what happens when player presses the Esc-key during gameplay.
public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    //At start, make sure the UI element is hidden.
    void Start()
    {
        pausePanel.SetActive(false);
    }
    //Look for the esc-key press in update, and pause the game if it is pressed.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pausePanel.activeInHierarchy)
            {
                PauseGame();
            }
            else if (pausePanel.activeInHierarchy)
            {
                ContinueGame();
            }
        }
    }
    //Method for pausing the game.
    private void PauseGame()
    {
        //Tell GameManager the game is paused. Disables actions that wouldn't be disabled by setting timeScale to 0.
        GameManager.instance.pause = true;

        //Stop time from moving.
        Time.timeScale = 0;

        //Make the pause UI visible.
        pausePanel.SetActive(true);
    }

    //Method for unpausing.
    private void ContinueGame()
    {
        //Start time again
        Time.timeScale = 1;

        //Hide the pause UI
        pausePanel.SetActive(false);

        //Tell GameManager that game is no longer paused, allowing all scripts to function normally again.
        GameManager.instance.pause = false;
    }

    //Loads out of the game, into Pause Menu, and destroys the persistent Managers.
    public void ExitCurrentPlaythrough()
    {
        Destroy(GameObject.Find("GameManager(Clone)"));
        Destroy(GameObject.Find("SoundManager(Clone)"));
        SceneManager.LoadScene("Menu");
    }
}
