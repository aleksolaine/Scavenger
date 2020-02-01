using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script component for the Melee powerup pickups
public class MeleeBoost : Powerup
{
    //Inherits start from Powerup, only defines the player.
    protected override void Start()
    {
        base.Start();
    }

    //Activate is called when the pickup gets picked up by player
    public override void Activate()
    {
        //Call the ActivatePowerup method from Player. MeleeBoost's index is 1 and boostFactor is a variable defined in the base class.
        player.ActivatePowerup(1, boostFactor);
    }
}