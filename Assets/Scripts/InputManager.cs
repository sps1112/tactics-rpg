using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIManager ui; // UI Manager reference

    private TurnManager turnManager;

    public GameObject player;

    public GameObject enemy;

    private Pathfinding playerPath;

    private Pathfinding enemyPath;

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
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement")))
        {
            ui.SetGridElementUI(hit.collider.gameObject.GetComponent<GridElement>());
        }
        else
        {
            ui.ResetGirdElementUI();
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GetComponent<ObstacleManager>().GenerateObstacles();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GetComponent<ObstacleManager>().DeleteObstacles();
        }
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
        if (turnManager.turn == TurnType.ENEMY && !enemyPath.moving)
        {
            GridElement playerGrid = playerPath.GetGrid();
            GridElement enemyGrid = enemyPath.GetGrid();
            int index = -1;
            int minLen = int.MaxValue;
            int minDist = int.MaxValue;
            int i = 0;
            foreach (GridElement neighbour in playerGrid.neighbours)
            {
                // Debug.Log("Possible target is:- (" + neighbour.pos.x + ", " + neighbour.pos.y + ")");
                if (neighbour.IsTraversable(false))
                {
                    // Debug.Log("Target is traversable");
                    Path nPath = enemyPath.GetPath(neighbour);
                    int pDist = nPath.GetPathDistance() + enemyGrid.GetDistance(nPath.elements[0]);
                    // Debug.Log("Path " + i + "'s length is:- " + nPath.length + " and distance is:- " + pDist);
                    if (nPath.length < minLen)
                    {
                        minLen = nPath.length;
                        minDist = pDist;
                        index = i;
                    }
                    else if (nPath.length == minLen)
                    {
                        if (pDist < minDist)
                        {
                            minDist = pDist;
                            index = i;
                        }
                    }

                }
                else
                {
                    // Debug.Log("Target is NOT traversable");
                }
                i++;
            }
            Debug.Log("Final target is:- (" + playerGrid.neighbours[index].pos.x + ", " + playerGrid.neighbours[index].pos.y + ")");
            Debug.Log("Min Path is:- " + index + " with length:- " + minLen + " and distance:- " + minDist);
            Path path = enemyPath.GetPath(playerGrid.neighbours[index]);
            enemyPath.MoveViaPath(path);
        }
    }
}
