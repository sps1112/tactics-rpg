using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsManager : MonoBehaviour
{
    private TurnManager turnManager; // Turn Manager Reference

    private CameraFollow cam; // Reference to the camera

    void Start()
    {
        turnManager = GetComponent<TurnManager>();
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
                status = turnManager.CheckAttackAction();
                break;
            default:
                break;
        }
        return status;
    }

    // Makes the player move to any valid grid
    public void Move()
    {
        turnManager.StartMovePhase();
    }

    // Makes player attack a character on any valid grid
    public void Attack()
    {
        turnManager.StartAttackPhase();
    }

    // Makes the player wait for this turn
    public void Wait()
    {
        turnManager.StartCoroutine("EndTurn");
    }

    // Snaps the camera back to the player and performs the action
    private IEnumerator StartAction(int actionID)
    {
        // Wait till the camera snaps to current character
        yield return cam.StartCoroutine("SnapToTarget", turnManager.current);
        // Then, perform the respective action
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
