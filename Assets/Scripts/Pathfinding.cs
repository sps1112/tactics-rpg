using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to store the path grids ans length
public class Path
{
    public List<GridElement> elements = new List<GridElement>(); // List of grid elements in the path

    public int length; // Number of elements in the path

    // Adds an element to the start of the Path
    public void AddGrid(GridElement element)
    {
        elements.Insert(0, element);
        length++;
    }

    // Gets the distance which will need to be traversered to clear this path
    public int GetPathDistance() // Relevant with diagonal motion enabled
    {
        int distance = 0;
        GridElement element1 = elements[0];
        for (int i = 1; i < length; i++)
        {
            GridElement element2 = elements[i];
            distance += element1.GetDistance(element2);
            element1 = element2;
        }
        return distance;
    }

    // Checks if the path is a complete path to the given target grid
    public bool IsCompletePath(GridElement target)
    {
        return elements.Contains(target) && elements[length - 1] == target;
    }

    // Shows and hides the highlight on this path
    public void HighlightPath(bool toHighlight)
    {
        for (int i = 0; i < length; i++)
        {
            if (toHighlight)
            {
                elements[i].PathHighlight(i == (length - 1));
            }
            else
            {
                elements[i].HideHighlight();
            }
        }
    }

    // Clamps the path to the grids available in given grid list
    public void FixForGrids(List<GridElement> grids)
    {
        List<GridElement> newElements = new List<GridElement>();
        // Go through each element in the path as long as it is present in the list of grids
        for (int i = 0; i < length; i++)
        {
            if (grids.Contains(elements[i]))
            {
                newElements.Add(elements[i]);
            }
            else
            {
                length = i;
                break;
            }
        }
        elements = newElements;
    }

    // Prints the path elements from start to finish
    public void PrintPath()
    {
        Debug.Log("Printing Path...");
        for (int i = 0; i < length; i++)
        {
            Debug.Log("At " + i + ":- (" + elements[i].pos.x + ", " + elements[i].pos.y + ")");
        }
        Debug.Log("Finished printing");
    }
}

public class Pathfinding : MonoBehaviour
{
    private Stats stats; // Reference to the character's stats

    private GridSpawner spawner = null; // Reference to the Grid Spawner

    private TurnManager turnManager = null; // Reference to the Turn Manager

    [SerializeField]
    private bool isPlayer = true; // Whether agent is Player or Enemy

    private bool toMove = false; // Whether the agent is moving

    private Path movePath = null; // Path being used for moving

    private int pathIndex = 0; // Index of which element in the current path to move to

    [SerializeField]
    private float moveSpeed = 7.5f; // Movement Speed while moving

    [SerializeField]
    private float minDistance = 0.05f; // Minimum Distance before stopping motion

    [SerializeField]
    private float jumpSpeed = 0.025f; // Speed used for jumping 

    public float maxYDiff = 0.65f; // Maximum difference to maintain with grid block while jumping

    [SerializeField]
    private float minDistJump = 0.35f; // Minimum distance from a block before starting jumping

    [SerializeField]
    private float rotateSpeed = 0.1f; // Rotate speed factor while rotating

    [SerializeField]
    private float minAngle = 0.5f; // Minimum angle difference before stopping rotation

    private GridElement currentGrid = null; // Grid before moving

    void Start()
    {
        GetData();
    }

    // Gets the reference data
    private void GetData()
    {
        stats = GetComponent<Stats>();
        spawner = GameObject.Find("GameManager").GetComponent<GridSpawner>();
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
    }

    // Returns the grid which the agent is currently upon
    public GridElement GetGrid()
    {
        if (spawner == null)
        {
            GetData();
        }
        // Return the grid at the current position
        return spawner.GetElement(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }

    // Sets the grid states and refreshes current grid
    public void SetGrid()
    {
        if (currentGrid != null) // If already a grid, reset it to empty
        {
            currentGrid.SetState(GridState.EMPTY);
        }
        currentGrid = GetGrid();
        currentGrid.SetState(isPlayer ? GridState.PLAYER : GridState.ENEMY);
    }

    // Gets all the connected grids for this character to the given steps
    public List<GridElement> GetConnectedGrids()
    {
        GridElement start = GetGrid();

        List<GridElement> grids = new List<GridElement>(); // List of connected grids
        grids.Add(start);

        List<GridElement> prevGrids = new List<GridElement>(); // List of grids added in previous step
        prevGrids.Add(start);

        int steps = stats.actions; // Number of steps to check for a grid
        for (int i = 0; i < steps; i++)
        {
            List<GridElement> newGrids = new List<GridElement>();
            foreach (GridElement grid in prevGrids) // Check for each grid we added last time
            {
                foreach (GridElement neighbour in grid.neighbours) // Check for each of its neighbour
                {
                    if (neighbour.IsTraversable(isPlayer) && !grids.Contains(neighbour)) // Check for any neighbour we already haven't added
                    {
                        if (Mathf.Abs(grid.height - neighbour.height) <= stats.character.jump) // If we can reach there, add it
                        {
                            newGrids.Add(neighbour); // List all the grids we are adding this step
                            grids.Add(neighbour);
                        }
                    }
                }
            }
            prevGrids = newGrids; // These will be checked in the next step
        }
        return grids;
    }

    // Gets the Path to the Target Grid
    public Path GetPath(GridElement target)
    {
        // Create lists to hold grid elements
        GridHeap openList = new GridHeap();
        GridHeap closedList = new GridHeap();

        // Store a list all the grids we have checked for as their gCost and hCost have to be reset
        List<GridElement> allGrids = new List<GridElement>();

        GridElement start = GetGrid(); // Current grid

        // Stores the closest grid we have gotten to the target in Pathfinding
        GridElement closestGrid = start;  // This will be used as the new target, if no path is available to the target
        int closestDist = int.MaxValue;

        openList.AddToHeap(start);
        allGrids.Add(start);

        int iterations = 0;
        while (openList.count > 0)
        {
            // Get Grid Element with the lowest FCost
            GridElement current = openList.PopElement();
            closedList.AddToHeap(current);
            if (!allGrids.Contains(current))
            {
                allGrids.Add(current);
            }

            // Keep Refreshing the Closest Grid to the Target
            if (target != start)
            {
                if (current == start)
                {
                    closestDist = current.GetDistance(target);
                    closestGrid = current;
                }
                else
                {
                    if (current.hCost < closestDist)
                    {
                        closestDist = current.hCost;
                        closestGrid = current;
                    }
                }
            }

            // Check if reached Target
            if (current == target)
            {
                break;
            }

            // Check for all neighbours
            for (int i = 0; i < current.neighbours.Count; i++)
            {
                GridElement neighbour = current.neighbours[i];
                if (neighbour.IsTraversable(isPlayer) && !closedList.HasElement(neighbour))
                {
                    if (Mathf.Abs(current.height - neighbour.height) <= stats.character.jump)
                    {
                        int moveCost = current.gCost + current.GetDistance(neighbour);
                        if (!openList.HasElement(neighbour) || moveCost < neighbour.gCost)
                        {
                            neighbour.gCost = moveCost;
                            neighbour.hCost = neighbour.GetDistance(target);
                            neighbour.parent = current;
                            if (!openList.HasElement(neighbour))
                            {
                                openList.AddToHeap(neighbour);
                                if (!allGrids.Contains(neighbour))
                                {
                                    allGrids.Add(neighbour);
                                }
                                // Refresh Closest Grid
                                if (neighbour.hCost < closestDist)
                                {
                                    closestDist = neighbour.hCost;
                                    closestGrid = neighbour;
                                }
                            }
                            else
                            {
                                openList.UpdateElement(neighbour);
                            }
                        }
                    }
                }
            }

            // Check if no path exists
            if (openList.count == 0)
            {
                target = closestGrid; // We use the closest grid as the new target
                break;
            }

            // Check for infinite loop
            iterations++;
            if (iterations > 250)
            {
                Debug.LogError("CRASH PATH");
                break;
            }
        }

        // Now backtrack from the target to reach the start grid to get the path
        Path path = new Path();
        path.AddGrid(target);
        GridElement element = target.parent;
        while (element != start && element != null)
        {
            path.AddGrid(element);
            element = element.parent;
        }

        // Reset all these grids so that they can be used fresh for the next check
        for (int i = 0; i < allGrids.Count; i++)
        {
            allGrids[i].gCost = 0;
            allGrids[i].hCost = 0;
            allGrids[i].index = 0;
            allGrids[i].parent = null;
        }

        return path;
    }

    // Starts motion along the given path
    public void MoveViaPath(Path path)
    {
        movePath = path;
        toMove = true;
        GetGrid().PathHighlight(false); // Highlight start grid
        path.HighlightPath(true); // Highlight path
        pathIndex = 0;
        StartCoroutine("RotateToTarget", movePath.elements[pathIndex].transform.position);
    }

    // Coroutine to rotate to given target
    public IEnumerator RotateToTarget(Vector3 target)
    {
        Vector3 finalDir = CustomMath.GetDirectionTo(GetGrid().transform.position, target, true);
        while (Mathf.Abs(Vector3.Angle(transform.forward, finalDir)) >= minAngle) //
        {
            transform.forward = Vector3.RotateTowards(transform.forward, finalDir, rotateSpeed * Time.deltaTime, 0.0f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.forward = CustomMath.ClampAlongCardinals(transform.forward);
    }

    void Update()
    {
        if (toMove)
        {
            Vector2 dis = movePath.elements[pathIndex].pos - new Vector2(transform.position.x, transform.position.z);
            Vector2 dir = dis.normalized;
            float dist = dis.magnitude;
            if (dist < minDistance) // Reached next grid
            {
                pathIndex++; // Move on to next one
                StopCoroutine("RotateToTarget");
                if (pathIndex >= movePath.length) // Path finished as reached target
                {
                    toMove = false;
                    SetGrid();
                    movePath.HighlightPath(false);
                    stats.UseActions(movePath.length); // Use actions for the number of steps moved
                    turnManager.StartCheckPhase();
                    return;
                }
                else // Else, we now have to move to next grid in path, thus start rotating to face it
                {
                    StartCoroutine("RotateToTarget", movePath.elements[pathIndex].transform.position);
                }
            }
            else // Still moving to next grid, so we move up or down to reach grid's height level
            {
                Vector3 pos = transform.position;
                // Move pos along Y axis
                float heightDiff = movePath.elements[pathIndex].height - GetGrid().height;
                // Only start moving up or down if close enough to the next grid
                if (heightDiff != 0 && dist < minDistJump)
                {
                    float currentY = GetGrid().transform.position.y;
                    float nextY = movePath.elements[pathIndex].transform.position.y;
                    float yDiff = nextY - currentY;
                    float jumpHeight = Mathf.Lerp(0.0f, yDiff, jumpSpeed * Time.deltaTime);
                    float newY = pos.y + jumpHeight;
                    // Clamp the final height between the limits based on the current and next grid's height
                    if (heightDiff > 0)
                    {
                        newY = CustomMath.ClampF(newY, currentY + maxYDiff, nextY + maxYDiff);
                    }
                    else
                    {
                        newY = CustomMath.ClampF(newY, nextY + maxYDiff, currentY + maxYDiff);
                    }
                    pos.y = newY;
                }
                // Move pos along X and Z axes
                pos += new Vector3(dir.x, 0.0f, dir.y) * moveSpeed * Time.deltaTime;
                transform.position = pos;
            }
        }
    }
}
