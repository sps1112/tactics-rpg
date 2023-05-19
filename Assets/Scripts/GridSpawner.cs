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

    public List<GameObject> gridElements; // List of all the grid elements

    bool gridActive = false; // Has the grid been generated

    void Start()
    {
        GenerateGrid();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GenerateGrid();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            DeleteGrid();
        }
    }

    // Generates the Grid
    public void GenerateGrid()
    {
        if (!gridActive)
        {
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
            gridActive = true;
        }
    }

    // Deletes the Grid
    public void DeleteGrid()
    {
        if (gridActive)
        {
            foreach (GameObject gridElement in gridElements)
            {
                Destroy(gridElement);
            }
            gridActive = false;
        }
    }
}
