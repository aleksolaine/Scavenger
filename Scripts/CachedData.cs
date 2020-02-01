using System;

//In this file are cached all the variables that may be saved into files.
//The variables are loaded from files to here, and saved to files from here.
//Changes made to variables in this script won't save automatically.

//This class holds the Level data, which in practice is the progress made since last death.
[Serializable]
public class CachedLevelData
{
    public static CachedLevelData instance = null;
    public int savedFood;                                   //Player's foodpoints at the time of saving
    public int savedNextLevel;                              //The next level the player will enter
    public string savedItem;                                //If the player had an item equipped, save it's name and remaining uses
    public int savedUses;

    public CachedLevelData()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //By default, get food and next level from difficulty settings. Item is initialised with default values.
        savedFood = CachedDifficulty.instance.initialFoodPoints;
        savedNextLevel = CachedDifficulty.instance.startFromLevel;
        savedItem = "";
        savedUses = 0;
    }

    //Update is called when changes need to be made to progress. Progress is fetched from the current GameManager instance, given as a variable.
    public void Update(GameManager gameManager)
    {
        savedFood = gameManager.playerFoodPoints;
        savedNextLevel = gameManager.level + 1;
        savedItem = gameManager.heldItemInfo.Key;
        savedUses = gameManager.heldItemInfo.Value;
    }

    //Reset returns progress to default values.
    public void Reset()
    {
        savedFood = CachedDifficulty.instance.initialFoodPoints;
        savedNextLevel = CachedDifficulty.instance.startFromLevel;
        savedItem = "";
        savedUses = 0;
    }
}

//This class holds the collectibles that have been found
public class CachedCollectibles
{
    public static CachedCollectibles instance = null;
    public bool[] collectiblesFound;                    //An array of booleans, each index matching the index of a collectible. True if collectible in question has been found.

    public CachedCollectibles()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //By default, all are false.
        collectiblesFound = new bool[12];
    }

    //Found is called when the player finds a collectible. Changes that collectible's matching boolean to True.
    public void Found(int collectibleIndex)
    {
        collectiblesFound[collectibleIndex] = true;
    }

    //Reset resets all values back to false.
    public void Reset()
    {
        collectiblesFound = new bool[12];
    }
}

//This class holds all the difficulty settings.
[Serializable]
public class CachedDifficulty
{
    public static CachedDifficulty instance = null;

    public int difficultyIndex;                     //Preset difficulty. 0 = Easy, 1 = Normal, 2 = Hard, 3 = Custom

    public int startFromLevel;                      //Level from which a new game starts
    public int initialFoodPoints;                   //How many foodpoints player starts with

    public float levelGrowthFactor;                 //How fast the levels grow as game progresses
    public float wallSpawnFactor;                   //How many walls get spawned on a map
    public float foodSpawnFactor;                   //How many food tiles get spawned on a map
    public float itemSpawnFactor;                   //How many items get spawned on a map
    public float enemySpawnFactor;                  //How many enemies get spawned on a map

    public int bombSpawn;                           //The day from which bombs start spawning
    public int crossbowSpawn;                       //The day from which crossbows start spawning
    public int pistolSpawn;                         //The day from which pistols start spawning
    public int rifleSpawn;                          //The day from which rifles start spawning
    public int enemy1Spawn;                         //The day from which tier 1 enemies start spawning
    public int enemy2Spawn;                         //The day from which tier 2 enemies start spawning
    public int enemy3Spawn;                         //The day from which tier 3 enemies start spawning

    public float enemyWaitTimeModifier;             //How long enemies wait between actions
    public float enemyPlayerDamageModifier;         //How much damage enemies do to player per hit
    public float enemyWallDamageModifier;           //How much damage enemies do to walls per hit
    public float hpModifier;                        //How much hp enemies have

    public float playerWaitTimeModifier;            //How long player has to wait between actions
    public float foodIntakeModifier;                //How much food player gets from picking up food items
    public float playerEnemyDamageModifier;         //How much damage player does to enemies per hit
    public float playerWallDamageModifier;          //How much damage player does to walls per hit
    public float foodDepletionTime;                 //How long it takes for one unit of food to deplete over time

    //By default, initialise all variables to match the normal difficulty
    public CachedDifficulty()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        Update(1);
    }

    //Constructor that accepts every variable individually.
    public void Update(int _difficultyIndex,
        int _startFromLevel, int _initialFoodPoints,
        float _levelGrowthFactor, float _wallSpawnFactor, float _foodSpawnFactor, float _itemSpawnFactor, float _enemySpawnFactor,
        int _bombSpawn, int _crossbowSpawn, int _pistolSpawn, int _rifleSpawn, int _enemy1Spawn, int _enemy2Spawn, int _enemy3Spawn,
        float _enemyWaitTimeModifier, float _enemyPlayerDamageModifier, float _enemyWallDamageModifier, float _hpModifier,
        float _playerWaitTimeModifier, float _foodIntakeModifier, float _playerEnemyDamageModifier, float _playerWallDamageModifier, float _foodDepletionTime)
    {
        difficultyIndex = _difficultyIndex;

        startFromLevel = _startFromLevel;
        initialFoodPoints = _initialFoodPoints;

        levelGrowthFactor = _levelGrowthFactor;
        wallSpawnFactor = _wallSpawnFactor;
        foodSpawnFactor = _foodSpawnFactor;
        itemSpawnFactor = _itemSpawnFactor;
        enemySpawnFactor = _enemySpawnFactor;
        bombSpawn = _bombSpawn;
        crossbowSpawn = _crossbowSpawn;
        pistolSpawn = _pistolSpawn;
        rifleSpawn = _rifleSpawn;
        enemy1Spawn = _enemy1Spawn;
        enemy2Spawn = _enemy2Spawn;
        enemy3Spawn = _enemy3Spawn;

        enemyWaitTimeModifier = _enemyWaitTimeModifier;
        enemyPlayerDamageModifier = _enemyPlayerDamageModifier;
        enemyWallDamageModifier = _enemyWallDamageModifier;
        hpModifier = _hpModifier;

        playerWaitTimeModifier = _playerWaitTimeModifier;
        foodIntakeModifier = _foodIntakeModifier;
        playerEnemyDamageModifier = _playerEnemyDamageModifier;
        playerWallDamageModifier = _playerWallDamageModifier;
        foodDepletionTime = _foodDepletionTime;
    }
    
    //Constructor that adjust variables based on the given preset difficulty.
    public void Update(int difficultyIndex)
    {
        if (difficultyIndex == 0)
        {
            this.difficultyIndex = difficultyIndex;

            startFromLevel = 1;
            initialFoodPoints = 200;

            levelGrowthFactor = 0.5f;
            wallSpawnFactor = 1;
            foodSpawnFactor = 1.5f;
            itemSpawnFactor = 1.5f;
            enemySpawnFactor = 0.75f;
            bombSpawn = 1;
            crossbowSpawn = 1;
            pistolSpawn = 1;
            rifleSpawn = 1;
            enemy1Spawn = 1;
            enemy2Spawn = 10;
            enemy3Spawn = 20;

            enemyWaitTimeModifier = 2;
            enemyPlayerDamageModifier = 0.5f;
            enemyWallDamageModifier = 0.5f;
            hpModifier = 0.5f;

            playerWaitTimeModifier = 1;
            foodIntakeModifier = 2;
            playerEnemyDamageModifier = 2;
            playerWallDamageModifier = 2;
            foodDepletionTime = 2;
        }
        else if (difficultyIndex == 1)
        {
            this.difficultyIndex = difficultyIndex;

            startFromLevel = 1;
            initialFoodPoints = 100;

            levelGrowthFactor = 1;
            wallSpawnFactor = 1;
            foodSpawnFactor = 1;
            itemSpawnFactor = 1;
            enemySpawnFactor = 1;
            bombSpawn = 3;
            crossbowSpawn = 10;
            pistolSpawn = 20;
            rifleSpawn = 30;
            enemy1Spawn = 1;
            enemy2Spawn = 10;
            enemy3Spawn = 20;

            enemyWaitTimeModifier = 1;
            enemyPlayerDamageModifier = 1;
            enemyWallDamageModifier = 1;
            hpModifier = 1;

            playerWaitTimeModifier = 1;
            foodIntakeModifier = 1;
            playerEnemyDamageModifier = 1;
            playerWallDamageModifier = 1;
            foodDepletionTime = 1;
        }
        else if (difficultyIndex == 2)
        {
            this.difficultyIndex = difficultyIndex;

            startFromLevel = 1;
            initialFoodPoints = 100;

            levelGrowthFactor = 1.5f;
            wallSpawnFactor = 1;
            foodSpawnFactor = 1;
            itemSpawnFactor = 0.75f;
            enemySpawnFactor = 1;
            bombSpawn = 3;
            crossbowSpawn = 10;
            pistolSpawn = 20;
            rifleSpawn = 30;
            enemy1Spawn = 1;
            enemy2Spawn = 1;
            enemy3Spawn = 1;

            enemyWaitTimeModifier = 1;
            enemyPlayerDamageModifier = 1.5f;
            enemyWallDamageModifier = 1.5f;
            hpModifier = 1.5f;

            playerWaitTimeModifier = 1;
            foodIntakeModifier = 1;
            playerEnemyDamageModifier = 1;
            playerWallDamageModifier = 0.5f;
            foodDepletionTime = 0.75f;
        }
    }

    //Reset changes variables back to normal difficulty
    public void Reset()
    {
        Update(1);
    }
}