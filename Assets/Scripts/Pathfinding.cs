using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public List<GridElement> elements = new List<GridElement>(); // List of grid elements in the path

    public int length; // Number of elements in the path

    // Adds an element to the Path
    public void AddGrid(GridElement element)
    {
        elements.Insert(0, element);
        length++;
    }

    // Gets the distance which will need to be traversered to clear this path
    public int GetPathDistance()
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
        return (elements[length - 1] == target);
    }

    // Highlights this path
    public void HighlightPath(bool toHighlight)
    {
        for (int i = 0; i < length; i++)
        {
            if (toHighlight)
            {
                elements[i].PathHighlight((i) == (length - 1));
            }
            else
            {
                elements[i].HideHighlight();
            }
        }
    }

    // Prints the path
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
    public Character character; // Reference to the character data

    private GridSpawner spawner = null; // Reference to the Grid Spawner

    public bool isPlayer = true; // Whether agent is Player or Enemy

    public bool moving = false; // Whether the agent is moving

    public Path movePath = null; // Path being used for moving

    int pathIndex = 0; // Index of which element in the current path to move to

    public float minDistance = 0.05f; // Minimum Distance before stopping 

    public float jumpSpeed = 0.025f; // Speed used for jumping 

    public float maxYDiff = 0.65f; // Maximum difference to maintain with grid block while jumping

    public float minDistJump = 0.35f; // Minimum distance from a block before starting jumping

    public float moveSpeed = 7.5f; // Movement Speed while moving

    public float rotateSpeed = 0.1f; // Rotate speed factor while turning

    public GridElement currentGrid = null; // Grid before moving

    void Start()
    {
        spawner = GameObject.Find("GameManager").GetComponent<GridSpawner>();
    }

    // Returns the grid which the agent is currently upon
    public GridElement GetGrid()
    {
        if (spawner == null)
        {
            spawner = GameObject.Find("GameManager").GetComponent<GridSpawner>();
        }
        return spawner.GetElement(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }

    // Sets the grid states and refreshes current grid
    public void SetGrid()
    {
        if (currentGrid != null)
        {
            currentGrid.SetState(GridState.EMPTY);
        }
        currentGrid = GetGrid();
        currentGrid.SetState((isPlayer) ? (GridState.PLAYER) : (GridState.ENEMY));
    }

    // Gets the Path to the Target Grid
    public Path GetPath(GridElement target)
    {
        // Create lists to hold grid elements
        GridHeap openList = new GridHeap();
        GridHeap closedList = new GridHeap();
        List<GridElement> allGrids = new List<GridElement>();

        GridElement start = GetGrid();

        GridElement closestGrid = start;
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
                    if (Mathf.Abs(current.height - neighbour.height) <= character.jump)
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
                target = closestGrid;
                break;
            }

            iterations++;
            if (iterations > 250)
            {
                Debug.LogError("CRASH PATH");
                break;
            }
        }

        // Go over the path elements
        Path path = new Path();
        path.AddGrid(target);
        GridElement element = target.parent;
        while (element != start && element != null)
        {
            path.AddGrid(element);
            element = element.parent;
        }

        // Reset all the grids checked for Pathfinding
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
        moving = true;
        path.HighlightPath(true);
        pathIndex = 0;
        StartCoroutine("RotateToTarget", movePath.elements[pathIndex].transform.position);
    }

    // Coroutine to rotate to given target
    public IEnumerator RotateToTarget(Vector3 target)
    {
        Vector3 finalDir = CustomMath.GetDirectionTo(GetGrid().transform.position, target, true);
        while (Mathf.Abs(Vector3.Angle(transform.forward, finalDir)) >= 0.5f)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, finalDir, rotateSpeed * Time.deltaTime, 0.0f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    void Update()
    {
        if (moving)
        {
            Vector2 displacement = movePath.elements[pathIndex].pos - new Vector2(transform.position.x, transform.position.z);
            Vector2 direction = displacement.normalized;
            float distance = displacement.magnitude;
            if (distance < minDistance)
            {
                pathIndex++;
                StopCoroutine("RotateToTarget");
                if (pathIndex >= movePath.length)
                {
                    moving = false;
                    movePath.HighlightPath(false);
                    SetGrid();
                    GameObject.Find("GameManager").GetComponent<TurnManager>().NextTurn();
                    return;
                }
                else
                {
                    StartCoroutine("RotateToTarget", movePath.elements[pathIndex].transform.position);
                }
            }
            else
            {
                Vector3 pos = transform.position;
                float heightDiff = movePath.elements[pathIndex].height - GetGrid().height;
                if (heightDiff != 0 && distance < minDistJump)
                {
                    float currentGridY = GetGrid().transform.position.y;
                    float nextGridY = movePath.elements[pathIndex].transform.position.y;
                    float yDiff = nextGridY - currentGridY;
                    float jumpHeight = Mathf.Lerp(0.0f, yDiff, jumpSpeed);
                    float newY = pos.y + jumpHeight;
                    if (heightDiff > 0)
                    {
                        newY = Mathf.Clamp(newY, currentGridY + maxYDiff, nextGridY + maxYDiff);
                    }
                    else
                    {
                        newY = Mathf.Clamp(newY, nextGridY + maxYDiff, currentGridY + maxYDiff);
                    }
                    pos.y = newY;
                }
                pos += new Vector3(direction.x, 0.0f, direction.y) * moveSpeed * Time.deltaTime;
                transform.position = pos;
            }
        }
    }
}
