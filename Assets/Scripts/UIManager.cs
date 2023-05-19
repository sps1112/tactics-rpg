using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI rowText; // Row UI Reference

    public TextMeshProUGUI columnText; // Column UI Reference

    public TextMeshProUGUI posText; // Position UI Reference

    void Start()
    {
        ResetGirdElementUI();
    }

    // Sets the Grid UI
    public void SetGridElementUI(GridElement element)
    {
        rowText.text = "Row: " + element.row.ToString();
        columnText.text = "Column: " + element.column.ToString();
        posText.text = "Pos: (" + element.pos.x.ToString() + ", " + element.pos.y.ToString() + ")";
    }

    // Resets the Grid UI
    public void ResetGirdElementUI()
    {
        rowText.text = "";
        columnText.text = "";
        posText.text = "";
    }
}
