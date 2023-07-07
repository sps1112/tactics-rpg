using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnType
{
    PLAYER,
    ENEMY,
}

public class TurnManager : MonoBehaviour
{
    private UIManager ui; // UI Manager reference

    public TurnType turn = TurnType.PLAYER;

    void Start()
    {
        ui = GetComponent<UIManager>();
    }

    public void NextTurn()
    {
        turn = (turn == TurnType.PLAYER) ? TurnType.ENEMY : TurnType.PLAYER;
        ui.SetTurnUI(turn);
    }
}
