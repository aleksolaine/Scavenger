using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for powerups
public abstract class Powerup : MonoBehaviour
{
    public float boostFactor;   //How much the powerup boosts stats
    protected Player player;    //The player
    protected virtual void Start()
    {
        //Find the player.
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    //Activate needs to be completed by all child classes.
    public abstract void Activate();
}