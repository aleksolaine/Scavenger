using UnityEngine;

//Loads with the camera of the main menu scene
public class ApplicationInitializer : MonoBehaviour
{
    public GameObject achievementManager;
    private void Awake()
    {
        //Ask the SaveManager to initialize all our data
        //This may mean creating new files if they don't exist, or if they do exist, load their data into the game.
        SaveManager.InitializeData();
    }
}