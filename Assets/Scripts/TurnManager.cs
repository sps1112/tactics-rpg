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
    private UIManager ui; // UI Manager reference

    public TurnType turn; // Current Turn Type

    private int turnCounter = 1; // Count for the turns+

    public GameObject player; // Player reference

    private Pathfinding playerPath; // Player pathfinding reference

    private GridElement playerGrid; // Reference to the last grid occupied by player

    public GameObject enemy; // Enemy reference

    private Pathfinding enemyPath; // Enemy pathfinding reference

    private GridElement enemyGrid; // Reference to the last grid occupied by enemy

    void Start()
    {
        ui = GetComponent<UIManager>();
        turn = TurnType.PLAYER;
        playerPath = player.GetComponent<Pathfinding>();
        playerPath.SetGrid();
        playerGrid = playerPath.GetGrid();
        enemyPath = enemy.GetComponent<Pathfinding>();
        enemyPath.SetGrid();
        enemyGrid = enemyPath.GetGrid();
        Camera.main.GetComponent<CameraFollow>().SetTarget(player);
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
