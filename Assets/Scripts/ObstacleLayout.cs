using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleLayout", menuName = "Levels/ObstacleLayout")]
public class ObstacleLayout : ScriptableObject
{
    public int rows; // Map rows

    public int columns; // Map columns

    public GameObject obstacle; // Obstacle to place at grid points

    public GameObject gridNoAction; // Reference to the No Action grid

    public int[] layout; // Grid layout on where to define states of each grid
}
