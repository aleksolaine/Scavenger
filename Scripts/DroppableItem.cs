using UnityEngine;

public abstract class DroppableItem : Item
{
    protected bool dropped = false;         //Boolean that's only true if the player has dropped this item.
    
    //High-level dropping logic. Creates a new clone and drops it on the game board.
    public void Drop()
    {
        //Instantiate a new clone of the currently equipped item.
        DroppableItem droppedClone = Instantiate(this, GetComponentInParent<Player>().transform.position, Quaternion.identity);

        //Make sure the new dropped item gets displayed below players and units, but on top of food.
        SpriteRenderer sr = droppedClone.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Items";
        sr.sortingOrder = 1;

        //Give confirmation that the item was actually dropped by player.
        droppedClone.dropped = true;

        //Calls the superclass' method to let the game know that an item has been used.
        ItemUsed();

        if (numberOfUses <= 0) return;

        SetAmmoText();
    }
    //Method for putting correct Bomb-related info on the display.
    public override void SetAmmoText()
    {
        GameManager.instance.ammoText.text = "Bombs: " + numberOfUses;
    }
}
