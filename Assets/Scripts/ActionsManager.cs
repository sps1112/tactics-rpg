using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsManager : MonoBehaviour
{
    private CameraFollow cam; // Reference to the camera

    void Start()
    {
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
                status = GetComponent<TurnManager>().CheckAttackAction();
                break;
            default:
                break;
        }
        return status;
    }

    // Makes the player move to any valid grid
    public void Move()
    {
        GetComponent<TurnManager>().ShowMoveGrids();
    }

    // Makes player attack a character on any valid grid
    public void Attack()
    {
        GetComponent<TurnManager>().ShowAttackGrids();
    }

    // Makes the player wait for this turn
    public void Wait()
    {
        GetComponent<TurnManager>().StartCoroutine("EndTurn");
    }

    // Snaps the camera back to the player and performs the action
    IEnumerator StartAction(int actionID)
    {
        GetComponent<UIManager>().SetActionsUI(false, false);
        yield return cam.StartCoroutine("SnapToTarget", GetComponent<TurnManager>().current);
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
