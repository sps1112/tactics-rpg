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

    public GridState state; // State of Grid Element

    public List<GridElement> neighbours; // List of neightbour grids

    public GridElement parent = null; // Parent element in case of path

    public int gCost; // Distance from start grid 

    public int hCost; // Distance to target grid

    public int index = 0; // Index in the Heap

    private Material highlight = null; // Reference to the grid highlight material

    public Color normalHColor; // Highlight color for normal grids

    public Color blockedHColor; // Highlight color for blocked grids

    public Color hiddenHColor; // Highlight color when not hovering

    public Color pathHColor; // Highlight color for grids on a path

    public Color targetHColor; // Highlight color for target grid in a path

    public Color spawnColor; // Highlight color for grids to spawn player

    void Start()
    {
        highlight = transform.GetChild(0).gameObject.GetComponent<Renderer>().material;
    }

    // Sets the state of the Grid Element
    public void SetInitialState(int row_, int column_, Vector2 pos_, int height_)
    {
        row = row_;
        column = column_;
        pos = pos_;
        state = GridState.EMPTY;
        height = height_;
    }

    // Sets the Grid state to some input state
    public void SetState(GridState state_)
    {
        state = state_;
    }

    // Checks if the Grid can be traversed in respect to the checking agent
    public bool IsTraversable(bool isPlayer)
    {
        if (isPlayer)
        {
            return (state != GridState.BLOCKED && state != GridState.ENEMY);
        }
        return (state != GridState.BLOCKED && state != GridState.PLAYER);
    }

    // Gets the distance to the target grid from this grid
    public int GetDistance(GridElement target)
    {
        int delX = Mathf.RoundToInt(Mathf.Abs(pos.x - target.pos.x));
        int delZ = Mathf.RoundToInt(Mathf.Abs(pos.y - target.pos.y));
        return ((14 * Mathf.Min(delX, delZ)) + (10 * (Mathf.Max(delX, delZ) - Mathf.Min(delX, delZ))));
    }

    // Sets the GCost and HCost for this grid
    public void SetCosts(GridElement start, GridElement target)
    {
        gCost = GetDistance(start);
        hCost = GetDistance(target);
    }

    // Returns the FCost as the sum of GCost and HCost
    public int GetFCost()
    {
        return gCost + hCost;
    }

    // Compares two grid elements to give more preferable grid
    public int CompareGridElement(GridElement element)
    {
        int delF = element.GetFCost() - GetFCost();
        return (delF == 0) ? (element.hCost - hCost) : (delF);
    }

    // Highlights the current grid
    public void ShowHighlight()
    {
        highlight.color = (IsTraversable(true)) ? (normalHColor) : (blockedHColor);
    }

    // Removes highlight from the grid
    public void HideHighlight()
    {
        highlight.color = hiddenHColor;
    }

    // Highlights grid along a path
    public void PathHighlight(bool isTarget)
    {
        highlight.color = (isTarget) ? (targetHColor) : (pathHColor);
    }

    // Highlights the grid to show it can spawn player
    public void SpawnHighlight()
    {
        if (highlight == null)
        {
            highlight = transform.GetChild(0).gameObject.GetComponent<Renderer>().material;
        }
        highlight.color = spawnColor;
    }

    // Prints the details of the current grid 
    public void PrintElement()
    {
        Debug.Log("Pos:- (" + pos.x + ", " + pos.y + ")");
        Debug.Log("Index is:- " + index);
        Debug.Log("Its costs are:- GCost:- " + gCost + ", HCost:- " + hCost + "and FCost:- " + GetFCost());
    }
}

public class GridHeap
{
    public List<GridElement> list = new List<GridElement>(); // List of all the grid elements in the heap

    int maxCount = 0; // The Maximum count of elements which were part of this heap

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
    public void SortUp(GridElement element)
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
            iterations++;
            if (iterations > 250)
            {
                Debug.LogError("CRASH UP");
                break;
            }
        }
    }

    // Swaps two elements in the heap
    public void SwapElements(GridElement a, GridElement b)
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
    void SortDown(GridElement element)
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
