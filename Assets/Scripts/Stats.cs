using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public Character character; // Reference to the character template

    public int hp; // Current HP left

    public int actions; // Current actions left

    void Awake()
    {
        hp = character.hp;
        actions = character.actions;
    }

    // Uses given amount of actions
    public void UseActions(int amount)
    {
        actions -= amount;
        actions = CustomMath.Clamp(actions, 0, character.actions);
    }
}
