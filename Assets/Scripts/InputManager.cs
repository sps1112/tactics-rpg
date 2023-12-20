using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIManager ui; // UI Manager reference

    private TurnManager turnManager; // Turn Manager reference

    private CameraFollow cam; // Camera Follow reference

    private GridElement currentGrid = null; // Reference to the last grid 

    private bool canInput = false; // Whether the player can input

    private bool canScan = false; // Whether the cursor scans and clicks on grid

    void Start()
    {
        ui = GetComponent<UIManager>();
        turnManager = GetComponent<TurnManager>();
        cam = Camera.main.GetComponent<CameraFollow>();
    }

    // Sets the Input state
    public void SetInput(bool status, bool scanStatus)
    {
        HideHighlight(null); // Reset current highlight
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
                HideHighlight(null);
            }
        }
        ui.SetGridElementUI(element);
        element.ShowHighlight();
        currentGrid = element;
    }

    // Hides the current grid highlight
    public void HideHighlight(Path path)
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
            else
            {
                if (path != null && path.elements.Contains(currentGrid)) // If Part of path grids, then show that color 
                {
                    return;
                }
                else // if not, hide the highlight
                {
                    currentGrid.HideHighlight();
                }
            }
            currentGrid = null;
        }
    }

    void FixedUpdate()
    {
        if (canScan) // Can scan grids
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement"))) // Cursor on grid
            {
                ShowHighlight(hit.collider.gameObject.GetComponent<GridElement>());
            }
            else // Not on any grid
            {
                HideHighlight(null);
            }
        }
        else
        {
            HideHighlight(null); // Hide any grid if it currently being hightlighted
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Zoom In
        {
            Camera.main.GetComponent<Camera>().orthographicSize -= 0.33f;
            Camera.main.GetComponent<Camera>().orthographicSize = CustomMath.ClampF(
                Camera.main.GetComponent<Camera>().orthographicSize,
                cam.camZoomLimits.x, cam.camZoomLimits.y);
        }
        if (Input.GetKeyDown(KeyCode.E)) // Zoom Out
        {
            Camera.main.GetComponent<Camera>().orthographicSize += 0.33f;
            Camera.main.GetComponent<Camera>().orthographicSize = CustomMath.ClampF(
               Camera.main.GetComponent<Camera>().orthographicSize,
               cam.camZoomLimits.x, cam.camZoomLimits.y);
        }
        if (canInput) // Can Input
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
            if (canScan) // Can click on a grid
            {
                if (Input.GetMouseButtonUp(0)) // Left Click
                {
                    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement")))
                    {
                        GridElement element = hit.collider.gameObject.GetComponent<GridElement>();
                        turnManager.ProcessGridClick(element);
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
