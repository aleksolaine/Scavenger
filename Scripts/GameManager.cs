using System.Collections.Generic;       
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;                   

public class GameManager : MonoBehaviour
{
    public float levelStartDelay;                           //Time to wait before starting level, in seconds.
    public int playerFoodPoints;                            //Starting value for Player food points.
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.                                            
    public GameObject[] items;                              //List of all usable items prefabs.
    public KeyValuePair<string,int> heldItemInfo;           //Holds player's equipped item in a string and it's remaining uses in an int.
    public bool pause = false;                              //Holds pause state of game.
    [HideInInspector] public Text ammoText;                 //Text that displays equipped item and it's remaining uses.
    public int level = 1;                                   //Current level number, expressed in game as "Day 1".
    public int FPStarget = 60;                              //FPS cap.

    private int recordLevel;                                //Variable for storing the highest level reached in current save.
    private Text levelText;                                 //Text to display current level number.
    private Text hintText;                                  //Text to display hints between levels.
    private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.
    private GameObject returnButton;                        //Button that appears after death and allows you to return to menu.
    private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
    private List<Enemy> enemies;                            //List of all Enemy units at level start.
    private bool firstLoad;                                 //Boolean that is true if this is the first load of a level for this instance of GameManager.
    private string[] hints = new string[17];                //Array of hints posted on screen between levels


    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Lock FPS to a certain value
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = FPStarget;

        //This is the first load of a level for this instance of GameManager.
        firstLoad = true;

        //Assign enemies to a new List of Enemy objects.
        enemies = new List<Enemy>();

        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        //Initialise a hint array
        InitHints();

        //Initialise first level from cached level data
            playerFoodPoints = CachedLevelData.instance.savedFood;
            level = CachedLevelData.instance.savedNextLevel;
            heldItemInfo = new KeyValuePair<string, int> ( CachedLevelData.instance.savedItem, CachedLevelData.instance.savedUses );

        //Initialise BoardManager's values based on difficulty settings.
        boardScript.InitialiseBoardManager();

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //Update is called every frame and makes sure the given target is maintained
    private void Update()
    {
        if (Application.targetFrameRate != FPStarget)
            Application.targetFrameRate = FPStarget;
    }

    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //Don't load a level if there is no instance of GameManager, or if this is the first load.
        if (instance == null || instance.firstLoad) return;
        instance.level++;
        instance.InitGame();
    }


    //Initializes the game for each level.
    void InitGame()
    {
        //Pause game while initialisation is under way.
        pause = true;

        //Get a reference to the text object that shows item information.
        ammoText = GameObject.Find("AmmoText").GetComponent<Text>();

        //Get a reference to our image LevelImage by finding it by name.
        levelImage = GameObject.Find("LevelImage");

        //Get a reference to the button used to return to menu from a death screen.
        returnButton = GameObject.Find("ReturnButton");

        //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //Get a reference to our text hintText's text component by finding it by name and calling GetComponent.
        hintText = GameObject.Find("HintText").GetComponent<Text>();

        //Initialise a random hint index.
        int randomHint = UnityEngine.Random.Range(0, hints.Length);

        //Set the text of levelText to the string "Day" and append the current level number.
        levelText.text = "Day " + level;

        //Find the random hint from the array and set it into HintText.
        hintText.text = hints[randomHint];

        //Hide the return button for now.
        returnButton.SetActive(false);

        //Set levelImage to active blocking player's view of the game board during setup.
        levelImage.SetActive(true);

        //Make sure time is moving forward.
        Time.timeScale = 1;

        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
        Invoke("HideLevelImage", levelStartDelay);

        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();

        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);

    }

    //Hides black image used between levels
    void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImage.SetActive(false);

        //Now we can be sure the following loads won't be firsts.
        firstLoad = false;

        //Unpause the game as level image gets hidden.
        pause = false;
    }

    //This is what every Enemy calls to sign themselves up. Returns their index for them to utilise.
    public int AddEnemyToList(Enemy script)
    {
        //Get the index that this enemy will get assigned.
        int index = enemies.Count;

        //Add Enemy to List enemies.
        enemies.Add(script);

        return index;
    }

    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        //Set levelText to display number of levels passed and game over message
        levelText.fontSize = 40;
        levelText.lineSpacing = 2;
        levelText.text = "You failed to protect your camp \n after " + level + " days.";

        //Don't show general hints after death.
        hintText.gameObject.SetActive(false);

        //Show the button that can be used to return to menu.
        returnButton.SetActive(true);

        //Enable black background image gameObject.
        levelImage.SetActive(true);
    }
   
    //Removes the given enemy from the list of enemies.
    public void RemoveEnemyFromList(Enemy enemy)
    {
        int enemyIndex = enemies.FindIndex(x => x.transform.position == enemy.transform.position);
        enemies.RemoveAt(enemyIndex);
    }

    //Getter for rows and colums on the game map. Used by the camera for its restrictions.
    public int[] RowsAndColums()
    {
        return new int[2] { boardScript.rows, boardScript.columns };
    }

    //Find if reached level is a record for this save.
    public void CheckRecord()
    {
        if (level > recordLevel)
        {
            recordLevel = level;
        }
    }

    //Initialise an array of hints to display
    private void InitHints()
    {
        hints[0] = ("Pick up the supply cache in each area to unlock exit");
        hints[1] = ("Food is depleted completing various actions, as well as over time");
        hints[2] = ("Pick up foodstuffs when scavenging to avoid starvation");
        hints[3] = ("Getting hit by baddies is the fastest way to starve");
        hints[4] = ("You can destroy a piece of wall by walking into it long enough");
        hints[5] = ("Collectibles only spawn on Normal and Hard difficulties");
        hints[6] = ("Bombs destroy a 3x3 area around them");
        hints[7] = ("It is strongly advisable to promptly relocate after dropping an explosive");
        hints[8] = ("Fire weapons using the arrow keys");
        hints[9] = ("Rifle shots will pass through enemies");
        hints[10] = ("Powerups only affect on the day they are collected");
        hints[11] = ("Powerups stack! Build yourself into an invulnerable one shot killer");
        hints[12] = ("Picking up a different weapon replaces your current one");
        hints[13] = ("You get more ammo for your weapon by picking up another of the same kind");
        hints[14] = ("Zombies like brains.");
        hints[15] = ("The strongest Zombies have brains and know how to use them");
        hints[16] = ("Zombies prefer to move horizontally");
    }
}


