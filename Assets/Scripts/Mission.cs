using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "RPG/Mission")]
public class Mission : ScriptableObject
{
    public string missionName; // Reference to the mission Name

    public GridLayout level; // Reference to which level to use for this mission

    public ObstacleLayout obstacles; // Reference to which obstacles to use for this mission

    public List<Character> enemies; // Reference to all the enemies in the level
}
