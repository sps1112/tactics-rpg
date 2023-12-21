using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines the State of a Grid Element
public enum GridState
{
    EMPTY, // Free Grid 
    BLOCKED, // Blocked by Obstacle
    PLAYER, // Blocked by Player
    ENEMY, // Blocked by Enemy
};

public class GridElement : MonoBehaviour
{
    public int row; // Grid Element Row

    public int column; // Grid Element Column

    public Vector2 pos; // Absolute Position of Grid Element

    public int height; // Height of the Grid Element

    [SerializeField]
    private GridState state; // State of Grid Element

    public List<GridElement> neighbours; // List of neightbour grid elements

    public GridElement parent = null; // Parent element in case of path traversal

    public int gCost; // Distance from start grid in path calculations

    public int hCost; // Distance to target grid in path calculations

    public int index = 0; // Index in the Heap in path calculations

    private Material highlight = null; // Reference to the grid highlight material

    [SerializeField]
    private Color normalHColor; // Highlight color for normal grids

    [SerializeField]
    private Color blockedHColor; // Highlight color for blocked grids

    [SerializeField]
    private Color hiddenHColor; // Highlight color when not hovering

    [SerializeField]
    private Color pathHColor; // Highlight color for grids on a path

    [SerializeField]
    private Color targetHColor; // Highlight color for target grid in a path

    [SerializeField]
    private Color actionHColor; // Highlight color for grids on which actions can be performed

    public bool isActionGrid = false; // Whether the grid is an action grid

    public bool canActOnGrid = true; // Whether actions can be performed on this grid

    void Start()
    {
        highlight = transform.GetChild(0).gameObject.GetComponent<Renderer>().material;
    }

    // Sets the initial state of the Grid Element
    public void SetInitialState(int row_, int column_, Vector2 pos_, int height_)
    {
        row = row_;
        column = column_;
        pos = pos_;
        state = GridState.EMPTY;
        height = height_;
    }

    // Changes the grid's state to a given state
    public void SetState(GridState state_)
    {
        state = state_;
    }

    // Checks if the grid can be traversed in respect to the checking agent
    public bool IsTraversable(bool isPlayer)
    {
        if (isPlayer)
        {
            return (state != GridState.BLOCKED) && (state != GridState.ENEMY);
        }
        return (state != GridState.BLOCKED) && (state != GridState.PLAYER);
    }

    // Gets the distance to the target grid from this grid
    public int GetDistance(GridElement target)
    {
        int delX = Mathf.RoundToInt(Mathf.Abs(pos.x - target.pos.x));
        int delZ = Mathf.RoundToInt(Mathf.Abs(pos.y - target.pos.y));
        return (14 * Mathf.Min(delX, delZ)) + (10 * CustomMath.Difference(delX, delZ)); // Manhattan Distancing
    }

    // Sets the GCost and HCost for this grid in the current path calculations
    public void SetCosts(GridElement start, GridElement target)
    {
        gCost = GetDistance(start);
        hCost = GetDistance(target);
    }

    // Returns the FCost which is the sum of GCost and HCost
    public int GetFCost()
    {
        return gCost + hCost;
    }

    // Compares two grid elements to give more preferable grid for path traversal
    public int CompareGridElement(GridElement element)
    {
        int delF = element.GetFCost() - GetFCost();
        return (delF == 0) ? (element.hCost - hCost) : (delF);
    }

    // Highlights the current grid
    public void ShowHighlight()
    {
        highlight.color = IsTraversable(true) ? normalHColor : blockedHColor;
    }

    // Removes highlight from the grid
    public void HideHighlight()
    {
        highlight.color = hiddenHColor;
        isActionGrid = false;
    }

    // Highlights grid along a path
    public void PathHighlight(bool isTarget)
    {
        highlight.color = isTarget ? targetHColor : pathHColor;
    }

    // Highlights the grid to show it can be used for actions
    public void ActionHighlight()
    {
        if (highlight == null)
        {
            highlight = transform.GetChild(0).gameObject.GetComponent<Renderer>().material;
        }
        highlight.color = actionHColor;
        isActionGrid = true;
    }

    // Prints the details of the current grid 
    public void PrintElement()
    {
        Debug.Log("Pos:- (" + pos.x + ", " + pos.y + ")");
        Debug.Log("Index is:- " + index);
        Debug.Log("Its costs are:- GCost:- " + gCost + ", HCost:- " + hCost + "and FCost:- " + GetFCost());
    }
}

// Heap Data structure to store grid elements in sorted form for pathfinding
public class GridHeap
{
    public List<GridElement> list = new List<GridElement>(); // List of all the grid elements in the heap

    private int maxCount = 0; // The Maximum count of elements which were part of this heap

    public int count = 0; // Current count of elements in the heap

    // Adds a new element to the heap
    public void AddToHeap(GridElement element)
    {
        if (!HasElement(element))
        {
            element.index = count;
            list.Add(element);
            count++;
            if (maxCount < count)
            {
                maxCount = count;
            }
            SortUp(element);
        }
    }

    // Sorts the Heap upwards from the given element
    private void SortUp(GridElement element)
    {
        int pIndex = (element.index - 1) / 2;
        if (pIndex < 0)
        {
            return;
        }
        pIndex = CustomMath.Clamp(pIndex, 0, maxCount - 1);
        int iterations = 0;
        while (true)
        {
            GridElement parentElement = list[pIndex];
            if (element.CompareGridElement(parentElement) > 0)
            {
                SwapElements(element, parentElement);
            }
            else
            {
                break;
            }
            pIndex = (element.index - 1) / 2;
            if (pIndex < 0)
            {
                return;
            }
            pIndex = CustomMath.Clamp(pIndex, 0, maxCount - 1);

            // Check for infinite loop
            iterations++;
            if (iterations > 250)
            {
                Debug.LogError("CRASH UP");
                break;
            }
        }
    }

    // Swaps two elements in the heap
    private void SwapElements(GridElement a, GridElement b)
    {
        list[a.index] = b;
        list[b.index] = a;

        int index = a.index;
        a.index = b.index;
        b.index = index;
    }

    // Pops the top element from the heap removing it
    public GridElement PopElement()
    {
        GridElement element = list[0];
        count--;
        if (count > 0)
        {
            GridElement final = list[count];
            list.Remove(list[count]);
            list[0] = final;
            list[0].index = 0;
            SortDown(list[0]);
        }
        else
        {
            list.Clear();
        }
        return element;
    }

    // Sorts the Heap downwards from the given element
    private void SortDown(GridElement element)
    {
        int iterations = 0;
        while (true)
        {
            int cIndex1 = element.index * 2 + 1;
            int cIndex2 = element.index * 2 + 2;
            int index = 0;
            if (cIndex1 < count)
            {
                index = cIndex1;
                if (cIndex2 < count)
                {
                    if (list[cIndex1].CompareGridElement(list[cIndex2]) < 0)
                    {
                        index = cIndex2;
                    }
                }
                if (element.CompareGridElement(list[index]) < 0)
                {
                    SwapElements(element, list[index]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }

            // Check for infinite loop
            iterations++;
            if (iterations > 250)
            {
                Debug.LogError("CRASH DOWN");
                break;
            }
        }
    }

    // Checks if the heap has the given element
    public bool HasElement(GridElement element)
    {
        if (count > 0 && element.index < count)
        {
            return list[element.index] == element;
        }
        return false;
    }

    // Sorts the Heap upwards from the given element
    public void UpdateElement(GridElement element)
    {
        SortUp(element);
    }

    // Prints the Heap
    public void PrintHeap()
    {
        Debug.Log("Printing Heap...");
        Debug.Log("Count is:- " + count + " and the max count is :- " + maxCount);
        if (count <= 0)
        {
            Debug.Log("Empty Heap");
            return;
        }
        for (int i = 0; i < count; i++)
        {
            Debug.Log("At:- " + i);
            list[i].PrintElement();
        }
        Debug.Log("Finished printing");
    }
}
