using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : RangedWeapon
{

    //Initialise Crossbow by calling base classes' Start methods.
    //First calls Start from RangedWeapon, which calls for Start from Item.
    //These fetch any previous ammo Crossbow may have had, and set the item info on player's display.
    protected override void Start()
    {

        base.Start();
    }

    //Shoot does just that, shoots an arrow. After it's decided how to rotate it.
    protected override void Shoot()
    {
        //Play sound effect for shooting an arrow
        SoundManager.instance.RandomizeSfx(shoot);

        //Instatiates a fast moving, deadly arrow with correct rotation.
        if (direction == new Vector2(0, 1))
        {
            Rigidbody2D arrow = Instantiate(projectile, (Vector2)GetComponentInParent<Player>().transform.position + (direction * 0.6f), Quaternion.Euler(0,0,325));
            arrow.velocity = direction * projectileSpeed;
            Physics2D.IgnoreCollision(arrow.GetComponent<Collider2D>(), GetComponentInParent<Player>().GetComponent<Collider2D>());
        }
        else if (direction == new Vector2(1, 0))
        {
            Rigidbody2D arrow = Instantiate(projectile, (Vector2)GetComponentInParent<Player>().transform.position + (direction * 0.6f), Quaternion.Euler(0, 0, 235));
            arrow.velocity = direction * projectileSpeed;
            Physics2D.IgnoreCollision(arrow.GetComponent<Collider2D>(), GetComponentInParent<Player>().GetComponent<Collider2D>());
        }
        else if (direction == new Vector2(0, -1))
        {
            Rigidbody2D arrow = Instantiate(projectile, (Vector2)GetComponentInParent<Player>().transform.position + (direction * 0.6f), Quaternion.Euler(0, 0, 145));
            arrow.velocity = direction * projectileSpeed;
            Physics2D.IgnoreCollision(arrow.GetComponent<Collider2D>(), GetComponentInParent<Player>().GetComponent<Collider2D>());
        }
        else if (direction == new Vector2(-1, 0))
        {
            Rigidbody2D arrow = Instantiate(projectile, (Vector2)GetComponentInParent<Player>().transform.position + (direction * 0.6f), Quaternion.Euler(0, 0, 55));
            arrow.velocity = direction * projectileSpeed;
            Physics2D.IgnoreCollision(arrow.GetComponent<Collider2D>(), GetComponentInParent<Player>().GetComponent<Collider2D>());
        }
        Debug.Log("Crossbow fired");
    }
}