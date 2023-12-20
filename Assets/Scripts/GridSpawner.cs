using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    private LevelLayout layout = null; // Reference to the grid layout object

    public Transform gridOrigin; // Start point for grid generation

    public float[] heightOffsets; // Offsets for the different types of gridblocks

    [SerializeField]
    private bool diagonalMotion = true; // Flag for whether diagonal motion will be allowed on the grid

    [SerializeField]
    private List<GameObject> gridParents; // List of all Grid Parent objects

    [SerializeField]
    private List<GameObject> gridElements; // List of all the grid elements

    private bool gridActive = false; // Has the grid been generated

    void Awake()
    {
        GenerateGrid();
    }

    // Generates the Grid
    public void GenerateGrid()
    {
        layout = GetComponent<GameManager>().mission.level;
        if (!gridActive)
        {
            if (gridParents.Count <= 0)
            {
                // Generate all the grid elements
                GameObject empty = new GameObject("Empty");
                for (int i = 0; i < layout.rows; i++)
                {
                    for (int j = 0; j < layout.columns; j++)
                    {
                        float xPos = gridOrigin.position.x + j * 1.0f;
                        float zPos = gridOrigin.position.z + i * 1.0f;
                        float yPos = gridOrigin.position.y;
                        Vector3 gridPos = new Vector3(xPos, yPos, zPos);
                        GameObject gridParent = Instantiate(empty, gridPos, Quaternion.identity, gridOrigin.transform);
                        gridParent.name = "Grid " + (i + 1).ToString() + "X" + (j + 1).ToString();
                        int height = layout.layout[i * layout.columns + j];
                        if (height <= 0)
                        {
                            gridElements.Add(empty);
                        }
                        else
                        {
                            Instantiate(layout.bottom, gridPos + Vector3.up * heightOffsets[0], Quaternion.identity, gridParent.transform);
                            Vector3 blockPos = gridPos + Vector3.up * heightOffsets[1];
                            for (int k = 1; k <= height; k++)
                            {
                                if (k == height)
                                {
                                    GameObject cube = Instantiate(layout.top, blockPos, Quaternion.identity, gridParent.transform);
                                    cube.GetComponent<GridElement>().SetInitialState(i + 1, j + 1, new Vector2(xPos, zPos), height);
                                    gridElements.Add(cube);
                                }
                                else
                                {
                                    Instantiate(layout.mid, blockPos, Quaternion.identity, gridParent.transform);
                                }
                                blockPos += Vector3.up * heightOffsets[2];
                            }
                        }
                        gridParents.Add(gridParent);
                    }
                }
                if (Application.isEditor)
                {
                    DestroyImmediate(empty);
                }
                else
                {
                    Destroy(empty);
                }
                SetupNeighbours(); // Set all the neighbours now
            }
            gridActive = true;
        }
    }

    // Sets the neighbour grids for all the grid elements
    private void SetupNeighbours()
    {
        for (int i = 0; i < layout.rows; i++)
        {
            for (int j = 0; j < layout.columns; j++)
            {
                int xPos = (int)(gridOrigin.position.x + j * 1.0f);
                int zPos = (int)(gridOrigin.position.z + i * 1.0f);
                if (layout.layout[i * layout.columns + j] > 0)
                {
                    GridElement element = GetElement(xPos, zPos);
                    // Search all possible neighbours of this grid via position
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            if (x != 0 || z != 0)
                            {
                                int X = CustomMath.Clamp(xPos + x, 0, layout.columns - 1);
                                int Z = CustomMath.Clamp(zPos + z, 0, layout.rows - 1);
                                if (layout.layout[Z * layout.columns + X] > 0) // Grid is there at the given position and not an empty space
                                {
                                    GridElement neighbour = GetElement(X, Z);
                                    if (!element.neighbours.Contains(neighbour) && neighbour != element && neighbour.IsTraversable(true))
                                    {
                                        if (!diagonalMotion && x != 0 && z != 0)
                                        {
                                            continue;
                                        }
                                        element.neighbours.Add(neighbour);
                                    }
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
        if (gridActive || gridParents.Count > 0)
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

            foreach (GameObject gridParent in gridParents)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(gridParent);
                }
                else
                {
                    Destroy(gridParent);
                }
            }
            gridParents.Clear();

            gridActive = false;
        }
    }

    // Returns the Grid Element based on Position
    public GridElement GetElement(int x, int z)
    {
        if (layout.layout[x + z * layout.columns] > 0)
        {
            return gridElements[x + z * layout.columns].GetComponent<GridElement>();
        }
        return null;
    }
}
