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

    public CharacterUI playerUI; // Reference to the player UI

    public CharacterUI enemyUI; // Reference to the enemy UI

    public GameObject actionsUI; // Reference to the actions UI

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

    // Sets the state of Actions UI
    public void SetActionsUI(bool status)
    {
        actionsUI.SetActive(status);
        GetComponent<InputManager>().SetInput(!status);
    }

    // Sets the Turn UI
    public void SetTurnUI(TurnType type, int turnCounter, Stats stats)
    {
        turnUI.SetActive(true);
        turnCountText.text = "TURN: " + turnCounter.ToString();
        if (type == TurnType.PLAYER)
        {
            enemyUI.gameObject.SetActive(false);
            turnText.text = "Player Turn";
            playerUI.gameObject.SetActive(true);
            playerUI.SetCharacter(stats);
            SetActionsUI(true);
        }
        else
        {
            playerUI.gameObject.SetActive(false);
            SetActionsUI(false);
            turnText.text = "Enemy Turn";
            enemyUI.gameObject.SetActive(true);
            enemyUI.SetCharacter(stats);
        }
    }

    // Hides one of the two character UI
    public void HideTurnUI(bool isPlayer)
    {
        if (isPlayer)
        {
            playerUI.gameObject.SetActive(false);
        }
        else
        {
            enemyUI.gameObject.SetActive(false);
        }
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
        StopCoroutine("ShowHint");
        hintText.text = "";
        hintUI.SetActive(false);
    }

    // Keeps showing the hint on the screen for a small time
    public void ShowHintTemp(string text, float time)
    {
        StopCoroutine("ShowHint");
        ShowHintText(text);
        StartCoroutine("ShowHint", time);
    }

    // Turns off the hint after a time
    IEnumerator ShowHint(float timeLimit)
    {
        float timer = 0.0f;
        while (timer <= timeLimit)
        {
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        HideHint();
    }
}
