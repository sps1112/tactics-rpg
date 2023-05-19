using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines the State of the Grid Element
public enum GridState
{
    EMPTY,
    BLOCKED,
    OCCUPIED,
};

public class GridElement : MonoBehaviour
{
    public int row; // Grid Element Row

    public int column; // Grid Element Column

    public Vector2 pos; // Grid Element Position (World Space)

    public GridState state; // State of Grid Element

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
}
