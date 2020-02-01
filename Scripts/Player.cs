using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
    public float restartLevelDelay;             //Delay time in seconds to restart level.
    public int pointsPerFood;                   //Number of points to add to player food points when picking up a food object.
    public int pointsPerSoda;                   //Number of points to add to player food points when picking up a soda object.
    public int wallDamage;                      //How much damage a player does to a wall when chopping it.
    public int enemyDamage;                     //How much damage a player does to an enemy when chopping it.
    public Text foodText;                       //UI Text to display current player food total.
    public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
    public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
    public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
    public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
    public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
    public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
    public AudioClip gameOverSound;             //Audio clip to play when player dies.
    public AudioClip itemPickup1;
    public AudioClip itemPickup2;
    public AudioClip collectiblePickup;
    public AudioClip chopSound1;                //1 of 2 audio clips that play when the wall is attacked by the player.
    public AudioClip chopSound2;                //2 of 2 audio clips that play when the wall is attacked by the player.

    public float foodDepletionTime;             //Time it takes for one food to get deducted.
    public Text collectText;

    private GameObject item;                    //Item equipped by player.
    private SpriteRenderer exitRenderer;        //SpriteRenderer for the exit GameObject on map at each time.
    private Animator animator;                  //Used to store a reference to the Player's animator component.
    private int food;                           //Used to store player food points total during level.
    private bool suppliesCollected = false;     //A check for if supplies on the level have been collected. Player can't exit the level without supplies.
    private float foodTicker;                   //Time since food was last depleted because of time elapsing.
    private bool moveCooldown = false;          //True if we still have to wait to move again
    private bool itemCooldown = false;          //True if our item isn't ready to be used again
    private float armorFactor = 1;

    //Start overrides the Start function of MovingObject
    protected override void Start()
    {
        //Adust variables according to difficulty settings.
        foodDepletionTime = CachedDifficulty.instance.foodDepletionTime;
        pointsPerFood = Mathf.CeilToInt(pointsPerFood * CachedDifficulty.instance.foodIntakeModifier);
        pointsPerSoda = Mathf.CeilToInt(pointsPerSoda * CachedDifficulty.instance.foodIntakeModifier);
        waitTime *= CachedDifficulty.instance.playerWaitTimeModifier;
        wallDamage = Mathf.CeilToInt(wallDamage * CachedDifficulty.instance.playerWallDamageModifier);
        enemyDamage = Mathf.CeilToInt(enemyDamage * CachedDifficulty.instance.playerEnemyDamageModifier);

        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

        //Get the current food point total stored in GameManager.instance between levels.
        food = GameManager.instance.playerFoodPoints;

        //Set the foodText to reflect the current player food total.
        foodText.text = "Food: " + food;

        //Initialise foodTicker at 0 to not instantly lose food at the start of level.
        foodTicker = 0;

        //Give the player their held item from last level.
        if (GameManager.instance.heldItemInfo.Key == "" || GameManager.instance.heldItemInfo.Key == null)
            GameManager.instance.ammoText.text = "No item";
        else
        {
            ChooseItem(GameManager.instance.heldItemInfo.Key);
        }
        

        //Fimd the exit gameobject, get it's SpriteRenderer and store it for later.
        exitRenderer = GameObject.FindGameObjectWithTag("Exit").GetComponent<SpriteRenderer>();

        //Call the Start function of the MovingObject base class.
        base.Start();
    }


    //This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable()
    {
        //When Player object is disabled, store the current local food total and held item in the GameManager so they can be re-loaded in next level.
        GameManager.instance.playerFoodPoints = food;

        //If Player has an item equipped, store it's information in the GameManager so they also can be reloaded next level.
        //Saved data includes item type and remaining uses.
        if (item != null)
        GameManager.instance.heldItemInfo = new KeyValuePair<string, int>(item.name.Replace("(Clone)", ""), item.GetComponent<Item>().numberOfUses);
    }


    private void Update()
    {
        //If game is paused, do nothing during Update
        if (GameManager.instance.pause) return;

        //Add deltaTime to foodTicker. When foodTicker reaches preset foodDepletionTime, one food is deducted from player.
        foodTicker += Time.deltaTime;
        if (foodTicker >= foodDepletionTime)
        {
            LoseFood();
            foodTicker = 0;
        }

        //If there's an item equipped, and it is not on cooldown, check if it is given any inputs, such as activate or fire.
        if (item != null && !itemCooldown)
        {

            //If the equipped item is a droppable item with just one mode of use, check if player is pressing spacebar to use it.
            if (item.GetComponent<DroppableItem>())
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ItemDrop();
                }
            }

            //Else if the item is a ranged weapon, check if player is giving a direction to fire at with the WASD keys.
            else if (item.GetComponent<RangedWeapon>())
            {
                int horizontalFire = 0;
                int verticalFire = 0;

                //Get firing inputs
                horizontalFire = (int)Input.GetAxisRaw("Horizontal Fire");

                verticalFire = (int)Input.GetAxisRaw("Vertical Fire");

                //Check if firing horizontally, if so set vertical to zero.
                if (horizontalFire != 0)
                {
                    verticalFire = 0;
                }

                //If there is an input value, fire the weapon at that direction
                if (horizontalFire != 0 || verticalFire != 0)
                {
                    ItemFire(horizontalFire, verticalFire);
                }
            }
        }

        //After checking for item usage, check if movement is on cooldown. If it is, exit update function. Otherwise, look for movement inputs.
        if (moveCooldown) return;

        int horizontalMove = 0;     //Used to store the horizontal move direction.
        int verticalMove = 0;       //Used to store the vertical move direction.

        //Get movement inputs
        horizontalMove = (int)Input.GetAxisRaw("Horizontal Move");

        verticalMove = (int)Input.GetAxisRaw("Vertical Move");

        //Check if moving horizontally, if so set vertical to zero.
        if (horizontalMove != 0)
        {
            verticalMove = 0;
        }

        //Check if we have a non-zero value for horizontal or vertical
        if (horizontalMove != 0 || verticalMove != 0)
        {
            //Call AttemptMove passing in the generic parameters Wall and Enemy, since those are what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            AttemptMove<Wall, Enemy>(horizontalMove, verticalMove);

            //After movement attempt, put movement on cooldown and start the coroutine that counts down the cooldown.
            moveCooldown = true;
            StartCoroutine(MoveCooldown());
        }
    }

    //This method is called for single use mode droppable items, such as bombs.
    private void ItemDrop()
    {
        //Tell the item object that it needs to drop an item.
        item.GetComponent<DroppableItem>().Drop();

        //Put item on cooldown and start the coroutine that counts down the cooldown.
        itemCooldown = true;
        StartCoroutine(ItemCooldown());
    }
    //This method is called for items that take directional inputs and fire at that direction
    private void ItemFire(int xDir, int yDir){
        
        //Convert the inputs into a direction vector and pass that vector to the weapon object, telling it to fire at that direction.
        Vector2 direction = new Vector2(xDir, yDir);
        item.GetComponent<RangedWeapon>().Fire(direction);

        //Put item on cooldown and start the coroutine that counts down the cooldown.
        itemCooldown = true;
        StartCoroutine(ItemCooldown());
    }
    
    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes two generic parameters T1 and T2 which for Player will be of the types Wall and Enemy, it also takes integers for x and y direction to move in.
    protected override void AttemptMove<T1, T2>(int xDir, int yDir)
    {
        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove<T1, T2>(xDir, yDir);

        //If Move from base class returns true, meaning Player was able to move into an empty space.
        if (canMove)
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        //Lose one food when attempting to move.
        LoseFood();
    }

    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is a Wall or an Enemy which the player can attack and destroy.
    protected override void OnCantMove<T>(T component)
    {
        //If the component is a wall, react by attacking the wall component.
        if (component.GetComponent<Wall>())
        {
            //Set hitWall to equal the component passed in as a parameter.
            Wall hitWall = component as Wall;

            //Call the DamageWall function of the Wall we are hitting.
            hitWall.DamageWall(wallDamage);

            //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
            animator.SetTrigger("playerChop");

        //Else if the component is an enemy, attack the Enemy instead.
        } else if (component.GetComponent<Enemy>())
        {
            SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);

            //Set hitEnemy to equal the component passed in as a parameter.
            Enemy hitEnemy = component as Enemy;

            //Call the DamageEnemy function of the Wall we are hitting.
            hitEnemy.DamageEnemy(enemyDamage);

            //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
            animator.SetTrigger("playerChop");
        }
    }

    //Coroutine that shows a message when a collectible or powerup gets picked up
    IEnumerator ShowMessage(string message, float delay)
    {
        collectText.text = message;
        yield return new WaitForSeconds(delay);
        collectText.text = "";
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Exit")
        {

            //If the player hasn't collectd supplies yet, nothing happens.
            if (!suppliesCollected) return;

            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke("Restart", restartLevelDelay);

            GameObject[] brains = GameObject.FindGameObjectsWithTag("Brain");
            foreach (GameObject brain in brains)
            {
                brain.GetComponent<EnemyProjectile>().Die();
            }

            //Pause the game as soon as player steps on the exit to prevent enemies from damaging the player while player waits to exit.
            GameManager.instance.pause = true;

            //Disable the player object since level is over.
            enabled = false;
        }

        //Check if the tag of the trigger collided with is Food.
        else if (other.tag == "Food")
        {
            //Add pointsPerFood to the players current food total.
            food += pointsPerFood;

            //Update foodText to represent current total and notify player that they gained points
            foodText.text = "+" + pointsPerFood + " Food: " + food;

            //Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

            //Disable the food object the player collided with.
            other.gameObject.SetActive(false);
        }

        //Check if the tag of the trigger collided with is Soda.
        else if (other.tag == "Soda")
        {
            //Add pointsPerSoda to players food points total
            food += pointsPerSoda;

            //Update foodText to represent current total and notify player that they gained points
            foodText.text = "+" + pointsPerSoda + " Food: " + food;

            //Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

            //Disable the soda object the player collided with.
            other.gameObject.SetActive(false);
        }

        //Check if the tag of the trigger collided with is Supplies.
        else if (other.tag == "Supplies")
        {
            //Disable the supplies object the player collided with.
            other.gameObject.SetActive(false);

            //
            suppliesCollected = true;

            exitRenderer.color = Color.white;
        }
        //Check if the tag of the trigger collided with is Item.
        else if (other.tag == "Item")
        {
            //Call for the method that translates Pickup-object's name into an Item GameObject usable by the player.
            ChooseItem(other.name);

            SoundManager.instance.RandomizeSfx(itemPickup1, itemPickup2);

            //Disable the item pickup-object the player collided with.
            other.gameObject.SetActive(false);
        }
        //Check if the tag of the trigger collided with is Collectible.
        else if (other.tag == "Collectible")
        {
            //Show message that powerup was picked up
            StartCoroutine(ShowMessage("You found a collectible item!", 1));

            SoundManager.instance.PlaySingle(collectiblePickup);

            //Register this collectible as found
            int collectibleIndex = Convert.ToInt32(other.name.Substring(11, 2));
            CachedCollectibles.instance.Found(collectibleIndex);

            //Unactivate the gameobject
            other.gameObject.SetActive(false);
        }
        //Check if the tag of the trigger collided with is Powerup.
        else if (other.tag == "Powerup")
        {
            //Call the activate method of the pickup in question
            other.GetComponent<Powerup>().Activate();

            SoundManager.instance.RandomizeSfx(itemPickup1, itemPickup2);

            //Unactivate the gameobject
            other.gameObject.SetActive(false);
        }
    }
    //ChooseItem takes a string and checks if any item's name is included in it.
    //If there is, that item will get equipped, or if that item is already equipped, add uses to that item.
    private void ChooseItem(string name)
    {
        //If the string is for example "Bomb(Clone)", we choose Bomb as our item.
        if (name.Contains("Bomb"))
        {
            //If the currently equipped item is a Bomb, add uses to it and exit the method.
            if (item != null && item.GetComponent<Bomb>())
            {
                item.GetComponent<Bomb>().AddUses();
                return;
            }

            //If player currently has an item equipped, that isn't a bomb, destroy that item.
            if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);

            //Instantiate the Bomb GameObject for player to equip.
            item = Instantiate(GameManager.instance.items[0], transform);
        }
        else if (name.Contains("Pistol"))
        {
            if (item != null && item.GetComponent<Pistol>())
            {
                item.GetComponent<Pistol>().AddUses();
                return;
            }
            if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
            
            item = Instantiate(GameManager.instance.items[1], transform);
        }
        else if (name.Contains("Rifle"))
        {
            if (item != null && item.GetComponent<Rifle>())
            {
                item.GetComponent<Rifle>().AddUses();
                return;
            }
            if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
            
            item = Instantiate(GameManager.instance.items[2], transform);
        }
        else if (name.Contains("Crossbow"))
        {
            if (item != null && item.GetComponent<Crossbow>())
            {
                item.GetComponent<Crossbow>().AddUses();
                return;
            }
            if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
            
            item = Instantiate(GameManager.instance.items[3], transform);
        }
    }

    //Restart reloads the scene when called.
    private void Restart()
    {
        //Check in GameManager if a new record was made by passing this level
        GameManager.instance.CheckRecord();

        //Automatically save level data after level (saves levels completed, food, and equipped item)
        CachedLevelData.instance.Update(GameManager.instance);
        SaveManager.SaveLevelData();
        SaveManager.SaveCollectibles();

        //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
        //and not load all the scene object in the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }


    //GetHit is called when player takes damage.
    //It takes a parameter loss which specifies how many food points to lose.
    public void GetHit(int loss)
   {
        //Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger("playerHit");

        //Subtract lost food points from the players total.
        food -= Mathf.CeilToInt(loss / armorFactor);

        //Update the food display with the new total.
        foodText.text = "-" + Mathf.CeilToInt(loss / armorFactor) + " Food: " + food;

        //Check food to see if game has ended.
        CheckIfGameOver();
    }

    //LoseFood is called for every action for which the player loses one food.
    public void LoseFood()
    {
        //Every time player makes an action, subtract from food points total.
        food--;

        //Update food text display to reflect current score.
        foodText.text = "Food: " + food;

        //Check food to see if game has ended.
        CheckIfGameOver();
    }

    //CheckIfGameOver checks if the player is out of food points and if so, ends the game.
    private void CheckIfGameOver()
    {
        //Check if food point total is less than or equal to zero.
        if (food <= 0)
        {
            //Stop time because game is over.
            Time.timeScale = 0;

            //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
            SoundManager.instance.PlaySingle(gameOverSound);

            //Stop the background music.
            SoundManager.instance.musicSource.Stop();

            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver();

            //Dying resets level data so you have to start over from level 1.
            CachedLevelData.instance.Reset();
        }
    }

    //Coroutine that counts down an item's cooldown.
    private IEnumerator ItemCooldown()
    {
        yield return new WaitForSeconds(item.GetComponent<Item>().cooldown * CachedDifficulty.instance.playerWaitTimeModifier);
        itemCooldown = false;
    }

    //Coroutine that counts down the player's movement cooldown.
    private IEnumerator MoveCooldown()
    {
        yield return new WaitForSeconds(waitTime);
        moveCooldown = false;
    }
    //Called by the powerups, calls for the approriate powerup's method.
    public void ActivatePowerup(int boostIndex, float boostFactor)
    {
        if (boostIndex == 1)
        {
            MeleeBoostPowerup(boostFactor);
        } 
        else if (boostIndex == 2)
        {
            ArmorBoostPowerup(boostFactor);
        }
    }
    //MeleeBoostPowerup increases the player's melee damage to enemies and walls until the end of the level.
    private void MeleeBoostPowerup(float boostFactor)
    {
        StartCoroutine(ShowMessage("Melee damage increased!", 1));
        enemyDamage = Mathf.CeilToInt(enemyDamage * boostFactor);
        wallDamage = Mathf.CeilToInt(wallDamage * boostFactor);
    }
    //ArmorBoostPowerup increases player's damage resistance until the end of the level.
    private void ArmorBoostPowerup(float boostFactor)
    {
        StartCoroutine(ShowMessage("Armor improved!", 1));
        armorFactor *= boostFactor;
    }
}


