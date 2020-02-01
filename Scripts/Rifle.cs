using UnityEngine;
public class Rifle : RangedWeapon
{
    //Initialise Gun by calling base classes' Start methods.
    //First calls Start from RangedWeapon, which calls for Start from Item.
    //These fetch any previous ammo Gun may have had, and set the item info on player's display.
    protected override void Start()
    {
        base.Start();
    }
    protected override void Shoot()
    {
        SoundManager.instance.RandomizeSfx(shoot);
        base.Shoot();
    }
}