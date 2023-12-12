using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsManager : MonoBehaviour
{
    private TurnManager turn; // Turn Manager Reference

    private UIManager ui; // UI Manager Reference

    private CameraFollow cam; // Reference to the camera

    void Start()
    {
        turn = GetComponent<TurnManager>();
        ui = GetComponent<UIManager>();
        cam = Camera.main.GetComponent<CameraFollow>();
    }

    // Performs a given action based on its ID
    public void PerformAction(int actionID)
    {
        if (canPerformAction(actionID))
        {
            StartCoroutine("StartAction", actionID);
        }
    }

    // Checks if the action can be performed
    public bool canPerformAction(int actionID)
    {
        bool status = true;
        switch (actionID)
        {
            case 2:
                status = turn.CheckAttackAction();
                break;
            default:
                break;
        }
        return status;
    }

    // Makes the player move to any valid grid
    public void Move()
    {
        turn.ShowMoveGrids();
    }

    // Makes player attack a character on any valid grid
    public void Attack()
    {
        turn.ShowAttackGrids();
    }

    // Makes the player wait for this turn
    public void Wait()
    {
        turn.StartCoroutine("EndTurn");
    }

    // Snaps the camera back to the player and performs the action
    IEnumerator StartAction(int actionID)
    {
        ui.SetActionsUI(false, false);
        yield return cam.StartCoroutine("SnapToTarget", turn.current);
        switch (actionID)
        {
            case 1:
                Move();
                break;
            case 2:
                Attack();
                break;
            case 3:
                Wait();
                break;
            default:
                break;
        }
    }
}
