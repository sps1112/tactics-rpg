using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "RPG/Character")]
public class Character : ScriptableObject
{
    public string characterName; // Name of the Character

    public Sprite potrait; // Potrait of the the character

    public int speed; // Speed of the character
}
