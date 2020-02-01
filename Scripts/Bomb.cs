using System.Collections;
using UnityEngine;


public class Bomb : DroppableItem
{
    public float fuseInSeconds;             //How long it takes for a bomb to blow after setting down.
    public GameObject explosion;            //Explosion particle effect
    public AudioClip boom;                  //Explosion sound effect
    public AudioClip tickTock;              //Sound effect when putting bomb down

    private Animator animator;

    protected override void Start()
    {
        //Run the Start function in parent class Item.
        base.Start();

        //Check if this Bomb was actually dropped by the player, and not instantiated some other way.
        if (!dropped)
        {
            SetAmmoText();
            return;
        }

        animator = GetComponent<Animator>();
        AudioSource audio = GetComponent<AudioSource>();

        animator.SetTrigger("StartFuse");
        audio.volume = SoundManager.instance.options[1];
        audio.clip = tickTock;
        audio.Play();

            //Start counting down time until explosion.
            StartCoroutine(TimeFuse());
    }

    //Boom finds every Object in its explosion radius and processes them accordingly.
    //Boom then destroys this Bomb GameObject.
    private void Boom()
    {
        //Instantiate the explosion object with particle effects, shake the camera and play the sound
        Instantiate(explosion, transform.position, Quaternion.identity);
        Camera.main.GetComponent<CameraController>().AddShake(0.5f);

        //Iterate over every x-axis value that this explosion touches.
        for (int x = -1; x <= 1; x++)
        {
            //Define starting point for each Raycast.
            Vector2 explosiveLineStart = new Vector2((int)transform.position.x + x, (int)transform.position.y - 1);

            //Do the raycast. These raycasts travel vertically, touching three tiles.
            RaycastHit2D[] hitObjects = Physics2D.RaycastAll(explosiveLineStart, new Vector2(0, 1), 2);

            //Iterate over each hit objects.
            foreach (RaycastHit2D hitObject in hitObjects)
            {
                //Get the gameobject of hit object.
                GameObject blownAway = hitObject.transform.gameObject;

                //Logic for killing enemies.
                if (blownAway.tag == "Enemy")
                {
                    blownAway.GetComponent<Enemy>().Die();
                }
                //Logic for hurting the player if they are too close.
                else if (blownAway.tag == "Player")
                {
                    blownAway.GetComponent<Player>().GetHit(50);
                }
                else if (blownAway.tag == "Brain")
                    //Logic for destroying flying brains.
                {
                    blownAway.GetComponent<EnemyProjectile>().Die();
                }
                //Unactivate all other objects, if they aren't outer walls, the exit or supplies.
                else if (blownAway.name.Contains("OuterWall") || blownAway.tag == "Exit" || blownAway.tag == "Supplies")
                {
                    continue;
                }
                else
                {
                    blownAway.SetActive(false);
                }
            }
        }
        //Finally destroy this Bomb.
        Destroy(gameObject);
    }

    //Counts down time for bomb to go off, calls the Boom function after fuse runs out.
    private IEnumerator TimeFuse()
    {
        yield return new WaitForSeconds(fuseInSeconds);
        Boom();
    }
    
}