using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelLayout", menuName = "Levels/LevelLayout")]
public class LevelLayout : ScriptableObject
{
    public string levelName; // Name of this level layout

    public int rows; // Map rows

    public int columns; // Map columns

    public GameObject bottom; // Level object to place at the bottom of each grid

    public GameObject mid; // Level object to place in the middle of each grid

    public GameObject top; // Level object to place at the top of each grid

    public int[] layout; // The height of each grid element
}
