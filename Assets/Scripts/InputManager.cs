using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIManager ui; // UI Manager reference

    public GameObject player;

    private Pathfinding playerPath;

    void Start()
    {
        ui = GetComponent<UIManager>();
        playerPath = player.GetComponent<Pathfinding>();
    }

    void FixedUpdate()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement")))
        {
            ui.SetGridElementUI(hit.collider.gameObject.GetComponent<GridElement>());
        }
        else
        {
            ui.ResetGirdElementUI();
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GetComponent<ObstacleManager>().GenerateObstacles();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GetComponent<ObstacleManager>().DeleteObstacles();
        }
        if (!playerPath.moving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("GridElement")))
                {
                    Path path = playerPath.GetPath(hit.collider.gameObject.GetComponent<GridElement>());
                    playerPath.MoveViaPath(path);
                }
            }
        }
    }
}
