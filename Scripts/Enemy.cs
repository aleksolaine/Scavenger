using System.Collections;
using UnityEngine;



//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MovingObject
{
    public int playerDamage;                            //The amount of food points to subtract from the player when attacking.
    public int wallDamage;                              //The amount of hp to subtract from walls when walking at them.
    public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.
    public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.
    public int hp;                                      //This enemy's hp.
    public int senseDistance;                           //How far the enemy senses the player from.
    public GameObject bloodSplash;

    protected Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
    protected Transform target;                           //Transform to attempt to move toward each turn. In effect the player.
    protected int thisEnemyIndex;                         //Queue number for this enemy. Used to slightly separate the times enemies make their moves.


    //Start overrides the virtual Start function of the base class.
    protected override void Start()
    {
        //Adjust variables according to difficulty settings.
        hp = Mathf.CeilToInt(hp * CachedDifficulty.instance.hpModifier);
        wallDamage = Mathf.CeilToInt(wallDamage * CachedDifficulty.instance.enemyWallDamageModifier);
        playerDamage = Mathf.CeilToInt(playerDamage * CachedDifficulty.instance.enemyPlayerDamageModifier);
        waitTime = Mathf.Clamp(waitTime * CachedDifficulty.instance.enemyWaitTimeModifier,1.1f,float.PositiveInfinity);

        //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
        //This allows the GameManager to issue movement commands.
        thisEnemyIndex = GameManager.instance.AddEnemyToList(this);

        //Get and store a reference to the attached Animator component.
        animator = GetComponent<Animator>();

        //Find the Player GameObject using it's tag and store a reference to its transform component.
        target = GameObject.FindGameObjectWithTag("Player").transform;

        //Call the start function of our base class MovingObject.
        base.Start();

        //Start waking up the enemy.
        StartCoroutine(WakeUp());
        
    }

    //WakeUp makes the enemy wait a short moment before making it's first move after loading into a level.
    private IEnumerator WakeUp()
    {
        //Prevent enemy from waking up while game is paused.
        while (GameManager.instance.pause) yield return null;

        //Separate enemies' movements by a tiny bit
        yield return new WaitForSeconds(waitTime + (((waitTime * 0.2f) * thisEnemyIndex)%1f));

        //Start the enemy's periodical moves
        StartCoroutine(MoveTimer());
    }

    //For as long as the enemy is alive, it will make moves periodically according to this method.
    private IEnumerator MoveTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            MoveEnemy();
        }
    }

    //MoveEnemy is called by the MoveTimer periodically.
    public virtual void MoveEnemy()
    {
        //If game is paused, skip this move.
        if (GameManager.instance.pause) return;

        //If player is further than this Enemy's sense distance, enemy won't move.
        if ((target.position - transform.position).magnitude >= senseDistance) return; 

        //Declare variables for X and Y axis move directions, these range from -1 to 1.
        //These values allow us to choose between the cardinal directions: up, down, left and right.
        int xDir = 0;
        int yDir = 0;

        //If the difference in positions is approximately zero (Epsilon) do the following:
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)

            //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
            yDir = target.position.y > transform.position.y ? 1 : -1;

        //If the difference in positions is not approximately zero (Epsilon) do the following:
        else
            //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
            xDir = target.position.x > transform.position.x ? 1 : -1;

        //Call the AttemptMove function from base class MovingObject 
        //Pass in the generic parameters Player and Wall, because Enemy is moving and expecting to potentially encounter a Player or Wall.
        AttemptMove<Player, Wall>(xDir, yDir);
    }

    //OnCantMove is called if Enemy attempts to move into a space occupied by a Player or Wall, it overrides the OnCantMove function of MovingObject 
    //and takes a generic parameter T which we use to pass in the component we encounter.
    protected override void OnCantMove<T>(T component)
    {
        //If bumped into a wall, damage the wall.
        if (component.GetComponent<Wall>())
        {
            //Set hitWall to equal the component passed in as a parameter.
            Wall hitWall = component as Wall;

            //Call the DamageWall function of the Wall we are hitting.
            hitWall.DamageWall(wallDamage);

            //Set the attack trigger of the enemy's animation controller in order to play the enemy's attack animation.
            animator.SetTrigger("enemyAttack");
        }
        //If bumped into player, damage the player.
        else if (component.GetComponent<Player>())
        {
            //Declare hitPlayer and set it to equal the encountered component.
            Player hitPlayer = component as Player;

            //Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
            hitPlayer.GetHit(playerDamage);

            //Set the attack trigger of animator to trigger Enemy attack animation.
            animator.SetTrigger("enemyAttack");

            //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
            SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);
        }
    }

    //DamageEnemy is called from Player when doing melee damage to Enemy.
    public void DamageEnemy(int loss)
    {
        //Subtract loss from hit point total.
        hp -= loss;

        //If hit points are less than or equal to zero:
        if (hp <= 0)

            //Kill the gameObject.
            Die();
    }

    //Die is called when the enemy dies from unnatural causes
    public void Die()
    {
        Instantiate(bloodSplash, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    protected void OnDestroy()
    {
        GameManager.instance.RemoveEnemyFromList(this);
    }
}
