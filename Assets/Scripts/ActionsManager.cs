using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsManager : MonoBehaviour
{
    // Allows the player to move to any valid grid
    public void Move()
    {
        GetComponent<TurnManager>().ShowMoveGrids();
    }

    // Allows player to attack any valid enemy
    public void Attack()
    {

    }

    // Allows the player to skip the turn
    public void Skip()
    {
        GetComponent<TurnManager>().NextTurn();
    }
}
