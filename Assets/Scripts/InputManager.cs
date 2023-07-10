using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIManager ui; // UI Manager reference

    private TurnManager turnManager; // Turn Manager reference

    public GameObject player; // Player reference

    private Pathfinding playerPath; // Player pathfinding reference

    public GameObject enemy; // Enemy reference

    private Pathfinding enemyPath; // Enemy pathfinding reference

    private GridElement currentGrid = null; // Reference to the last grid 

    void Start()
    {
        ui = GetComponent<UIManager>();
        turnManager = GetComponent<TurnManager>();
        ui.SetTurnUI(turnManager.turn);
        playerPath = player.GetComponent<Pathfinding>();
        playerPath.SetGrid();
        enemyPath = enemy.GetComponent<Pathfinding>();
        enemyPath.SetGrid();
    }

    void FixedUpdate()
    {
        if (turnManager.turn == TurnType.PLAYER && !playerPath.moving)
        {

            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Hovering over a Grid Element
            if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement")))
            {
                if (currentGrid != null)
                {
                    if (hit.collider.gameObject.GetComponent<GridElement>() == currentGrid)
                    {
                        return;
                    }
                    currentGrid.HideHighlight();
                }
                currentGrid = hit.collider.gameObject.GetComponent<GridElement>();
                ui.SetGridElementUI(currentGrid);
                currentGrid.ShowHighlight();
            }
            else
            {
                if (currentGrid != null)
                {
                    ui.ResetGridElementUI();
                    currentGrid.HideHighlight();
                    currentGrid = null;
                }
            }
        }
        if (turnManager.turn == TurnType.ENEMY)
        {
            if (currentGrid != null)
            {
                ui.ResetGridElementUI();
                currentGrid.HideHighlight();
                currentGrid = null;
            }
        }
    }

    void Update()
    {
        // Player's Turn
        if (turnManager.turn == TurnType.PLAYER && !playerPath.moving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement")))
                {
                    GridElement element = hit.collider.gameObject.GetComponent<GridElement>();
                    if (element.IsTraversable(true))
                    {
                        Path path = playerPath.GetPath(element);
                        playerPath.MoveViaPath(path);
                    }
                }
            }
        }

        // Enemy's Turn
        if (turnManager.turn == TurnType.ENEMY && !enemyPath.moving)
        {
            GridElement playerGrid = playerPath.GetGrid();
            GridElement enemyGrid = enemyPath.GetGrid();
            int minLen = int.MaxValue;
            int minDist = int.MaxValue;
            GridElement target = null;
            foreach (GridElement neighbour in playerGrid.neighbours)
            {
                if (neighbour.IsTraversable(false))
                {
                    Path nPath = enemyPath.GetPath(neighbour);
                    int pDist = nPath.GetPathDistance() + enemyGrid.GetDistance(nPath.elements[0]);
                    if (nPath.length < minLen)
                    {
                        minLen = nPath.length;
                        minDist = pDist;
                        target = neighbour;
                    }
                    else if (nPath.length == minLen)
                    {
                        if (pDist < minDist)
                        {
                            minDist = pDist;
                            target = neighbour;
                        }
                    }
                }
            }
            Path path = enemyPath.GetPath(target);
            enemyPath.MoveViaPath(path);
        }
    }
}
