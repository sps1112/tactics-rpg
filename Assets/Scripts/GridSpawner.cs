using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public Transform gridOrigin; // Start point for grid generation

    public GameObject gridCube; // Prefab for Grid Element

    [SerializeField]
    private int rows = 10; // Grid Row Count

    [SerializeField]
    private int columns = 10; // Grid Column Count

    public bool diagonalMotion = true;

    public List<GameObject> gridElements; // List of all the grid elements

    private bool gridActive = false; // Has the grid been generated

    void Awake()
    {
        GenerateGrid();
    }

    // Generates the Grid
    public void GenerateGrid()
    {
        if (!gridActive)
        {
            if (gridElements.Count > 0)
            {
                gridActive = true;
                return;
            }
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    float xPos = gridOrigin.position.x + j * 1.0f;
                    float zPos = gridOrigin.position.z + i * 1.0f;
                    float yPos = gridOrigin.position.y;
                    GameObject cube = Instantiate(gridCube, new Vector3(xPos, yPos, zPos), Quaternion.identity, gridOrigin.transform);
                    cube.GetComponent<GridElement>().SetInitialState(i + 1, j + 1, new Vector2(xPos, zPos));
                    gridElements.Add(cube);
                }
            }
            SetupNeighbours();
            gridActive = true;
        }
    }

    void SetupNeighbours()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int xPos = (int)(gridOrigin.position.x + j * 1.0f);
                int zPos = (int)(gridOrigin.position.z + i * 1.0f);
                GridElement element = GetElement(xPos, zPos);
                for (int x = -1; x <= 1; x++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x != 0 || z != 0)
                        {
                            int X = Mathf.Clamp(xPos + x, 0, columns - 1);
                            int Z = Mathf.Clamp(zPos + z, 0, rows - 1);
                            GridElement neighbour = GetElement(X, Z);
                            if (!element.neighbours.Contains(neighbour) && neighbour != element && neighbour.IsTraversable(true))
                            {
                                if (!diagonalMotion)
                                {
                                    if (x == 0 || z == 0)
                                    {
                                        element.neighbours.Add(neighbour);
                                    }
                                }
                                else
                                {
                                    element.neighbours.Add(neighbour);
                                }
                            }
                        }
                    }

                }
            }
        }
    }

    // Deletes the Grid
    public void DeleteGrid()
    {
        if (gridActive || gridElements.Count > 0)
        {
            foreach (GameObject gridElement in gridElements)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(gridElement);
                }
                else
                {
                    Destroy(gridElement);
                }
            }
            gridElements.Clear();
            gridActive = false;
        }
    }

    // Returns the Grid Element based on Position
    public GridElement GetElement(int x, int z)
    {
        return gridElements[x + z * columns].GetComponent<GridElement>();
    }
}
