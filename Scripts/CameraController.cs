using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;                      //Public variable to store a reference to the player game object

    private int rows;                               //Number of rows on current game level.
    private int columns;                            //Number of colums on current game level.
    private Vector3 targetPosition;                 //Player's position.
    private Vector3 velocity = Vector3.zero;        //Zero vector used in smoothing the camera.

    
    private float shakeDuration = 0f;               //Desired duration of bomb shake effect
    private float shakeMagnitude = 0.1f;            //A measure of magnitude for the shake
    private float dampingSpeed = 1.0f;              //A measure of how quickly the shake effect should evaporate
    private Vector3 shake;                          //The shake vector applied to where the camera should be

    //Initialise camera.
    void Start () 
    {
        //Get a reference to Player and call GameManager for the rows and columns in the current map.
        //Rows and columns are needed to limit the camera to stop at board bounds.
        player = GameObject.FindGameObjectWithTag("Player");
        rows = GameManager.instance.RowsAndColums()[0];
        columns = GameManager.instance.RowsAndColums()[1];
    }

    //Camera position is calculated in LateUpdate
    void LateUpdate () 
    {
        //Check if there's any shake from an explosion
        if (shakeDuration > 0)
        {
            //if so, apply shake magnitude to shake vector and lower the duration left
            shake = Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            //If there's no shake left, set duration to zero and shake vector to zero vector.
            shakeDuration = 0f;
            shake = Vector3.zero;
        }
        //targetPosition is where the camera tries to be, after taking into account player's position and level restrictions.
        targetPosition = new Vector3(
        Mathf.Clamp(player.transform.position.x, 3.5f, columns - 4.5f),
        Mathf.Clamp(player.transform.position.y, 3.5f, rows - 4.5f),
        -10);

        //Move the camera smoothly toward targetPosition and apply shake in case of an explosion.
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.3f) + shake;
    }

    //Called by Bomb when it explodes, adds shake to the camera.
    public void AddShake(float shakeDuration)
    {
        this.shakeDuration += shakeDuration;
    }
}
