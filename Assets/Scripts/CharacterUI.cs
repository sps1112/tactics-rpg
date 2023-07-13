using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUI : MonoBehaviour
{
    public TextMeshProUGUI nameText; // Reference to the Name UI

    public Image portrait; // Reference to the Portrait UI

    public TextMeshProUGUI hpText; // Reference to the HP UI

    public TextMeshProUGUI actionText; // Reference to the Actions UI

    // Sets the Character Details
    public void SetCharacter(Stats stats)
    {
        nameText.text = stats.character.characterName;
        portrait.sprite = stats.character.potrait;
        hpText.text = "HP:- " + stats.hp + "/" + stats.character.hp;
        actionText.text = "Actions:- " + stats.actions + "/" + stats.character.actions;
    }
}
