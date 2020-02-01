using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class describes the behavior of the RifleBullet prefab, once it's been instantiated from a Rifle.
public class RifleBullet : MonoBehaviour
{
    private Vector2 v;      //Speed and direction the bullet is moving at.
    private void Start()
    {
        //Save the bullet's velocity to be called later after collisions with Enemies.
        v = GetComponent<Rigidbody2D>().velocity;
    }

    //Logic for handling collisions, or what happens when bullet hits something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Find what was hit.
        GameObject otherParty = collision.collider.gameObject;

        //If an Enemy was hit, destroy the Enemy and give the bullet it's velocity back, simulating the fact that the
        //bullet simply goes through the enemy without losing momentum.
        if (otherParty.tag == "Enemy")
        {
            otherParty.GetComponent<Enemy>().Die();
            GetComponent<Rigidbody2D>().velocity = v;
        }
        else if (otherParty.tag == "Brain")
        {
            otherParty.GetComponent<EnemyProjectile>().Die();
            GetComponent<Rigidbody2D>().velocity = v;
        }
        else
        {
            //If something else was hit, just freeze the bullet. TrailRenderer will destroy the frozen bullets very quickly.
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
