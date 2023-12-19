using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIManager ui; // UI Manager reference

    private TurnManager turnManager; // Turn Manager reference

    private CameraFollow cam; // Camera Follow reference

    private GridElement currentGrid = null; // Reference to the last grid 

    public bool canInput = false; // Whether the player can input

    void Start()
    {
        ui = GetComponent<UIManager>();
        turnManager = GetComponent<TurnManager>();
        cam = Camera.main.GetComponent<CameraFollow>();
    }

    // Hides the current grid highlight
    public void HideCurrentHighlight(Path path)
    {
        if (currentGrid != null)
        {
            ui.ResetGridElementUI();
            if (currentGrid.isActionGrid) // If Action grid, highlight that color
            {
                currentGrid.ActionHighlight();
            }
            else
            {
                if (path != null && path.elements.Contains(currentGrid)) // If Part of path grids, then show that color 
                {
                    return;
                }
                else
                {
                    currentGrid.HideHighlight();
                }
            }
            currentGrid = null;
        }
    }

    // Sets the Input state
    public void SetInput(bool status)
    {
        HideCurrentHighlight(null);
        canInput = status;
    }

    // Checks the input status
    public bool GetInputStatus()
    {
        bool status = false;
        switch (turnManager.turn)
        {
            case TurnType.NONE: // Player spawning
                if (turnManager.phase == TurnPhase.SPAWN)
                {
                    status = true;
                }
                break;
            case TurnType.PLAYER: // Player's turn
                if (turnManager.phase == TurnPhase.MENU || turnManager.phase == TurnPhase.MOVE || turnManager.phase == TurnPhase.ATTACK)
                {
                    status = true;
                }
                break;
            default:
                break;
        }
        // canInput = status;
        return status;
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
                        HideCurrentHighlight(null);
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
                    HideCurrentHighlight(null);
                }
            }
            if (turnManager.turn == TurnType.ENEMY)
            {
                HideCurrentHighlight(null);
            }
        }
        else
        {
            if (canInput)
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
                        HideCurrentHighlight(null);
                    }
                    currentGrid = hit.collider.gameObject.GetComponent<GridElement>();
                    currentGrid.ShowHighlight();
                }
                else
                {
                    HideCurrentHighlight(null);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Zoom In
        {
            Camera.main.GetComponent<Camera>().orthographicSize -= 0.25f;
            Camera.main.GetComponent<Camera>().orthographicSize = CustomMath.ClampF(
                Camera.main.GetComponent<Camera>().orthographicSize,
                cam.camZoomLimits.x, cam.camZoomLimits.y);
        }
        if (Input.GetKeyDown(KeyCode.E)) // Zoom Out
        {
            Camera.main.GetComponent<Camera>().orthographicSize += 0.25f;
            Camera.main.GetComponent<Camera>().orthographicSize = CustomMath.ClampF(
               Camera.main.GetComponent<Camera>().orthographicSize,
               cam.camZoomLimits.x, cam.camZoomLimits.y);
        }
        if (GetInputStatus())
        {
            if (Input.GetMouseButtonDown(1)) // Drag Start
            {
                cam.StartDrag();
            }
            if (!Input.GetMouseButton(1)) // Release Dragging
            {
                cam.StopDrag();
            }
            if (Input.GetKeyDown(KeyCode.F)) // Set back to snap
            {
                cam.Snap();
            }
        }
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
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        HideCurrentHighlight(null);
                        turnManager.HideMoveGrids();
                    }
                }
            }
        }
        else
        {
            if (canInput)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement")))
                    {
                        GridElement element = hit.collider.gameObject.GetComponent<GridElement>();
                        turnManager.StartCoroutine("ConfirmPlayerSpawning", element);
                    }
                }
            }
        }
    }
}
