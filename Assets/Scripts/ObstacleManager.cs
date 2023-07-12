using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public ObstacleLayout layout; // Layout Object Reference

    public float obstacleHeightOffset = 0.475f; // Offset for spawning the obstacles

    public List<GameObject> obstacles; // List of obstacles spawned

    private bool obstaclesActive = false; // Have the obstacles been generated

    [SerializeField]
    private GridSpawner grid = null; // Reference to the grid

    [SerializeField]
    private GameObject obstacleParent = null; // Obstacles Parent reference

    private TurnManager turn = null; // Reference to the Turn Manager

    void Start()
    {
        grid = GetComponent<GridSpawner>();
        turn = GetComponent<TurnManager>();
        GenerateObstacles();
    }

    // Generates the obstacles
    public void GenerateObstacles()
    {
        if (!obstaclesActive)
        {
            if (obstacles.Count <= 0)
            {
                // For Obstacle generation via Editor
                if (grid == null || turn == null)
                {
                    grid = GetComponent<GridSpawner>();
                    turn = GetComponent<TurnManager>();
                }
                // Create Parent object if already not created
                if (obstacleParent == null)
                {
                    GameObject empty = new GameObject("Empty");
                    obstacleParent = Instantiate(empty, Vector3.zero, Quaternion.identity, grid.gridOrigin.transform);
                    obstacleParent.name = "Obstacles";
                    if (Application.isEditor)
                    {
                        DestroyImmediate(empty);
                    }
                    else
                    {
                        Destroy(empty);
                    }
                }
                for (int i = 0; i < layout.rows; i++)
                {
                    for (int j = 0; j < layout.columns; j++)
                    {
                        float xPos = grid.gridOrigin.position.x + j * 1.0f;
                        float zPos = grid.gridOrigin.position.z + i * 1.0f;
                        if (layout.layout[i * layout.columns + j] == 1) // Obstacle
                        {
                            if (grid.GetElement((int)xPos, (int)zPos) != null)
                            {
                                float yPos = grid.GetElement((int)xPos, (int)zPos).transform.position.y + obstacleHeightOffset;
                                GameObject obs = Instantiate(layout.obstacle, new Vector3(xPos, yPos, zPos), Quaternion.identity, obstacleParent.transform);
                                grid.GetElement((int)xPos, (int)zPos).SetState(GridState.BLOCKED);
                                obstacles.Add(obs);
                            }
                        }
                        else if (layout.layout[i * layout.columns + j] == 3) // Enemy
                        {
                            turn.AddSpawnPoint(grid.GetElement((int)xPos, (int)zPos), true);
                        }
                        else if (layout.layout[i * layout.columns + j] == 4) // Player
                        {
                            turn.AddSpawnPoint(grid.GetElement((int)xPos, (int)zPos), false);
                        }
                    }
                }
            }
            obstaclesActive = true;
            turn.GenerateEnemies();
        }
    }

    // Deletes the obstacles
    public void DeleteObstacles()
    {
        if (obstaclesActive || obstacles.Count > 0)
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
            obstaclesActive = false;
        }
    }
}
