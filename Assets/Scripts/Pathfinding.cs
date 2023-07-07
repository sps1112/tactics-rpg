using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public List<GridElement> elements = new List<GridElement>();

    public int length;

    public void AddGrid(GridElement element)
    {
        elements.Insert(0, element);
        length++;
    }

    public void PrintPath()
    {
        Debug.Log("Path is:-");
        for (int i = 0; i < length; i++)
        {
            Debug.Log(i + ":- (" + elements[i].pos.x + ", " + elements[i].pos.y + ")");
        }
    }

    public int GetPathDistance()
    {
        int distance = 0;
        GridElement element1 = elements[0];
        for (int i = 1; i < length; i++)
        {
            GridElement element2 = elements[i];
            distance += element2.GetDistance(element1);
            element1 = element2;
        }
        return distance;
    }
}

public class Pathfinding : MonoBehaviour
{
    private GridSpawner spawner = null;

    public bool isPlayer = true;

    public bool moving = false;

    public Path movePath = null;

    public GridElement nextElement = null;

    int pathIndex = 0;

    public float minDistance = 0.05f;

    public float moveSpeed = 10.0f;

    public GridElement currentGrid = null;

    public GridElement GetGrid()
    {
        if (spawner == null)
        {
            spawner = GameObject.Find("GameManager").GetComponent<GridSpawner>();
        }
        return spawner.GetElement(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }

    public void SetGrid()
    {
        if (currentGrid != null)
        {
            currentGrid.SetState(GridState.EMPTY);
        }
        currentGrid = GetGrid();
        currentGrid.SetState((isPlayer) ? (GridState.PLAYER) : (GridState.ENEMY));
    }

    public Path GetPath(GridElement target)
    {
        if (spawner == null)
        {
            spawner = GameObject.Find("GameManager").GetComponent<GridSpawner>();
        }
        // Debug.Log("0: Current pos is:- " + transform.position);
        // int xStart = (int)transform.position.x;
        // int zStart = (int)transform.position.z;
        // Debug.Log("1: Current pos is:- " + xStart + ", " + zStart);
        GridElement start = GetGrid();
        // Debug.Log("Start:- " + start.pos.x + " " + start.pos.y);
        // Debug.Log("Target:- " + target.pos.x + " " + target.pos.y);

        GridHeap openList = new GridHeap();
        GridHeap closedList = new GridHeap();
        List<GridElement> allGrids = new List<GridElement>();
        GridElement closestGrid = start;
        int closestDist = int.MaxValue;
        openList.AddToHeap(start);
        allGrids.Add(start);
        int iterations = 0;
        while (openList.count > 0)
        {
            // Debug.Log("Run:- " + iterations);
            // Debug.Log("OPEN HEAP");
            // openList.PrintHeap();
            // Debug.Log("CLOSED HEAP");
            // closedList.PrintHeap();
            GridElement current = openList.PopElement();
            // Debug.Log("Current is:-" + current.pos.x + " " + current.pos.y);
            closedList.AddToHeap(current);
            if (!allGrids.Contains(current))
            {
                allGrids.Add(current);
            }
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
            if (current == target)
            {
                // Debug.Log("Reached target");
                break;
            }
            for (int i = 0; i < current.neighbours.Count; i++)
            {
                // Debug.Log("Searching neighbour: " + i);
                GridElement neighbour = current.neighbours[i];
                // Debug.Log("Neighbour is:- " + neighbour.pos.x + " " + neighbour.pos.y);
                if (neighbour.IsTraversable(isPlayer) && !closedList.HasElement(neighbour))
                {
                    // Debug.Log("Checking Neighbour");
                    int moveCost = current.gCost + current.GetDistance(neighbour);
                    if (!openList.HasElement(neighbour) || moveCost < neighbour.gCost)
                    {
                        neighbour.gCost = moveCost;
                        neighbour.hCost = neighbour.GetDistance(target);
                        neighbour.parent = current;
                        if (!openList.HasElement(neighbour))
                        {
                            // Debug.Log("Adding this Neighbour");
                            openList.AddToHeap(neighbour);
                            if (neighbour.hCost < closestDist)
                            {
                                closestDist = neighbour.hCost;
                                closestGrid = neighbour;
                            }
                            if (!allGrids.Contains(neighbour))
                            {
                                allGrids.Add(neighbour);
                            }
                        }
                        else
                        {
                            openList.UpdateElement(neighbour);
                        }
                    }
                }
            }
            if (openList.count == 0)
            {
                Debug.Log("No Path");
                target = closestGrid;
                break;
            }
            iterations++;
            if (iterations > 100)
            {
                Debug.LogError("CRASH PATH");
                break;
            }
        }
        // Debug.Log("Found Path");
        Path path = new Path();
        path.AddGrid(target);
        // path.PrintPath();
        GridElement element = target.parent;
        // Debug.Log(element.pos.x + " " + element.pos.y);
        // Debug.Log("Before Loop");
        while (element != start && element != null)
        {
            // Debug.Log("Element is:- " + element.pos.x + " " + element.pos.y);
            path.AddGrid(element);
            element = element.parent;
        }
        // path.PrintPath();
        for (int i = 0; i < allGrids.Count; i++)
        {
            allGrids[i].gCost = 0;
            allGrids[i].hCost = 0;
            allGrids[i].index = 0;
            allGrids[i].parent = null;
        }
        return path;
    }

    public void MoveViaPath(Path path)
    {
        movePath = path;
        moving = true;
        pathIndex = 0;
        nextElement = path.elements[0];
    }

    void Update()
    {
        if (moving)
        {
            Vector2 displacement = nextElement.pos - new Vector2(transform.position.x, transform.position.z);
            Vector2 direction = displacement.normalized;
            float distance = displacement.magnitude;
            if (distance < minDistance)
            {
                // Debug.Log("Move 1 block");
                pathIndex++;
                if (pathIndex >= movePath.length)
                {
                    moving = false;
                    SetGrid();
                    GameObject.Find("GameManager").GetComponent<TurnManager>().NextTurn();
                    return;
                }
                nextElement = movePath.elements[pathIndex];
            }
            else
            {
                Vector3 pos = transform.position;
                pos += new Vector3(direction.x, 0.0f, direction.y) * moveSpeed * Time.deltaTime;
                transform.position = pos;
            }
        }
    }
}
