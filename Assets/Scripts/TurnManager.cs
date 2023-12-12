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

    public GameObject current = null; // Reference to the current gameobject

    public float turnTimerThreshold = 1.0f; // Time taken for the turn timer to fill for a character

    public List<GameObject> turnQueue = new List<GameObject>(); // The Turn Order

    public int queueSize = 7; // Size of the queue to maintain

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

    private bool actedThisTurn = false; // Whether the character has acted this turn

    public float titleUITime = 1.5f; // Time for which the title screen is shown

    public float gameStartTime = 0.75f; // Time to spend on starting game message

    private CameraFollow cam; // Reference to the camera

    public float enemyTurnWaitTime = 1.5f;

    void Start()
    {
        ui = GetComponent<UIManager>();
        SetupLevel();
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

    // Sets up the level for gameplay
    public void SetupLevel()
    {
        GenerateEnemies();
        ui.StartCoroutine("ShowTitleUI", titleUITime);
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

    // Starts the next turn
    public void NextTurn()
    {
        playerGrid.HideHighlight();
        playerGrid = playerPath.GetGrid();
        enemyGrid.HideHighlight();
        enemyGrid = enemyPath.GetGrid();
        current = turnQueue[0];
        turnQueue.RemoveAt(0);
        current.GetComponent<Stats>().ResetActions();
        actedThisTurn = false;
        StartCoroutine("StartTurn");
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
        ui.SetActionsUI(false, true);
        if (turn == TurnType.PLAYER)
        {
            highlightedGrids = playerPath.GetConnectedGrids();
            foreach (GridElement grid in highlightedGrids)
            {
                grid.ActionHighlight();
            }
            ui.ShowHintText("Click on the highlighted grids to move", false);
        }
    }

    // Moves back to action menu for the character
    public void BackToActionsMenu()
    {
        ui.HideUI(turn == TurnType.PLAYER);
        ui.SetActionsUI(true, false);
        ui.SetTurnUI(turn, turnCounter, current, turnQueue);
        if (turn == TurnType.PLAYER)
        {
            playerGrid.HideHighlight();
            playerGrid = playerPath.GetGrid();
            cam.SetDrag(true);
        }
    }

    // Hides the possible move grids for the current character
    public void HideMoveGrids()
    {
        foreach (GridElement grid in highlightedGrids)
        {
            grid.HideHighlight();
        }
        BackToActionsMenu();
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
            target.ShowHighlight();
            turn = TurnType.NONE;
            StartCoroutine("MoveCharacterAlongPath", path);
        }
        else
        {
            ui.ShowHintTemp("CANNOT TRAVEL TO TARGET GRID!", 0.75f);
        }
    }

    // Moves the enemy to a player grid
    public Path GetEnemyPath()
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
        path.FixForGrids(highlightedGrids);
        return path;
    }

    // Checks if attack action can be done
    public bool CheckAttackAction()
    {
        bool status = true;
        if (!actedThisTurn)
        {
            if (!playerGrid.canActOnGrid || playerStats.actions < 1)
            {
                status = false;
                ui.ShowHintTemp("CANNOT EXECUTE ACTION THIS TURN!", 0.75f);
            }
        }
        else
        {
            status = false;
            ui.ShowHintTemp("CANNOT EXECUTE ACTION ON THIS BLOCK!", 0.75f);
        }
        return status;
    }

    // Starts the attack action for the current character
    public void ShowAttackGrids()
    {
        ui.SetActionsUI(false, true);
        // IMPLEMENTATION HERE LATER
        if (playerStats.actions < playerStats.character.actions) // Has moved this turn
        {
            playerStats.UseActions(playerStats.actions); // Use all remaining actions
        }
        else // Has not moved this turn
        {
            playerStats.UseActions(1);
        }
        actedThisTurn = true;
        CheckTurnStatus();
    }

    // Checks if the character has used all their actions
    public void CheckTurnStatus()
    {
        if (turn == TurnType.PLAYER)
        {
            if (playerStats.actions == 0)
            {
                StartCoroutine("EndTurn");
            }
            else
            {
                BackToActionsMenu();
            }
        }
        else
        {
            StartCoroutine("EndTurn");
        }
    }

    // Starts the game getting the turn order
    IEnumerator StartGame()
    {
        ui.HideUI(true);
        cam.SetDrag(false);
        yield return new WaitForSeconds(gameStartTime / 2.0f);
        ui.ShowHintText("Starting Game...", false);
        gameStarted = true;
        float timer = 0.0f;
        while (turnQueue.Count < queueSize || timer < gameStartTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ui.HideHint();
        yield return new WaitForSeconds(gameStartTime / 2.0f);
        NextTurn();
    }

    // Starts the player spawning process
    public IEnumerator StartPlayerSpawning()
    {
        cam = Camera.main.GetComponent<CameraFollow>();
        yield return cam.StartCoroutine("SnapToTarget", playerSpawnPoints[0].gameObject);
        if (ui == null)
        {
            ui = GetComponent<UIManager>();
        }
        ui.ShowHintText("Click on the highlighted grids to spawn the player", false);
        foreach (GridElement element in playerSpawnPoints)
        {
            element.ActionHighlight();
        }
        GetComponent<InputManager>().SetInput(true);
        playerPrefab.GetComponent<Stats>().SetStats();
        ui.SetCharacterUI(true, true, playerPrefab.GetComponent<Stats>());
    }

    // Confirms the player's choice regarding player spawning
    IEnumerator ConfirmPlayerSpawning(GridElement element)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        if (playerSpawnPoints.Contains(element))
        {
            GetComponent<InputManager>().SetInput(false);
            element.ShowHighlight();
            ui.ShowHintText("Press Enter/Left-Click to confirm spawning this character at the selected grid. Right Click to go back", false);
            GameObject tempPlayer = Instantiate(playerPrefab,
                                    element.transform.position + Vector3.up * playerPrefab.GetComponent<Pathfinding>().maxYDiff,
                                    Quaternion.identity);
            while (true)
            {
                if (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Return))
                {
                    foreach (GridElement grid in playerSpawnPoints)
                    {
                        grid.HideHighlight();
                    }
                    ui.HideHint();

                    player = tempPlayer;
                    playerStats = player.GetComponent<Stats>();
                    playerPath = player.GetComponent<Pathfinding>();
                    playerPath.SetGrid();
                    playerGrid = playerPath.GetGrid();
                    break;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    GetComponent<InputManager>().SetInput(true);
                    Destroy(tempPlayer);
                    element.ActionHighlight();
                    ui.ShowHintText("Click on the highlighted grids to spawn the player", false);
                    yield break;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            StartCoroutine("StartGame");
        }
        else
        {
            ui.ShowHintTemp("CANNOT SPAWN PLAYER AT THAT GRID!", 0.75f);
        }
    }

    // Starts the next turn
    IEnumerator StartTurn()
    {
        yield return cam.StartCoroutine("SnapToTarget", current);
        if (current == player)
        {
            turn = TurnType.PLAYER;
        }
        else
        {
            turn = TurnType.ENEMY;
            cam.SetDrag(false);
            StartCoroutine("StartEnemyTurn");
        }
        turnCounter++;
        ui.SetTurnUI(turn, turnCounter, current, turnQueue);
    }

    // Moves the character along the path
    public IEnumerator MoveCharacterAlongPath(Path path)
    {
        ui.HideUI(current.GetComponent<Pathfinding>().isPlayer);
        yield return cam.StartCoroutine("SnapToTarget", current);
        cam.SetDrag(false);
        turn = current.GetComponent<Pathfinding>().isPlayer ? TurnType.PLAYER : TurnType.ENEMY;
        current.GetComponent<Pathfinding>().GetGrid().PathHighlight(false);
        current.GetComponent<Pathfinding>().MoveViaPath(path);
    }

    // Ends the turn with the player choosing the snap direction
    IEnumerator EndTurn()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        ui.HideUI(current.GetComponent<Pathfinding>().isPlayer);
        if (turn == TurnType.PLAYER)
        {
            playerGrid.HideHighlight();
            ui.HideTurnUI(true);
            ui.ResetGridElementUI();
            turn = TurnType.NONE;
            ui.ShowHintText("Use WSAD/Arrow Keys to choose direction to snap to. Press Enter/Left-Click to end turn", false);
            ui.SetActionsUI(false, false);
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

    // Starts the enemy turn showing the grids and initiating action
    IEnumerator StartEnemyTurn()
    {
        yield return new WaitForSeconds(enemyTurnWaitTime);
        highlightedGrids = enemyPath.GetConnectedGrids();
        foreach (GridElement grid in highlightedGrids)
        {
            grid.ActionHighlight();
        }
        yield return new WaitForSeconds(enemyTurnWaitTime / 2.0f);
        foreach (GridElement grid in highlightedGrids)
        {
            grid.HideHighlight();
        }
        Path path = GetEnemyPath();
        GridElement targetGrid = path.elements[path.length - 1];
        targetGrid.ShowHighlight();
        ui.HideUI(current.GetComponent<Pathfinding>().isPlayer);
        yield return cam.StartCoroutine("SnapToTarget", targetGrid.gameObject);
        StartCoroutine("MoveCharacterAlongPath", path);
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
