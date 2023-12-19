using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    private ObstacleLayout layout = null; // Layout Object Reference

    public float obstacleHeightOffset = 0.475f; // Offset for spawning the obstacles

    [SerializeField]
    private List<GameObject> obstacles; // List of obstacles spawned

    private bool obstaclesActive = false; // Have the obstacles been generated

    private GridSpawner grid = null; // Reference to the grid

    [SerializeField]
    private GameObject obstacleParent = null; // Obstacles Parent reference

    private LevelManager levelManager = null; // Reference to the Turn Manager

    void Start()
    {
        GetData();
        GenerateObstacles();
    }

    // Gets the reference data
    private void GetData()
    {
        layout = GetComponent<GameManager>().mission.obstacles;
        grid = GetComponent<GridSpawner>();
        levelManager = GetComponent<LevelManager>();
    }

    // Generates the obstacles
    public void GenerateObstacles()
    {
        layout = GetComponent<GameManager>().mission.obstacles;
        if (!obstaclesActive)
        {
            if (obstacles.Count <= 0)
            {
                // For Obstacle generation via Editor
                if (layout == null || grid == null || levelManager == null)
                {
                    GetData();
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
                        GridElement currentGrid = grid.GetElement((int)xPos, (int)zPos);
                        if (currentGrid != null)
                        {
                            float yPos = currentGrid.transform.position.y;
                            switch (layout.layout[i * layout.columns + j])
                            {
                                case 1:
                                    // Obstacle
                                    yPos += obstacleHeightOffset;
                                    GameObject obs = Instantiate(layout.obstacle, new Vector3(xPos, yPos, zPos), Quaternion.identity, obstacleParent.transform);
                                    currentGrid.SetState(GridState.BLOCKED);
                                    obstacles.Add(obs);
                                    break;
                                case 2:
                                    // No Action Grid
                                    yPos += grid.heightOffsets[2];
                                    GameObject top = Instantiate(layout.gridNoAction, new Vector3(xPos, yPos, zPos), Quaternion.identity, obstacleParent.transform);
                                    currentGrid.canActOnGrid = false;
                                    obstacles.Add(top);
                                    break;
                                case 3:
                                    // Enemy Spawn Point
                                    levelManager.AddSpawnPoint(currentGrid, true);
                                    break;
                                case 4:
                                    // Player Spawn Point
                                    levelManager.AddSpawnPoint(currentGrid, false);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            obstaclesActive = true;
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
