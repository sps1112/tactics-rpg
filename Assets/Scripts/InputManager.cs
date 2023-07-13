using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIManager ui; // UI Manager reference

    private TurnManager turnManager; // Turn Manager reference

    private GridElement currentGrid = null; // Reference to the last grid 

    public bool canInput = false; // Whether the player can input

    void Start()
    {
        ui = GetComponent<UIManager>();
        turnManager = GetComponent<TurnManager>();
    }

    // Sets the Input state
    public void SetInput(bool status)
    {
        canInput = status;
    }

    // Hides the current grid highlight
    void HideCurrentHighlight()
    {
        if (currentGrid != null)
        {
            ui.ResetGridElementUI();
            currentGrid.HideHighlight();
            currentGrid = null;
        }
    }

    void FixedUpdate()
    {
        if (turnManager.gameStarted)
        {
            if (turnManager.turn == TurnType.PLAYER && !turnManager.isPlayerMoving())
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
                    if (canInput)
                    {
                        currentGrid = hit.collider.gameObject.GetComponent<GridElement>();
                        ui.SetGridElementUI(currentGrid);
                        currentGrid.ShowHighlight();
                    }
                }
                else
                {
                    HideCurrentHighlight();
                }
            }
            if (turnManager.turn == TurnType.ENEMY)
            {
                HideCurrentHighlight();
            }
        }
        else
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
                    if (turnManager.playerSpawnPoints.Contains(currentGrid))
                    {
                        currentGrid.SpawnHighlight();
                    }
                    else
                    {
                        currentGrid.HideHighlight();
                    }
                }
                currentGrid = hit.collider.gameObject.GetComponent<GridElement>();
                currentGrid.ShowHighlight();
            }
            else
            {
                if (currentGrid != null)
                {
                    if (turnManager.playerSpawnPoints.Contains(currentGrid))
                    {
                        currentGrid.SpawnHighlight();
                    }
                    else
                    {
                        currentGrid.HideHighlight();
                    }
                    currentGrid = null;
                }
            }
        }
    }

    void Update()
    {
        if (turnManager.gameStarted)
        {
            // Player's Turn
            if (turnManager.turn == TurnType.PLAYER && !turnManager.isPlayerMoving())
            {
                if (canInput)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement")))
                        {
                            GridElement element = hit.collider.gameObject.GetComponent<GridElement>();
                            if (element.IsTraversable(true))
                            {
                                turnManager.MovePlayer(element);
                            }
                        }
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        HideCurrentHighlight();
                        GetComponent<UIManager>().SetActionsUI(true);
                    }
                }
            }

            // Enemy's Turn
            if (turnManager.turn == TurnType.ENEMY && !turnManager.isEnemyMoving())
            {
                turnManager.MoveEnemy();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement")))
                {
                    GridElement element = hit.collider.gameObject.GetComponent<GridElement>();
                    turnManager.SpawnNewPlayer(element);
                }
            }
        }
    }
}
