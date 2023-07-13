using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Defines the types of turn availables
public enum TurnType
{
    NONE, // No Turn
    PLAYER, // Player's Turn
    ENEMY, // Enemy's Turn
}

public class TurnManager : MonoBehaviour
{
    private UIManager ui = null; // UI Manager reference

    public TurnType turn = TurnType.NONE; // Current Turn Type

    public int turnCounter = 0; // Count for the turns+

    public float turnTimerThreshold = 1.0f; // Time taken for the turn timer to fill for a character

    public List<GameObject> turnQueue = new List<GameObject>(); // The Turn Order

    public int queueSize = 7; // Size of the queue to maintain

    public Image[] turnPortaits; // Reference to the turn potraits

    public List<GridElement> enemySpawnPoints = new List<GridElement>(); // Spawn points where enemy can spawn

    public List<GridElement> playerSpawnPoints = new List<GridElement>(); // Spawn points where player can spawn

    public GameObject enemyPrefab; // Reference to the enemy prefab for generating enemies

    public GameObject playerPrefab; // Reference to the player prefab for generating players

    public bool gameStarted = false; // Whether the game has started

    public GameObject player; // Player reference

    public Stats playerStats; // Reference to the player stats

    public Pathfinding playerPath; // Player pathfinding reference

    public GridElement playerGrid; // Reference to the last grid occupied by player

    public GameObject enemy = null; // Enemy reference

    public Stats enemyStats; // Reference to the enemy stats

    public Pathfinding enemyPath; // Enemy pathfinding reference

    public GridElement enemyGrid; // Reference to the last grid occupied by enemy

    public float playerTimer = 0; // Timer for the player

    public float enemyTimer = 0; // Timer for the enemy

    private List<GridElement> highlightedGrids; // List of grids currently being highlighted

    void Start()
    {
        ui = GetComponent<UIManager>();
        GenerateEnemies();
        GeneratePlayers();
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
            enemyStats = enemy.GetComponent<Stats>();
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
            element.ActionHighlight();
        }
        if (playerSpawnPoints.Count > 0)
        {
            Camera.main.GetComponent<CameraFollow>().SetTarget(playerSpawnPoints[0].gameObject);
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
            playerStats = player.GetComponent<Stats>();
            playerPath = player.GetComponent<Pathfinding>();
            playerPath.SetGrid();
            playerGrid = playerPath.GetGrid();

            StartCoroutine("StartGame");
            gameStarted = true;
        }
    }

    // Starts the next turn
    public void NextTurn()
    {
        playerGrid.HideHighlight();
        playerGrid = playerPath.GetGrid();
        enemyGrid.HideHighlight();
        enemyGrid = enemyPath.GetGrid();
        GameObject current = turnQueue[0];
        turnQueue.RemoveAt(0);
        if (current == player)
        {

            turn = TurnType.PLAYER;
            Camera.main.GetComponent<CameraFollow>().SetTarget(player);
            Camera.main.GetComponent<CameraFollow>().SetMotion(false);
        }
        else
        {
            turn = TurnType.ENEMY;
            Camera.main.GetComponent<CameraFollow>().SetTarget(enemy);
        }
        turnCounter++;
        ui.SetTurnUI(turn, turnCounter, current.GetComponent<Stats>());
        for (int i = 0; i < 6; i++)
        {
            if (i == 0)
            {
                turnPortaits[0].sprite = current.GetComponent<Stats>().character.potrait;
            }
            else
            {
                turnPortaits[i].sprite = turnQueue[i - 1].GetComponent<Stats>().character.potrait;
            }
        }
    }

    // Checks if the player is moving
    public bool isPlayerMoving()
    {
        return playerPath.moving;
    }

    // Checks if the enemy is moving
    public bool isEnemyMoving()
    {
        return enemyPath.moving;
    }

    // Shows the possible move grids for the current character
    public void ShowMoveGrids()
    {
        ui.SetActionsUI(false);
        if (turn == TurnType.PLAYER)
        {
            highlightedGrids = playerPath.GetConnectedGrids();
            foreach (GridElement grid in highlightedGrids)
            {
                grid.ActionHighlight();
            }
            ui.ShowHintTemp("Click on highlighted grids to move to...", 1.0f);
        }
    }

    // Hides the possible move grids for the current character
    public void HideMoveGrids()
    {
        ui.SetActionsUI(true);
        ui.HideHint();
        foreach (GridElement grid in highlightedGrids)
        {
            grid.HideHighlight();
        }
    }

    // Moves the player to the target grid
    public void MovePlayer(GridElement target)
    {
        if (highlightedGrids.Contains(target))
        {
            Path path = playerPath.GetPath(target);
            foreach (GridElement grid in highlightedGrids)
            {
                grid.HideHighlight();
            }
            playerGrid.PathHighlight(false);
            Camera.main.GetComponent<CameraFollow>().SetTarget(player);
            Camera.main.GetComponent<CameraFollow>().SetMotion(true);
            playerPath.MoveViaPath(path);
        }
        else
        {
            ui.ShowHintTemp("CANNOT TRAVEL TO TARGET GRID!", 0.75f);
        }
    }

    // Moves the enemy to a player grid
    public void MoveEnemy()
    {
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
                if (Mathf.Abs(playerGrid.height - neighbour.height) <= enemyStats.character.jump)
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

    // Starts the game getting the turn order
    IEnumerator StartGame()
    {
        ui.ShowHintText("Starting Game...");
        gameStarted = true;
        while (turnQueue.Count < queueSize)
        {
            yield return new WaitForSeconds(Time.deltaTime * 10.0f);
        }
        ui.HideHint();
        NextTurn();
    }

    // Ends the turn with the player choosing the snap direction
    IEnumerator EndTurn()
    {
        if (turn == TurnType.PLAYER)
        {
            playerGrid.HideHighlight();
            ui.HideTurnUI(true);
            ui.ResetGridElementUI();
            turn = TurnType.NONE;
            ui.ShowHintText("Use WSAD/Directional Keys to choose a direction to snap to and Enter/Left-Click to end turn...");
            while (true)
            {
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    player.transform.forward = Vector3.forward;
                }
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    player.transform.forward = -Vector3.forward;
                }
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    player.transform.forward = -Vector3.right;
                }
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                {
                    player.transform.forward = Vector3.right;
                }
                if (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Return))
                {
                    break;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            ui.HideHint();
        }
        else
        {
            enemyPath.StopCoroutine("RotateToTarget");
            enemyPath.StartCoroutine("RotateToTarget", playerGrid.transform.position);
        }
        NextTurn();
    }

    void Update()
    {
        if (gameStarted)
        {
            if (turnQueue.Count < queueSize)
            {
                playerTimer += playerStats.character.speed * Time.deltaTime * 10.0f;
                if (playerTimer >= turnTimerThreshold)
                {
                    turnQueue.Add(player);
                    playerTimer -= turnTimerThreshold;
                }
                enemyTimer += enemyStats.character.speed * Time.deltaTime * 10.0f;
                if (enemyTimer >= turnTimerThreshold)
                {
                    turnQueue.Add(enemy);
                    enemyTimer -= turnTimerThreshold;
                }
            }
        }
    }
}
