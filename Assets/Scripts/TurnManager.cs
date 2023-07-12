using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines the types of turn availables
public enum TurnType
{
    PLAYER, // Player's Turn
    ENEMY, // Enemy's Turn
}

public class TurnManager : MonoBehaviour
{
    private UIManager ui = null; // UI Manager reference

    public TurnType turn; // Current Turn Type

    private int turnCounter = 1; // Count for the turns+

    public List<GridElement> enemySpawnPoints = new List<GridElement>(); // Spawn points where enemy can spawn

    public List<GridElement> playerSpawnPoints = new List<GridElement>(); // Spawn points where player can spawn

    public GameObject enemyPrefab; // Reference to the enemy prefab for generating enemies

    public GameObject playerPrefab; // Reference to the player prefab for generating players

    public bool gameStarted = false; // Whether the game has started

    public GameObject player; // Player reference

    public Pathfinding playerPath; // Player pathfinding reference

    public GridElement playerGrid; // Reference to the last grid occupied by player

    public GameObject enemy = null; // Enemy reference

    public Pathfinding enemyPath; // Enemy pathfinding reference

    public GridElement enemyGrid; // Reference to the last grid occupied by enemy

    void Start()
    {
        ui = GetComponent<UIManager>();
    }

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

    // Generates enemies on the possible point points
    public void GenerateEnemies()
    {
        if (enemy == null)
        {
            int index = Random.Range(0, enemySpawnPoints.Count);
            GridElement spawnGrid = enemySpawnPoints[index];
            enemy = Instantiate(enemyPrefab,
                                    spawnGrid.transform.position + Vector3.up * enemyPrefab.GetComponent<Pathfinding>().maxYDiff,
                                    Quaternion.identity);
            enemyPath = enemy.GetComponent<Pathfinding>();
            enemyPath.SetGrid();
            enemyGrid = enemyPath.GetGrid();
        }
    }

    // Generates player on grid
    public void GeneratePlayers()
    {
        if (ui == null)
        {
            ui = GetComponent<UIManager>();
        }
        ui.ShowHintText("Left Click on any of the highlighted grids to spawn the player...");
        foreach (GridElement element in playerSpawnPoints)
        {
            element.SpawnHighlight();
        }
    }

    // Spawns a new player at given grid
    public void SpawnNewPlayer(GridElement element)
    {
        if (playerSpawnPoints.Contains(element))
        {
            foreach (GridElement grid in playerSpawnPoints)
            {
                grid.HideHighlight();
            }
            ui.HideHint();

            player = Instantiate(playerPrefab,
                        element.transform.position + Vector3.up * playerPrefab.GetComponent<Pathfinding>().maxYDiff,
                        Quaternion.identity);
            playerPath = player.GetComponent<Pathfinding>();
            playerPath.SetGrid();
            playerGrid = playerPath.GetGrid();
            Camera.main.GetComponent<CameraFollow>().SetTarget(player);

            turn = TurnType.PLAYER;
            ui.SetTurnUI(turn, turnCounter);
            gameStarted = true;
        }
    }

    // Starts the next turn
    public void NextTurn()
    {
        playerGrid.HideHighlight();
        enemyGrid.HideHighlight();
        if (turn == TurnType.PLAYER)
        {
            turn = TurnType.ENEMY;
            Camera.main.GetComponent<CameraFollow>().SetTarget(enemy);
        }
        else
        {
            enemyPath.StopCoroutine("RotateToTarget");
            enemyPath.StartCoroutine("RotateToTarget", playerGrid.transform.position);
            turn = TurnType.PLAYER;
            Camera.main.GetComponent<CameraFollow>().SetTarget(player);
            Camera.main.GetComponent<CameraFollow>().SetMotion(false);
        }
        turnCounter++;
        ui.SetTurnUI(turn, turnCounter);
    }

    public bool isPlayerMoving()
    {
        return playerPath.moving;
    }

    public bool isEnemyMoving()
    {
        return enemyPath.moving;
    }

    public void MovePlayer(GridElement target)
    {
        Path path = playerPath.GetPath(target);
        playerGrid.PathHighlight(false);
        Camera.main.GetComponent<CameraFollow>().SetTarget(player);
        Camera.main.GetComponent<CameraFollow>().SetMotion(true);
        playerPath.MoveViaPath(path);
    }

    public void MoveEnemy()
    {
        playerGrid = playerPath.GetGrid();
        enemyGrid = enemyPath.GetGrid();
        List<Path> completePaths = new List<Path>();
        int minLenC = int.MaxValue;
        int minDistC = int.MaxValue;
        List<Path> incompletePaths = new List<Path>();
        int minLenIC = int.MaxValue;
        int minDistIC = int.MaxValue;
        foreach (GridElement neighbour in playerGrid.neighbours)
        {
            if (neighbour.IsTraversable(false))
            {
                Path nPath = enemyPath.GetPath(neighbour);
                int pDist = nPath.GetPathDistance() + enemyGrid.GetDistance(nPath.elements[0]);
                if (nPath.IsCompletePath(neighbour))
                {
                    if (nPath.length < minLenC)
                    {
                        minLenC = nPath.length;
                        minDistC = pDist;
                        completePaths.Insert(0, nPath);
                    }
                    else if (nPath.length == minLenC)
                    {
                        if (pDist < minDistC)
                        {
                            minDistC = pDist;
                            completePaths.Insert(0, nPath);
                        }
                    }
                }
                else
                {
                    if (nPath.length < minLenIC)
                    {
                        minLenIC = nPath.length;
                        minDistIC = pDist;
                        incompletePaths.Insert(0, nPath);

                    }
                    else if (nPath.length == minLenIC)
                    {
                        if (pDist < minDistIC)
                        {
                            minDistIC = pDist;
                            incompletePaths.Insert(0, nPath);
                        }
                    }
                }
            }
        }
        Path path = new Path();
        if (completePaths.Count > 0)
        {
            path = completePaths[0];
        }
        else
        {
            path = incompletePaths[0];
        }
        enemyGrid.PathHighlight(false);
        enemyPath.MoveViaPath(path);
    }
}
