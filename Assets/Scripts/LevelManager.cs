using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GridElement> enemySpawnPoints = new List<GridElement>(); // Spawn points where enemy can spawn

    public List<GridElement> playerSpawnPoints = new List<GridElement>(); // Spawn points where player can spawn

    // Adds spawn point for enemy and players to spawn to
    public void AddSpawnPoint(GridElement element, bool isEnemy)
    {
        if (isEnemy)
        {
            if (!enemySpawnPoints.Contains(element))
            {
                enemySpawnPoints.Add(element);
            }
        }
        else
        {
            if (!playerSpawnPoints.Contains(element))
            {
                playerSpawnPoints.Add(element);
            }
        }
    }
}
