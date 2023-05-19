using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIManager ui; // UI Manager reference

    void Start()
    {
        ui = GetComponent<UIManager>();
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
    }
}
