using System.Collections;
using UnityEngine;

//This class describes the behavior of the Arrow prefab, once it's been instantiated from a Crossbow.
public class EnemyProjectile : MonoBehaviour
{
    public GameObject brainBlood;   //Object with particle effects for when brain gets destroyed

    private Vector2 v;              //Speed and direction the projectile is moving at.
    private int rotateRate;         //Rate at which the projectile rotates
    private int rotation;           //Current rotation at each moment
    private int wallDamage;         //Projectile's damage to walls
    private int playerDamage;       //Projectiles damage to player
    private void Start()
    {
        //Save the projectile's velocity to be called later after collisions with Enemies.
        v = GetComponent<Rigidbody2D>().velocity;

        //Give rotation a rate
        rotateRate = 10;

        //Initialise rotation at 0
        rotation = 0;

        //Start rotating the projectile
        StartCoroutine(Spin());
    }

    //This coroutine rotates the projectile around it's center for as long as the projectile exists
    private IEnumerator Spin()
    {
        while (true)
        {
            rotation += rotateRate;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
            yield return new WaitForSeconds(0.02f);
        }
    }

    //What happens when the projectile hits something
    private void OnCollisionEnter2D(Collision2D collision)
    {

        //Find what was hit.
        GameObject otherParty = collision.collider.gameObject;

        if (otherParty.GetComponent<Wall>())
        {
            //If a wall was hit, make sure the projectile doesn't hit the same wall again. Do damage to the wall and freeze the projectile.
            Physics2D.IgnoreCollision(otherParty.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            otherParty.GetComponent<Wall>().DamageWall(wallDamage);
            Instantiate(brainBlood, transform.position, Quaternion.identity);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        } 
        else if (otherParty.tag == "Enemy")
        {
            //If an enemy was hit, make sure the projectile doesn't hit the same enemy again.
            Physics2D.IgnoreCollision(otherParty.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            //If the enemy isn't Enemy3
            if (!otherParty.name.Contains("3"))
            {
                //If the enemy isn't Enemy3, boost the enemy's movement time and turn it golden.
                otherParty.GetComponent<Enemy>().waitTime = otherParty.GetComponent<Enemy>().waitTime * 0.8f;
                if (otherParty.GetComponent<Enemy>().waitTime < 0.21f)
                {
                    otherParty.GetComponent<Enemy>().waitTime = 0.21f;
                }
                otherParty.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            //After collision, make sure the projectile continues at the same velocity, basically ignoring the collision effects on the projectile.
            GetComponent<Rigidbody2D>().velocity = v;
        } else if (otherParty.tag == "Player")
        {
            //If the player was hit, make sure the projectile doesn't hit the player again.
            Physics2D.IgnoreCollision(otherParty.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            //Do damage to the player
            otherParty.GetComponent<Player>().GetHit(playerDamage);

            Instantiate(brainBlood, transform.position, Quaternion.identity);
            //Freeze the projectile
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        } else
        {
            Instantiate(brainBlood, transform.position, Quaternion.identity);
            //If anything else was hit, just freeze the projectile. Attached Trail Renderer will destroy the projectile.
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    //The enemy throwing this brain gives the brain it's damage values
    public void SetDamage(int player, int wall)
    {
        playerDamage = player;
        wallDamage = wall;
    }

    //Called when the brain dies
    public void Die()
    {
        Instantiate(brainBlood, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
