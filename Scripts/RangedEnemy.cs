using System.Collections;
using UnityEngine;



//RangeEnemy inherits from Enemy.
public class RangedEnemy : Enemy
{
    public Rigidbody2D projectile;      //The Rigidbody prefab to be used as a projectile.
    public float projectileSpeed;       //The projectile's speed
 
    

    //Start overrides the virtual Start function of the base class.
    protected override void Start()
    {
        //Call the start function of our base class Enemy.
        base.Start();
    }

    //MoveEnemy is called by the MoveTimer periodically.
    public override void MoveEnemy()
    {
        //If game is paused, skip this move.
        if (GameManager.instance.pause) return;

        //If player is further than this Enemy's sense distance, enemy won't do anything.
        if ((target.position - transform.position).magnitude >= senseDistance) return;

        //Instantiate a projectile that flies in the Player's direction. Make sure the projectile doesn't collide with this enemy.
        Vector2 direction = (target.position - transform.position).normalized;
        //Set the attack trigger of animator to trigger Enemy attack animation.
        animator.SetTrigger("enemyAttack");
        Rigidbody2D brains = Instantiate(projectile, transform.position, Quaternion.identity);
        brains.velocity = direction * projectileSpeed;
        brains.GetComponent<EnemyProjectile>().SetDamage(playerDamage, wallDamage);
        Physics2D.IgnoreCollision(brains.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }
    
}

