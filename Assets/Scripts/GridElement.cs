using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines the State of the Grid Element
public enum GridState
{
    EMPTY, // Free Grid 
    BLOCKED, // Blocked by Obstacle
    OCCUPIED, // Occupied by Player
};

public class GridElement : MonoBehaviour
{
    public int row; // Grid Element Row

    public int column; // Grid Element Column

    public Vector2 pos; // Grid Element Position (World Space)

    public GridState state; // State of Grid Element

    public List<GridElement> neighbours; // List of neightbour grids

    public GridElement parent = null; // Parent element in case of path

    public int gCost; // Distance from start grid 

    public int hCost; // Distance to target grid

    public int index = 0; // Index in the Heap

    // Sets the state of the Grid Element
    public void SetInitialState(int row_, int column_, Vector2 pos_)
    {
        row = row_;
        column = column_;
        pos = pos_;
        state = GridState.EMPTY;
    }

    // Sets the Grid state to some input state
    public void SetState(GridState state_)
    {
        state = state_;
    }

    public bool IsTraversable()
    {
        return (state != GridState.BLOCKED);
    }

    public int GetDistance(GridElement element)
    {
        int delX = (int)Mathf.Abs(pos.x - element.pos.x);
        int delZ = (int)Mathf.Abs(pos.y - element.pos.y);
        return ((14 * Mathf.Min(delX, delZ)) + (10 * (Mathf.Max(delX, delZ) - Mathf.Min(delX, delZ))));
    }

    public void SetCosts(GridElement start, GridElement target)
    {
        gCost = GetDistance(start);
        hCost = GetDistance(target);
    }

    public int GetFCost()
    {
        return gCost + hCost;
    }

    public int CompareGridElement(GridElement element)
    {
        int delF = element.GetFCost() - GetFCost();
        return (delF == 0) ? (element.hCost - hCost) : (delF);
    }

    public void PrintElement()
    {
        Debug.Log("Pos:- " + pos.x + ", " + pos.y);
        Debug.Log("Index is:- " + index);
        Debug.Log("Its costs are:- GCost: " + gCost + ", HCost: " + hCost + ", FCost: " + GetFCost());
    }
}

public class GridHeap
{
    public List<GridElement> list = new List<GridElement>();

    int maxCount = 0;

    public int count = 0;

    public void AddToHeap(GridElement element)
    {
        // Debug.Log("Adding Element:- " + element.pos);
        if (!HasElement(element))
        {
            // Debug.Log("Before Adding, the list was:-");
            // PrintHeap();
            element.index = count;
            list.Add(element);
            count++;
            // Debug.Log("Element Added");
            if (maxCount < count)
            {
                maxCount = count;
            }
            // Debug.Log("Before Sorting, the list was:-");
            // PrintHeap();
            SortUp(element);
            // Debug.Log("After Sorting, the list was:-");
            // PrintHeap();

        }
    }

    public void SortUp(GridElement element)
    {
        // Debug.Log("Sorting Up");
        // Debug.Log("Element is:- " + element.pos);
        // Debug.Log("Index:- " + element.index);
        int pIndex = (element.index - 1) / 2;
        // Debug.Log("PIndex:- " + pIndex);
        if (pIndex < 0)
        {
            // Debug.Log("Reached Top");
            return;
        }
        pIndex = CustomMath.Clamp(pIndex, 0, maxCount - 1);
        // Debug.Log("PIndex:- " + pIndex);
        int iterations = 0;
        while (true)
        {
            // Debug.Log("At Iteration:- " + iterations + ", PIndex:- " + pIndex);
            GridElement parentElement = list[pIndex];
            // Debug.Log("Parent Element is:- " + parentElement.pos + " and parent index is:- " + parentElement.index);
            if (element.CompareGridElement(parentElement) > 0)
            {
                // Debug.Log("Diff is:- " + element.CompareGridElement(parentElement));
                SwapElements(element, parentElement);
                // Debug.Log("Parent Element is:- " + parentElement.pos + " and parent index is:- " + parentElement.index);
            }
            else
            {
                // Debug.Log("Finish Sorting up");
                break;
            }
            pIndex = (element.index - 1) / 2;
            if (pIndex < 0)
            {
                // Debug.Log("Reached Top");
                return;
            }
            pIndex = CustomMath.Clamp(pIndex, 0, maxCount - 1);
            // pIndex = Mathf.Clamp((element.index - 1) / 2, 0, maxCount - 1);
            iterations++;
            if (iterations > 100)
            {
                Debug.LogError("CRASH UP");
                break;
            }
        }
    }

    public void SwapElements(GridElement a, GridElement b)
    {
        list[a.index] = b;
        list[b.index] = a;

        int index = a.index;
        a.index = b.index;
        b.index = index;
    }

    public GridElement PopElement()
    {
        // Debug.Log("Getting top");
        // Debug.Log("Before Popping List was:- ");
        // PrintHeap();
        GridElement element = list[0];
        // Debug.Log("Element is:- " + element.pos + " and index is:- " + element.index);
        // Debug.Log("Initially:- ");
        // Debug.Log("First Element is:- " + list[0].pos + " and index is:- " + list[0].index);
        // Debug.Log("Final Element is:- " + list[count - 1].pos + " and index is:- " + list[count - 1].index);
        count--;
        if (count > 0)
        {
            GridElement final = list[count];
            list.Remove(list[count]);
            // Debug.Log("Element Popped.");
            // Debug.Log("After popping:- ");
            // Debug.Log("First Element is:- " + list[0].pos + " and index is:- " + list[0].index);
            // Debug.Log("Final Element is:- " + list[count - 1].pos + " and index is:- " + list[count - 1].index);
            list[0] = final;
            list[0].index = 0;
            // list[0] = list[count];
            // list[0].index = 0;
            // Debug.Log("After setting data:- ");
            // Debug.Log("First Element is:- " + list[0].pos + " and index is:- " + list[0].index);
            // Debug.Log("Final Element is:- " + list[count - 1].pos + " and index is:- " + list[count - 1].index);
            // Debug.Log("New List before sorting is:- ");
            // PrintHeap();
            SortDown(list[0]);
            // Debug.Log("Sorted Down");
            // Debug.Log("After sorting:- ");
            // Debug.Log("First Element is:- " + list[0].pos + " and index is:- " + list[0].index);
            // Debug.Log("Final Element is:- " + list[count - 1].pos + " and index is:- " + list[count - 1].index);
        }
        else
        {
            list.Clear();
        }
        // Debug.Log("New List after sorting is:- ");
        // PrintHeap();
        return element;
    }

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
            if (iterations > 25)
            {
                Debug.LogError("CRASH DOWN");
                break;
            }
        }
    }

    public bool HasElement(GridElement element)
    {
        if (count > 0 && element.index < count)
        {
            return list[element.index] == element;
        }
        return false;
    }

    public void UpdateElement(GridElement element)
    {
        SortUp(element);
    }

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
        Debug.Log("Finished print.");
        // Debug.Log("Simple Print of list is:-");
        // int j = 0;
        // foreach (GridElement element in list)
        // {
        //     Debug.Log("Index is:- " + j);
        //     element.PrintElement();
        //     Debug.Log("Its index was:- " + element.index);
        //     Debug.Log("Its costs are:- GCost: " + element.gCost + ", HCost: " + element.hCost + ", FCost: " + element.GetFCost());
        //     j++;
        // }
        // Debug.Log("Finished simple print");
    }
}
