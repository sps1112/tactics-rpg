using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridLayout", menuName = "LevelGrid/GridLayout")]
public class GridLayout : ScriptableObject
{
    public int rows; // Map rows

    public int columns; // Map columns

    public GameObject bottom; // Obstacle to place at the bottom of each grid

    public GameObject mid; // Obstacle to place in the middle of bottom and top

    public GameObject top; // Obstacle to place at the top of each grid

    public int[] layout; // The height of each grid
}

