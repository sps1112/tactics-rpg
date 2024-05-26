using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIManager ui; // UI Manager reference

    private TurnManager turnManager; // Turn Manager reference

    private Camera cam; // Camera component reference

    private CameraFollow camFollow; // Camera Follow reference

    private GridElement currentGrid = null; // Reference to the last grid 

    private bool canInput = false; // Whether the player can input

    private bool canScan = false; // Whether the cursor scans and clicks on grid

    void Start()
    {
        ui = GetComponent<UIManager>();
        turnManager = GetComponent<TurnManager>();
        cam = Camera.main.GetComponent<Camera>();
        camFollow = Camera.main.GetComponent<CameraFollow>();
    }

    // Sets the Input state
    public void SetInput(bool status, bool scanStatus)
    {
        HideHighlight(); // Reset current highlight
        canInput = status;
        canScan = scanStatus;
    }

    // Shows the current grid highlight
    private void ShowHighlight(GridElement element)
    {
        if (currentGrid != null) // A grid is already being highlighted
        {
            if (element != currentGrid) // If the new grid, is different, then hide the previous one
            {
                HideHighlight();
            }
        }
        ui.SetGridElementUI(element);
        element.ShowHighlight();
        currentGrid = element;
    }

    // Hides the current grid highlight
    public void HideHighlight()
    {
        if (currentGrid != null) // Already highlighting something
        {
            // Hide UI
            ui.ResetGridElementUI();
            // Hide grid highlight
            if (currentGrid.isActionGrid) // If the grid is an action grid, then keep showing that color
            {
                currentGrid.ActionHighlight();
            }
            else // Else, hide the highlight
            {
                currentGrid.HideHighlight();
            }
            currentGrid = null;
        }
    }

    void FixedUpdate()
    {
        if (canScan || (turnManager.turn == TurnType.PLAYER && turnManager.phase == TurnPhase.MENU && Input.GetKey(KeyCode.LeftShift))) // Can scan grids
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement"))) // Cursor on grid
            {
                ShowHighlight(hit.collider.gameObject.GetComponent<GridElement>());
            }
            else // Not on any grid
            {
                HideHighlight();
            }
        }
        else
        {
            HideHighlight(); // Hide any grid if it currently being hightlighted
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) // Toggle pause menu on or off
        {
            ui.TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.Q)) // Zoom In
        {
            cam.orthographicSize -= 0.33f;
            cam.orthographicSize = CustomMath.ClampF(
                cam.orthographicSize,
                camFollow.camZoomLimits.x, camFollow.camZoomLimits.y);
        }
        if (Input.GetKeyDown(KeyCode.E)) // Zoom Out
        {
            cam.orthographicSize += 0.33f;
            cam.orthographicSize = CustomMath.ClampF(
                cam.orthographicSize,
                camFollow.camZoomLimits.x, camFollow.camZoomLimits.y);
        }
        if (canInput) // Can Input
        {
            if (Input.GetMouseButtonDown(1)) // Drag Start
            {
                camFollow.StartDrag();
            }
            if (!Input.GetMouseButton(1)) // Release Dragging
            {
                camFollow.StopDrag();
            }
            if (Input.GetKeyDown(KeyCode.F)) // Set back to snap
            {
                camFollow.Snap();
            }
            if (canScan) // Can click on a grid
            {
                if (Input.GetMouseButtonUp(0)) // Left Click
                {
                    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement")))
                    {
                        turnManager.ProcessGridClick(hit.collider.gameObject.GetComponent<GridElement>());
                    }
                }
                if (Input.GetKeyDown(KeyCode.Space)) // Undo process
                {
                    turnManager.UndoProcess();
                }
            }
        }
    }
}
