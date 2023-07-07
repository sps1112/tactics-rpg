using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines the types of turn availables
public enum TurnType
{
    PLAYER, // Player's Turn
    ENEMY, // Enemy's Turn
}

public class TurnManager : MonoBehaviour
{
    private UIManager ui; // UI Manager reference

    public TurnType turn; // Current Turn Type

    void Start()
    {
        ui = GetComponent<UIManager>();
        turn = TurnType.PLAYER;
    }

    // Starts the next turn
    public void NextTurn()
    {
        turn = (turn == TurnType.PLAYER) ? TurnType.ENEMY : TurnType.PLAYER;
        ui.SetTurnUI(turn);
    }
}
