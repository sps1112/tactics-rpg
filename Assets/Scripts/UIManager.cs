using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public List<Image> turnPortraits; // Reference to the Turn portraits

    public GameObject hintUI; // Reference to the Hint UI

    public TextMeshProUGUI hintText; // Hint UI Reference

    private bool isHintOn = false; // Whether a hint is being shown (not temporary)

    private bool isTempHintOn = false; // Whether a temporary hint is being shown

    private string prevHintText = ""; // The previous hint which was being displayed

    public CharacterUI playerUI; // Reference to the player UI

    public CharacterUI enemyUI; // Reference to the enemy UI

    public GameObject actionsUI; // Reference to the actions UI

    public GameObject titleUI; // Reference to the title UI

    public TextMeshProUGUI levelText; // Level UI Reference

    public TextMeshProUGUI missionText; // Mission UI Reference

    void Start()
    {
        ResetGridElementUI();
    }

    // Hides all the UI Elements
    public void HideUI(bool isPlayer)
    {
        ResetGridElementUI();
        turnUI.SetActive(false);
        HideTurnUI(isPlayer);
        HideHint();
        isHintOn = false;
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
    public void SetActionsUI(bool status, bool inputStatus)
    {
        actionsUI.SetActive(status);
        GetComponent<InputManager>().SetInput(inputStatus);
    }

    // Sets the character UI for the character with status
    public void SetCharacterUI(bool isPlayer, bool status, Stats stats)
    {
        if (isPlayer)
        {
            playerUI.gameObject.SetActive(status);
            if (status)
            {
                playerUI.SetCharacter(stats);
            }
        }
        else
        {
            enemyUI.gameObject.SetActive(status);
            if (status)
            {
                enemyUI.SetCharacter(stats);
            }
        }
    }

    // Sets the Turn UI
    public void SetTurnUI(TurnType type, int turnCounter, GameObject current, List<GameObject> turnQueue)
    {
        turnUI.SetActive(true);
        turnCountText.text = "TURN: " + turnCounter.ToString();
        if (type == TurnType.PLAYER)
        {
            SetCharacterUI(false, false, null);
            turnText.text = "Player Turn";
            SetCharacterUI(true, true, current.GetComponent<Stats>());
            SetActionsUI(true, false);
        }
        else
        {
            SetCharacterUI(true, false, null);
            SetActionsUI(false, false);
            turnText.text = "Enemy Turn";
            SetCharacterUI(false, true, current.GetComponent<Stats>());
        }
        for (int i = 0; i < turnPortraits.Count; i++)
        {
            if (i == 0)
            {
                turnPortraits[0].sprite = current.GetComponent<Stats>().character.potrait;
            }
            else
            {
                turnPortraits[i].sprite = turnQueue[i - 1].GetComponent<Stats>().character.potrait;
            }
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
    public void ShowHintText(string text, bool isTemp)
    {
        hintUI.SetActive(true);
        if (isTemp)
        {
            isTempHintOn = true;
        }
        else
        {
            if (isTempHintOn)
            {
                StopCoroutine("ShowHint");
                isTempHintOn = false;
            }
            isHintOn = true;
            prevHintText = text;
        }
        hintText.text = text;
    }

    // Hides the hint UI
    public void HideHint()
    {
        if (isTempHintOn)
        {
            StopCoroutine("ShowHint");
            isTempHintOn = false;
            if (isHintOn)
            {
                hintText.text = prevHintText;
                return;
            }
            else
            {
                hintText.text = "";
            }
        }
        else if (isHintOn)
        {
            isHintOn = false;
            hintText.text = "";
        }
        hintUI.SetActive(false);
    }

    // Keeps showing the hint on the screen for a small time
    public void ShowHintTemp(string text, float time)
    {
        if (isTempHintOn)
        {
            StopCoroutine("ShowHint");
        }
        ShowHintText(text, true);
        StartCoroutine("ShowHint", time);
    }

    // Turns off the hint after a time
    IEnumerator ShowHint(float timeLimit)
    {
        yield return new WaitForSeconds(timeLimit);
        HideHint();
    }

    // Shows the Title UI and later starts the game
    public IEnumerator ShowTitleUI(float timeLimit)
    {
        titleUI.SetActive(true);
        Mission mission = GetComponent<GameManager>().mission;
        levelText.text = mission.level.levelName;
        missionText.text = mission.missionName;
        Animator anim = titleUI.transform.GetChild(0).gameObject.GetComponent<Animator>();
        while (!Standard.IsAnimationPlaying(anim, "UI Normal"))
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(timeLimit);
        anim.Play("UI Fade Out");
        yield return new WaitForSeconds(Time.deltaTime);
        while (Standard.IsAnimationPlaying(anim, "UI Fade Out"))
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        titleUI.SetActive(false);
        GetComponent<TurnManager>().StartCoroutine("StartPlayerSpawning");
    }
}
