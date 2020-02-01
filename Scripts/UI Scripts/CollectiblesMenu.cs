using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CollectiblesMenu : MonoBehaviour
{
    public Toggle[] toggles = new Toggle[12];
    // Start is called before the first frame update
    void OnEnable()
    {
        for (int i = 0; i < toggles.Length; i++)
        {

            toggles[i].isOn = CachedCollectibles.instance.collectiblesFound[i];

        }
    }
}