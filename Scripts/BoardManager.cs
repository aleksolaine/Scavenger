using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class Count
    {
        public int minimum;             //Minimum value for our Count class.
        public int maximum;             //Maximum value for our Count class.


        //Assignment constructor.
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
    [HideInInspector] public int columns;                           //Number of columns in our game board.
    [HideInInspector] public int rows;                              //Number of rows in our game board.
    public Count wallCount;                                         //Lower and upper limit for our random number of walls per level.
    public Count foodCount;                                         //Lower and upper limit for our random number of food items per level.
    public Count itemCount;                                         //Lower and upper limit for our random number of items per level.
    public GameObject exit;                                         //Prefab for exit.
    public GameObject supplies;                                     //Prefab for supplies.
    public GameObject[] floorTiles;                                 //Array of floor prefabs.
    public GameObject[] wallTiles;                                  //Array of wall prefabs.
    public GameObject[] foodTiles;                                  //Array of food prefabs.
    public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
    public GameObject[] outerWallTiles;                             //Array of outer wall tile prefabs.
    public GameObject[] itemTiles;                                  //Array of item pickup prefabs.
    public GameObject[] collectibles;                               //Array of collectible prefabs.
    public GameObject[] powerups;                                   //Array of powerups.

    //DIFFICULTY FACTORS
    private int bombFirstSpawnLevel;
    private int crossbowFirstSpawnLevel;
    private int pistolFirstSpawnLevel;
    private int rifleFirstSpawnLevel;
    private int itemFirstSpawnLevel;
    private int enemy1FirstSpawnLevel;
    private int enemy2FirstSpawnLevel;
    private int enemy3FirstSpawnLevel;
    private int enemyFirstSpawnLevel;
    private float wallSpawnFactor;
    private float foodSpawnFactor;
    private float itemSpawnFactor;
    private float enemySpawnFactor;
    private float levelSizeGrowthRate;                              
    //DIFFICULTY FACTORS
    
    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
    private List<Vector3> gridPositions = new List<Vector3>();      //A list of possible locations to place tiles.
    private float areaRatio;                                        //The ratio of randomly generated area to the original 7x7 area.

    //Adjust variables according to the difficulty settings
    public void InitialiseBoardManager()
    {
        levelSizeGrowthRate = CachedDifficulty.instance.levelGrowthFactor * 0.25f;
        wallSpawnFactor = CachedDifficulty.instance.wallSpawnFactor;
        foodSpawnFactor = CachedDifficulty.instance.foodSpawnFactor;
        itemSpawnFactor = CachedDifficulty.instance.itemSpawnFactor;
        enemySpawnFactor = CachedDifficulty.instance.enemySpawnFactor;

        bombFirstSpawnLevel = CachedDifficulty.instance.bombSpawn;
        crossbowFirstSpawnLevel = CachedDifficulty.instance.crossbowSpawn;
        pistolFirstSpawnLevel = CachedDifficulty.instance.pistolSpawn;
        rifleFirstSpawnLevel = CachedDifficulty.instance.rifleSpawn;
        itemFirstSpawnLevel = Mathf.Min(bombFirstSpawnLevel, crossbowFirstSpawnLevel, pistolFirstSpawnLevel, rifleFirstSpawnLevel);

        enemy1FirstSpawnLevel = CachedDifficulty.instance.enemy1Spawn;
        enemy2FirstSpawnLevel = CachedDifficulty.instance.enemy2Spawn;
        enemy3FirstSpawnLevel = CachedDifficulty.instance.enemy3Spawn;
        enemyFirstSpawnLevel = Mathf.Min(enemy1FirstSpawnLevel, enemy2FirstSpawnLevel, enemy3FirstSpawnLevel);
    }

    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList()
    {
        //Clear our list gridPositions.
        gridPositions.Clear();

        //Loop through x axis (columns).
        for (int x = 0; x < columns; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = 0; y < rows; y++)
            {
                if ((x == 0 && y == 0) || (x == 1 && y == 0) || (x == 0 && y == 1) || (x == 1 && y == 1)) break;

                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    //Sets up the outer walls and floor (background) of the game board.
    void BoardSetup()
    {
        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;

        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for (int x = -1; x < columns + 1; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for (int y = -1; y < rows + 1; y++)
            {
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                GameObject instance =
                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition()
    {
        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range(0, gridPositions.Count);

        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = gridPositions[randomIndex];

        //Remove the entry at randomIndex from the list so that it can't be re-used.
        gridPositions.RemoveAt(randomIndex);

        //Return the randomly selected Vector3 position.
        return randomPosition;
    }

    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = Random.Range(minimum, maximum + 1);

        //Instantiate objects until the randomly chosen limit objectCount is reached
        int i = 0;
        while (i < objectCount)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Choose a random tile from tileArray and assign it to tileChoice
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            //If object to be instantiated is an item, make sure we are allowed to spawn that item on this level.
            if (tileChoice.tag == "Item")
            {
                if (tileChoice.name.Contains("Bomb") && bombFirstSpawnLevel > GameManager.instance.level) continue;
                else if (tileChoice.name.Contains("Crossbow") && crossbowFirstSpawnLevel > GameManager.instance.level) continue;
                else if (tileChoice.name.Contains("Pistol") && pistolFirstSpawnLevel > GameManager.instance.level) continue;
                else if (tileChoice.name.Contains("Rifle") && rifleFirstSpawnLevel > GameManager.instance.level) continue;
            } 
            //If it is an Enemy, do the same
            else if (tileChoice.GetComponent<Enemy>())
            {
                if (enemyFirstSpawnLevel > GameManager.instance.level) break;
                else if (tileChoice.GetComponent<Enemy>().name.Contains("1") && enemy1FirstSpawnLevel > GameManager.instance.level) continue;
                else if (tileChoice.GetComponent<Enemy>().name.Contains("2") && enemy2FirstSpawnLevel > GameManager.instance.level) continue;
                else if (tileChoice.GetComponent<Enemy>().name.Contains("3") && enemy3FirstSpawnLevel > GameManager.instance.level) continue;
            }

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);

            i++;
        }
    }


    //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void SetupScene(int level)
    {
        //Generate level size.
        GenerateLevelSize(level);

        //Creates the outer walls and floor.
        BoardSetup();

        //Reset our list of gridpositions.
        InitialiseList();

        //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(wallTiles, Mathf.FloorToInt(wallCount.minimum * areaRatio * wallSpawnFactor), Mathf.FloorToInt(wallCount.maximum * areaRatio * wallSpawnFactor));

        //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(foodTiles, Mathf.FloorToInt(foodCount.minimum * areaRatio * foodSpawnFactor), Mathf.FloorToInt(foodCount.maximum * areaRatio * foodSpawnFactor));

        //Instantiate a random number of items based on minimum and maximum, at randomized positions.
        if (itemFirstSpawnLevel <= GameManager.instance.level)
            LayoutObjectAtRandom(itemTiles, Mathf.FloorToInt(itemCount.minimum * areaRatio * itemSpawnFactor), Mathf.FloorToInt(itemCount.maximum * areaRatio * itemSpawnFactor));

        //Check if enemies spawn on this level
        if (enemyFirstSpawnLevel <= GameManager.instance.level)
        {
            //Determine the number of enemies to spawn based on a logarithmic function, difficulty setting and the size of the level.
            int enemyCount = Mathf.RoundToInt(Mathf.Log(level, 2.5f) * enemySpawnFactor * areaRatio);

            //Instantiate the number of enemies, at randomized positions.
            LayoutObjectAtRandom(enemyTiles, Mathf.FloorToInt(enemyCount), Mathf.FloorToInt(enemyCount));
        }

        //Instantiate the supplies object at a random location.
        LayoutObjectAtRandom(new GameObject[1] { supplies }, 1, 1);

        //Instantiate the exit tile in a random position.
        LayoutObjectAtRandom(new GameObject[1] { exit }, 1, 1);

        //Instantiate collectibles with a chance that depends on level. Only instantiate collectibles that haven't been found.
        //Probability for collectible at level 1 = 1/300. Level 30 = 1/10. Level 300 = 1
        if (Random.Range(1,(300/level)+1) == 1 && CachedDifficulty.instance.difficultyIndex >= 1 && CachedDifficulty.instance.difficultyIndex <=2)
        {
            List<GameObject> collectiblesNotFoundList = new List<GameObject>();
            for(int i = 0; i < collectibles.Length; i++)
            {
                if (!CachedCollectibles.instance.collectiblesFound[i])
                {
                    collectiblesNotFoundList.Add(collectibles[i]);
                }
            }
            GameObject[] collectiblesNotFound = collectiblesNotFoundList.ToArray();
            if (collectiblesNotFound.Length > 0) LayoutObjectAtRandom(collectiblesNotFound, 1, 1);
        }

        //Instantiate a random number of powerups based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(powerups, Mathf.FloorToInt(Mathf.Log(level,10) * 0.5f * areaRatio), Mathf.FloorToInt(Mathf.Log(level,10) * 1.5f * areaRatio));
    
    }

    //Randomly generates the size of each level, within variables dependent on how far the player has progressed.
    private void GenerateLevelSize(int level)
    {
        //Get random numbers for rows and colums, based on the level and levelSizeGrowthRate.
        //They always add up to at least 8.
        rows = Random.Range(7 + Mathf.CeilToInt(level * levelSizeGrowthRate), 7 + Mathf.CeilToInt(level * levelSizeGrowthRate*2));
        columns = Random.Range(7 + Mathf.CeilToInt(level * levelSizeGrowthRate), 7 + Mathf.CeilToInt(level * levelSizeGrowthRate*2));

        //Count the ratio for how much bigger the new area is comparing to the original game's 7x7 area
        areaRatio = (rows * columns) / 49;
    }
}