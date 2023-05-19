using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleLayout", menuName = "ObstacleLayout")]
public class ObstacleLayout : ScriptableObject
{
    public int rows; // Map rows

    public int columns; // Map columns

    public GameObject obstacle; // Obstacle to place at grid points

    public bool[] layout; // Grid layout on where to place obstacles
}
