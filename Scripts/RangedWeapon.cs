using System.Collections;
using UnityEngine;
public abstract class RangedWeapon : Item
{
    public Rigidbody2D projectile;      //Projectile prefab that this weapon fires.
    public float projectileSpeed;       //The speed at which the projectile leaves the weapon.
    public AudioClip[] shoot;

    protected Vector2 start;            //Starting point of a projectile when shot
    protected Vector2 direction;        //Direction of the projectile.
    protected Vector2 end;              //Where the projectile stops.

    //Start calls the base Item class' Start method, which finds how many uses this item should have.
    //Item info is then updated into the AmmoText textbox.
    protected override void Start()
    {
        base.Start();
        SetAmmoText();
    }

    //Fire takes the direction which was shot at, and completes the firing procedure.
    public void Fire(Vector2 direction)
    {
        //Take the given direction into the instance variable of this object.
        this.direction = direction;

        //Define starting point for the projectile: 
        //One tile away from players position in the direction of the shot.
        start = (Vector2)GetComponentInParent<Player>().transform.position + direction;

        //Get the shot logic from below.
        Shoot();

        //Let base class know we are done with the use action.
        ItemUsed();

        //If number of uses is less than zero, exit this method here so AmmoText can be updated in the base class to show that this item was used up.
        if (numberOfUses <= 0) return;
        SetAmmoText();

    }

    //Virtual shoot method for subclasses to modify or overwrite if needed.
    protected virtual void Shoot()
    {
        //Instantiate a projectile at the direction the shot was made at and give the projectile appropriate speed, depending on the weapon used.
        Rigidbody2D projectileRB2D = Instantiate(projectile, (Vector2)GetComponentInParent<Player>().transform.position + (direction * 0.6f), Quaternion.identity);
        projectileRB2D.velocity = direction * projectileSpeed;
    }

    //RangedWeapons' version of how AmmoText gets set.
    public override void SetAmmoText()
    {
        GameManager.instance.ammoText.text = name.Replace("(Clone)", "") + "\nAmmo: " + numberOfUses;
    }
}