using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines the types of turns available
public enum TurnType
{
    NONE, // No Turn
    PLAYER, // Player's Turn
    ENEMY, // Enemy's Turn
}

// The various phases which make up a turn
public enum TurnPhase
{
    NONE, // No phase
    SPAWN, // Spawning a player character
    CHECK, // Checking whether action left to go to menu
    MENU, // Choosing action in Action Menu
    SNAPPING, // Snapping to character
    MOVE, // Chossing grid to move to
    MOVING, // Moving to chosen grid
    ATTACK, // Choosing grid to attack
    ATTACKING, // Attacking chosen grid
    ENDING, // Ending this turn
}

public class TurnManager : MonoBehaviour
{
    private UIManager ui = null; // UI Manager reference

    private LevelManager levelManager; // Level Manager reference

    public TurnType turn = TurnType.NONE; // Current Turn Type

    public TurnPhase phase = TurnPhase.NONE; // Turn's phase

    [SerializeField]
    private TurnPhase prevPhase = TurnPhase.NONE; // Turn's previous phase

    public int turnCounter = 0; // Count for the turns

    public GameObject current = null; // Reference to the current gameobject

    public float turnTimerThreshold = 1.0f; // Time taken for the turn timer to fill for a character

    public List<GameObject> turnQueue = new List<GameObject>(); // The Turn Order

    public int queueSize = 7; // Size of the queue to maintain

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
        levelManager = GetComponent<LevelManager>();
        SetupLevel();
    }

    // Sets up the level for gameplay
    private void SetupLevel()
    {
        GenerateEnemies();
        ui.StartCoroutine("ShowTitleUI", titleUITime);
    }

    // Generates enemies on the possible point points
    private void GenerateEnemies()
    {
        if (enemy == null)
        {
            int index = Random.Range(0, levelManager.enemySpawnPoints.Count);
            GridElement spawnGrid = levelManager.enemySpawnPoints[index];
            enemy = Instantiate(enemyPrefab,
                                    spawnGrid.transform.position + Vector3.up * enemyPrefab.GetComponent<Pathfinding>().maxYDiff,
                                    Quaternion.identity);
            enemyStats = enemy.GetComponent<Stats>();
            enemyPath = enemy.GetComponent<Pathfinding>();
            enemyPath.SetGrid();
            enemyGrid = enemyPath.GetGrid();
        }
    }

    // Sets the turn phase to the given phase
    public void SetPhase(TurnPhase phase_)
    {
        prevPhase = phase;
        phase = phase_;
    }

    // Restores the phase to the previous phase
    public void RestorePhase()
    {
        phase = prevPhase;
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
        SetPhase(TurnPhase.MOVE);
        ui.HideTurnUI();
        ui.SetActionsUI(false);
        GetComponent<InputManager>().SetInput(true);
        if (turn == TurnType.PLAYER)
        {
            highlightedGrids = playerPath.GetConnectedGrids();
            foreach (GridElement grid in highlightedGrids)
            {
                grid.ActionHighlight();
            }
            ui.ShowHint("Click on the highlighted grids to move", false);
        }
    }

    // Moves back to action menu for the character
    public void BackToActionsMenu()
    {
        SetPhase(TurnPhase.MENU);
        ui.HideUI();
        ui.SetTurnUI(turn, turnCounter, current, turnQueue);
        if (turn == TurnType.PLAYER)
        {
            playerGrid.HideHighlight();
            playerGrid = playerPath.GetGrid();
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
            StartCoroutine("MoveCharacterAlongPath", path);
        }
        else
        {
            ui.ShowHint("CANNOT TRAVEL TO TARGET GRID!", true);
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
        if (!actedThisTurn) // Not performed an action this turn
        {
            if (!playerGrid.canActOnGrid || playerStats.actions < 1)
            {
                status = false;
                if (!playerGrid.canActOnGrid) // Currently on a no-action grid
                {
                    ui.ShowHint("CANNOT EXECUTE ACTION ON THIS BLOCK!", true);
                }
                else // Depleted action points (by some condition)
                {
                    ui.ShowHint("CANNOT EXECUTE ACTION THIS TURN!", true);
                }
            }
        }
        else // Already performed an action this turn
        {
            status = false;
            ui.ShowHint("CANNOT EXECUTE ACTION THIS TURN!", true);
        }
        return status;
    }

    // Starts the attack action for the current character
    public void ShowAttackGrids()
    {
        SetPhase(TurnPhase.ATTACK);
        ui.SetActionsUI(false);
        GetComponent<InputManager>().SetInput(true);
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
        SetPhase(TurnPhase.CHECK);
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
    private IEnumerator StartGame()
    {
        ui.HideUI();
        yield return new WaitForSeconds(gameStartTime / 2.0f);
        ui.ShowHint("Starting Game...", false);
        gameStarted = true;
        float timer = 0.0f;
        while (turnQueue.Count < queueSize || timer < gameStartTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ui.HideHint(false);
        yield return new WaitForSeconds(gameStartTime / 2.0f);
        NextTurn();
    }

    // Starts the player spawning process
    public IEnumerator StartPlayerSpawning()
    {
        cam = Camera.main.GetComponent<CameraFollow>();
        yield return cam.StartCoroutine("SnapToTarget", levelManager.playerSpawnPoints[0].gameObject);
        SetPhase(TurnPhase.SPAWN);
        if (ui == null)
        {
            ui = GetComponent<UIManager>();
        }
        ui.ShowHint("Click on the highlighted grids to spawn the player", false);
        foreach (GridElement element in levelManager.playerSpawnPoints)
        {
            element.ActionHighlight();
        }
        GetComponent<InputManager>().SetInput(true);
        playerPrefab.GetComponent<Stats>().SetStats();
        ui.SetCharacterUI(true, true, playerPrefab.GetComponent<Stats>()); // Show Player UI
    }

    // Confirms the player's choice regarding player spawning
    public IEnumerator ConfirmPlayerSpawning(GridElement element)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        if (levelManager.playerSpawnPoints.Contains(element))
        {
            GetComponent<InputManager>().SetInput(false);
            element.ShowHighlight();
            ui.ShowHint("Press Enter/Left-Click to confirm spawning this character at the selected grid. Space to go back", false);
            GameObject tempPlayer = Instantiate(playerPrefab,
                                    element.transform.position + Vector3.up * playerPrefab.GetComponent<Pathfinding>().maxYDiff,
                                    Quaternion.identity);
            while (true)
            {
                if (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Return))
                {
                    foreach (GridElement grid in levelManager.playerSpawnPoints)
                    {
                        grid.HideHighlight();
                    }
                    ui.HideHint(false);

                    player = tempPlayer;
                    playerStats = player.GetComponent<Stats>();
                    playerPath = player.GetComponent<Pathfinding>();
                    playerPath.SetGrid();
                    playerGrid = playerPath.GetGrid();
                    break;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    GetComponent<InputManager>().SetInput(true);
                    Destroy(tempPlayer);
                    element.ActionHighlight();
                    ui.ShowHint("Click on the highlighted grids to spawn the player", false);
                    yield break;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            StartCoroutine("StartGame");
        }
        else
        {
            ui.ShowHint("CANNOT SPAWN PLAYER AT THAT GRID!", true);
        }
    }

    // Starts the next turn
    private IEnumerator StartTurn()
    {
        yield return cam.StartCoroutine("SnapToTarget", current);
        SetPhase(TurnPhase.CHECK);
        if (current == player)
        {
            turn = TurnType.PLAYER;
        }
        else
        {
            turn = TurnType.ENEMY;
            StartCoroutine("StartEnemyTurn");
        }
        turnCounter++;
        ui.SetTurnUI(turn, turnCounter, current, turnQueue);
        SetPhase(TurnPhase.MENU);
    }

    // Moves the character along the path
    public IEnumerator MoveCharacterAlongPath(Path path)
    {
        ui.HideUI();
        yield return cam.StartCoroutine("SnapToTarget", current);
        SetPhase(TurnPhase.MOVING);
        turn = current.GetComponent<Pathfinding>().isPlayer ? TurnType.PLAYER : TurnType.ENEMY;
        current.GetComponent<Pathfinding>().GetGrid().PathHighlight(false);
        current.GetComponent<Pathfinding>().MoveViaPath(path);
    }

    // Ends the turn with the player choosing the snap direction
    public IEnumerator EndTurn()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        SetPhase(TurnPhase.ENDING);
        GetComponent<InputManager>().SetInput(false);
        ui.HideUI();
        if (turn == TurnType.PLAYER)
        {
            playerGrid.HideHighlight();
            ui.ShowHint("Use WSAD/Arrow Keys to choose direction to snap to. Press Enter/Left-Click to end turn", false);
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
            ui.HideHint(false);
        }
        else
        {
            enemyPath.StopCoroutine("RotateToTarget");
            enemyPath.StartCoroutine("RotateToTarget", playerGrid.transform.position);
        }
        SetPhase(TurnPhase.NONE);
        NextTurn();
    }

    // Starts the enemy turn showing the grids and initiating action
    IEnumerator StartEnemyTurn()
    {
        yield return new WaitForSeconds(enemyTurnWaitTime);
        SetPhase(TurnPhase.MOVE);
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
        ui.HideUI();
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
