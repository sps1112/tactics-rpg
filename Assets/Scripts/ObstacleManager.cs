using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public ObstacleLayout layout; // Layout Object Reference

    public List<GameObject> obstacles; // List of obstacles spawned

    private bool obstacleActive = false; // Have the obstacles been generated

    [SerializeField]
    private GridSpawner grid = null; // Reference to the grid

    [SerializeField]
    private GameObject obstacleParent = null; // Obstacles Parent reference

    void Start()
    {
        GenerateObstacles();
    }

    // Generates the obstacles
    public void GenerateObstacles()
    {
        if (!obstacleActive)
        {
            if (grid == null)
            {
                grid = GetComponent<GridSpawner>();
            }
            if (obstacleParent == null)
            {
                GameObject empty = new GameObject("Empty");
                obstacleParent = Instantiate(empty, Vector3.zero, Quaternion.identity, grid.gridOrigin.transform);
                obstacleParent.name = "Obstacles";
                Destroy(empty);
            }
            for (int i = 0; i < layout.rows; i++)
            {
                for (int j = 0; j < layout.columns; j++)
                {
                    if (layout.layout[i * layout.columns + j])
                    {
                        float xPos = grid.gridOrigin.position.x + j * 1.0f;
                        float zPos = grid.gridOrigin.position.z + i * 1.0f;
                        float yPos = grid.gridOrigin.position.y + 1.0f;
                        GameObject sphere = Instantiate(layout.obstacle, new Vector3(xPos, yPos, zPos), Quaternion.identity, obstacleParent.transform);
                        // Set Grid Element as blocked
                        obstacles.Add(sphere);
                    }
                }
            }
            obstacleActive = true;
        }
    }

    // Deletes the obstacles
    public void DeleteObstacles()
    {
        if (obstacleActive)
        {
            foreach (GameObject obstacle in obstacles)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(obstacle);
                }
                else
                {
                    Destroy(obstacle);
                }
            }
            obstacles.Clear();
            obstacleActive = false;
        }
    }
}
