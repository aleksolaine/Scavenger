using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    [HideInInspector]public int numberOfUses;       //How many uses this item has left
    public int initNumberOfUses;                    //How many uses come with this item initially
    public int maxNumberOfUses;                     //How many uses can be stacked by collecting multiple pickups
    public float cooldown;                          //How often item can be used

    //When the item is initialized, check if it is after a level change by looking if GameManager has its data saved
    //If there isn't, initialize the item with default initial number of uses.
    protected virtual void Start()
    {
        if (GameManager.instance.heldItemInfo.Value != 0)
        {
            numberOfUses = GameManager.instance.heldItemInfo.Value;
            GameManager.instance.heldItemInfo = new KeyValuePair<string, int>();
        }
        else if (GameManager.instance.heldItemInfo.Value == 0)
        {
            numberOfUses = initNumberOfUses;
        }
    }

    //Method called by subclasses once the item has been used.
    protected void ItemUsed()
    {
        //Using an item causes the player to lose food.
        GetComponentInParent<Player>().LoseFood();

        //Take one use off the item.
        numberOfUses--;

        //Check if uses are out and destroy the item if so.
        if (numberOfUses <= 0)
        {
            Destroy(gameObject, 0.05f);
            GameManager.instance.ammoText.text = "No item";
        }
    }
    
    //Abstract method that subclasses have to fill. Displays item information on game display.
    public abstract void SetAmmoText();

    //Adds uses to the equipped item, up to the maximum stackable amount. Also updates item info on display.
    public void AddUses()
    {
        numberOfUses = Mathf.Clamp(numberOfUses + initNumberOfUses, 0, maxNumberOfUses);
        SetAmmoText();
    }
}
