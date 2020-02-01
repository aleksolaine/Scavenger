using UnityEngine;

//This class describes the behavior of the Arrow prefab, once it's been instantiated from a Crossbow.
public class Arrow : MonoBehaviour
{
    public AudioClip thonk;             //AudioClip played when arrow hits something

    //What happens when arrow hits something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        //Find what was hit.
        GameObject otherParty = collision.collider.gameObject;

        if (otherParty.tag == "Enemy")
        //If an Enemy was hit, destroy the Enemy and the Arrow on the spot.
        {
            otherParty.GetComponent<Enemy>().Die();
            Destroy(gameObject);
        }
        else if (otherParty.tag == "Brain")
            //Same for brains
        {
            otherParty.GetComponent<EnemyProjectile>().Die();
        }

        //Do these for anything that was hit, including Enemy & brain
        //Plays the arrow hit sound, and makes sure the arrow stops
        //Once Arrow has stopped, the included TrailRenderer will destroy the arrow as soon as trail catches up with it.
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        SoundManager.instance.PlaySingle(thonk);
    }


}
