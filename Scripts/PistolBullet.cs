using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class describes the behavior of the PistolBullet prefab, once it's been instantiated from a Pistol.
public class PistolBullet : MonoBehaviour
{

    //Logic for handling collisions, or what happens when bullet hits something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Find what was hit.
        GameObject otherParty = collision.collider.gameObject;

        //If an Enemy was hit, destroy the Enemy and freeze the bullet.
        if (otherParty.tag == "Enemy")
        {
            otherParty.GetComponent<Enemy>().Die();
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else if (otherParty.tag == "Brain")
        {
            otherParty.GetComponent<EnemyProjectile>().Die();
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else 
        //If something else was hit, just freeze the bullet. TrailRenderer will destroy the frozen bullets very quickly.
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}