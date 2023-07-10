using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject gridElementUI; // Reference to the parent grid element object

    public TextMeshProUGUI rowText; // Row UI Reference

    public TextMeshProUGUI columnText; // Column UI Reference

    public TextMeshProUGUI posText; // Position UI Reference

    public TextMeshProUGUI turnCountText; // Turn Count UI Reference

    public TextMeshProUGUI turnText; // Turn UI Reference

    void Start()
    {
        ResetGridElementUI();
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
        turnCountText.text = "TURN: " + turnCounter.ToString();
        turnText.text = (type == TurnType.PLAYER) ? ("Player Turn") : ("Enemy Turn");
    }
}
