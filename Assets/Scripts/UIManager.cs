using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject gridElementUI; // Reference to the Grid UI

    public TextMeshProUGUI rowText; // Row UI Reference

    public TextMeshProUGUI columnText; // Column UI Reference

    public TextMeshProUGUI posText; // Position UI Reference

    public GameObject turnUI; // Reference to the Turn UI

    public TextMeshProUGUI turnCountText; // Turn Count UI Reference

    public TextMeshProUGUI turnText; // Turn Text UI Reference

    public GameObject hintUI; // Reference to the Hint UI

    public TextMeshProUGUI hintText; // Hint UI Reference

    void Start()
    {
        ResetGridElementUI();
    }

    // Hides all the UI Elements
    public void HideUI()
    {
        gridElementUI.SetActive(false);
        turnUI.SetActive(false);
        hintUI.SetActive(false);
    }

    // Sets the Grid UI
    public void SetGridElementUI(GridElement element)
    {
        gridElementUI.SetActive(true);
        rowText.text = "Row: " + element.row.ToString();
        columnText.text = "Column: " + element.column.ToString();
        posText.text = "Pos: (" + element.pos.x.ToString() + ", " + element.pos.y.ToString() + ")";
    }

    // Resets the Grid UI
    public void ResetGridElementUI()
    {
        rowText.text = "";
        columnText.text = "";
        posText.text = "";
        gridElementUI.SetActive(false);
    }

    // Sets the Turn UI
    public void SetTurnUI(TurnType type, int turnCounter)
    {
        turnUI.SetActive(true);
        turnCountText.text = "TURN: " + turnCounter.ToString();
        turnText.text = (type == TurnType.PLAYER) ? ("Player Turn") : ("Enemy Turn");
    }

    // Shows the hint UI with given text
    public void ShowHintText(string text)
    {
        hintUI.SetActive(true);
        hintText.text = text;
    }

    // Hides the hint UI
    public void HideHint()
    {
        hintText.text = "";
        hintUI.SetActive(false);
    }
}
