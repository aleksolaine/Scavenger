//Script component for the Armor powerup pickups
public class ArmorBoost : Powerup
{
    //Inherits start from Powerup, only defines the player.
    protected override void Start()
    {
        base.Start();
    }
    //Activate is called when the pickup gets picked up by player
    public override void Activate()
    {
        //Call the ActivatePowerup method from Player. Armor's index is 2 and boostFactor is a variable defined in the base class.
        player.ActivatePowerup(2, boostFactor);
    }
}