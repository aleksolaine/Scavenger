using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationMenu : MonoBehaviour
{
    //Reset all level-related save data (level, food, item).
    public void ResetProgress()
    {
        CachedDifficulty.instance.Reset();
        CachedLevelData.instance.Reset();
        CachedCollectibles.instance.Reset();
        SaveManager.SaveDifficultyData();
        SaveManager.SaveLevelData();
        SaveManager.SaveCollectibles();
    }
}
