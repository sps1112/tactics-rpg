using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "RPG/Character")]
public class Character : ScriptableObject
{
    public string characterName; // Name of the Character

    public Sprite potrait; // Potrait of the the character

    public int speed; // Speed of the character

    public int jump; // How high can characters perform actions for adjacent grids 

    public int hp; // Max HP for the character

    public int actions; // Max actions for the character
}
