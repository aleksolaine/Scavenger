using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameOptions : MonoBehaviour
{
    public Text difficultyLevel;

    public Slider startLevel;
    public Slider initFood;

    public Slider levelGrowthRate;
    public Slider wallSpawn;
    public Slider foodSpawn;
    public Slider itemSpawn;
    public Slider enemySpawn;

    public Slider bombDay;
    public Slider crossbowDay;
    public Slider pistolDay;
    public Slider rifleDay;
    public Slider enemy1Spawn;
    public Slider enemy2Spawn;
    public Slider enemy3Spawn;

    public Slider enemyWait;
    public Slider enemyPlayerDamage;
    public Slider enemyWallDamage;
    public Slider enemyHP;
    
    public Slider playerWaitTime;
    public Slider foodIntake;
    public Slider playerEnemyDamage;
    public Slider playerWallDamage;
    public Slider foodDepletionTime;

    private static string[] difficultyLevels = new string[4] { "Easy", "Normal", "Hard", "Custom" };
    private int difficultyIndex;

    // Start is called before the first frame update
    void OnEnable()
    {
        difficultyIndex = CachedDifficulty.instance.difficultyIndex;
        SetDifficultyData(difficultyIndex);
    }
    public void CacheDifficultyData()
    {
        int firstLevel = (int)startLevel.value;
        int startFood = (int)initFood.value;

        float levelSizeGrowthRate = levelGrowthRate.value;
        float wallFactor = wallSpawn.value;
        float foodFactor = foodSpawn.value;
        float itemFactor = itemSpawn.value;
        float enemyFactor = enemySpawn.value;

        int bombSpawn = (int)bombDay.value;
        int pistolSpawn = (int)pistolDay.value;
        int crossbowSpawn = (int)crossbowDay.value;
        int rifleSpawn = (int)rifleDay.value;
        int enemy1Day = (int)enemy1Spawn.value;
        int enemy2Day = (int)enemy2Spawn.value;
        int enemy3Day = (int)enemy3Spawn.value;

        float enemyCooldown = enemyWait.value;
        float enemyWall = enemyWallDamage.value;
        float enemyPlayer = enemyPlayerDamage.value;
        float zombieHP = enemyHP.value;
        
        float playerWait = playerWaitTime.value;
        float metabolism = foodIntake.value;
        float playerEnemy = playerEnemyDamage.value;
        float playerWall = playerWallDamage.value;
        float foodDepletion = foodDepletionTime.value;

        CachedDifficulty.instance.Update(difficultyIndex, firstLevel, startFood, levelSizeGrowthRate, wallFactor,
            foodFactor, itemFactor, enemyFactor, bombSpawn, crossbowSpawn, pistolSpawn, rifleSpawn, enemy1Day, enemy2Day, enemy3Day,
            enemyCooldown, enemyPlayer, enemyWall, zombieHP, playerWait, metabolism, playerEnemy, playerWall, foodDepletion);
    }
    public void IncreaseDifficulty()
    {
        difficultyIndex = Mathf.Clamp(++difficultyIndex, 0, 2);
        SetDifficultyData(difficultyIndex);
    }
    public void DecreaseDifficulty()
    {
        difficultyIndex = Mathf.Clamp(--difficultyIndex, 0, 2);
        SetDifficultyData(difficultyIndex);
    }
    public void CustomiseDifficulty()
    {
        difficultyIndex = 3;
        difficultyLevel.text = difficultyLevels[difficultyIndex];
        Slider[] sliders = GetComponentsInChildren<Slider>();
        foreach (Slider slider in sliders)
        {
            slider.interactable = true;
        }
    }
    private void SetDifficultyData(int index)
    {
        if (index == 3)
        {
            Slider[] sliders = GetComponentsInChildren<Slider>();
            foreach (Slider slider in sliders)
            {
                slider.interactable = true;
            }
        }
        else
        {
            Slider[] sliders = GetComponentsInChildren<Slider>();
            foreach (Slider slider in sliders)
            {
                slider.interactable = false;
                CachedDifficulty.instance.Update(index);
            }
        }

        difficultyLevel.text = difficultyLevels[index];

        initFood.value = CachedDifficulty.instance.initialFoodPoints;
        startLevel.value = CachedDifficulty.instance.startFromLevel;

        levelGrowthRate.value = CachedDifficulty.instance.levelGrowthFactor;
        wallSpawn.value = CachedDifficulty.instance.wallSpawnFactor;
        foodSpawn.value = CachedDifficulty.instance.foodSpawnFactor;
        itemSpawn.value = CachedDifficulty.instance.itemSpawnFactor;
        enemySpawn.value = CachedDifficulty.instance.enemySpawnFactor;

        bombDay.value = CachedDifficulty.instance.bombSpawn;
        crossbowDay.value = CachedDifficulty.instance.crossbowSpawn;
        pistolDay.value = CachedDifficulty.instance.pistolSpawn;
        rifleDay.value = CachedDifficulty.instance.rifleSpawn;
        enemy1Spawn.value = CachedDifficulty.instance.enemy1Spawn;
        enemy2Spawn.value = CachedDifficulty.instance.enemy2Spawn;
        enemy3Spawn.value = CachedDifficulty.instance.enemy3Spawn;

        enemyWait.value = CachedDifficulty.instance.enemyWaitTimeModifier;
        enemyPlayerDamage.value = CachedDifficulty.instance.enemyPlayerDamageModifier;
        enemyWallDamage.value = CachedDifficulty.instance.enemyWallDamageModifier;
        enemyHP.value = CachedDifficulty.instance.hpModifier;

        playerWaitTime.value = CachedDifficulty.instance.playerWaitTimeModifier;
        foodIntake.value = CachedDifficulty.instance.foodIntakeModifier;
        playerEnemyDamage.value = CachedDifficulty.instance.playerEnemyDamageModifier;
        playerWallDamage.value = CachedDifficulty.instance.playerWallDamageModifier;
        foodDepletionTime.value = CachedDifficulty.instance.foodDepletionTime;
}
    public void StartGame()
    {
        CacheDifficultyData();
        CachedLevelData.instance.Reset();
        SaveManager.SaveDifficultyData();
        SaveManager.SaveLevelData();
    }
}