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

    public GameObject hintUI; // Reference to the Hint UI

    public TextMeshProUGUI hintText; // Hint UI Reference

    private bool isHintOn = false; // Whether a hint is being shown (not temporary)

    private bool isTempHintOn = false; // Whether a temporary hint is being shown

    private string prevHintText = ""; // The previous hint which was being displayed

    public GameObject titleUI; // Reference to the title UI

    public TextMeshProUGUI levelText; // Level UI Reference

    public TextMeshProUGUI missionText; // Mission UI Reference

    public CharacterUI playerUI; // Reference to the player UI

    public CharacterUI enemyUI; // Reference to the enemy UI

    public GameObject actionsUI; // Reference to the actions UI

    public GameObject turnUI; // Reference to the Turn UI

    public TextMeshProUGUI turnCountText; // Turn Count UI Reference

    public TextMeshProUGUI turnText; // Turn Text UI Reference

    public List<Image> turnPortraits; // Reference to the Turn portraits

    void Awake()
    {
        HideUI();
    }

    // Hides all the UI Elements
    public void HideUI()
    {
        ResetGridElementUI();
        HideHint(true);
        HideCharacterUI(true); // Hide player ui
        HideCharacterUI(false); // Hide enemy ui
        HideTurnUI();
    }

    // Resets the Grid UI
    public void ResetGridElementUI()
    {
        rowText.text = "";
        columnText.text = "";
        posText.text = "";
        gridElementUI.SetActive(false);
    }

    // Sets the Grid UI
    public void SetGridElementUI(GridElement element)
    {
        gridElementUI.SetActive(true);
        rowText.text = "Row: " + element.row.ToString();
        columnText.text = "Column: " + element.column.ToString();
        posText.text = "Pos: (" + element.pos.x.ToString() + ", " + element.pos.y.ToString() + ")";
    }

    // Hides the hint UI
    public void HideHint(bool forceStop)
    {
        if (forceStop) // Force stop all the ui elements
        {
            isTempHintOn = false;
            isHintOn = false;
            prevHintText = "";
            hintText.text = "";
        }
        else
        {
            if (isTempHintOn) // The hint was a temp hint
            {
                StopCoroutine("HideHintDelay");
                isTempHintOn = false;
                if (isHintOn) // If there was a hint before this temp hint
                {
                    ShowHint(prevHintText, false); // Show the previous hint without closing hint ui
                    prevHintText = "";
                    return;
                }
                else
                {
                    hintText.text = "";
                }
            }
            else // The hint was a normal hint
            {
                isHintOn = false;
                prevHintText = "";
                hintText.text = "";
            }
        }
        hintUI.SetActive(false);
    }

    // Shows the hint UI with given text
    public void ShowHint(string text, bool isTemp, float hintPeriod = 0.75f)
    {
        hintUI.SetActive(true);
        if (isTemp) // Show new temp hint
        {
            if (isTempHintOn) // If there is already a temp hint, reset it
            {
                StopCoroutine("HideHintDelay");
                if (isHintOn)
                {
                    hintText.text = prevHintText; // Reset to original hint
                }
            }
            StartCoroutine("HideHintDelay", hintPeriod); // Start this temp hint's timer
            isTempHintOn = true;
            if (isHintOn) // If there is already a hint, save it as backup
            {
                prevHintText = hintText.text;
            }
        }
        else // Show new hint
        {
            isHintOn = true;
            if (isTempHintOn) // If there is already a temp hint, stop it
            {
                StopCoroutine("HideHintDelay");
                isTempHintOn = false;
            }
        }
        hintText.text = text;
    }

    // Shows the character UI for the character with its stats
    public void ShowCharacterUI(bool isPlayer, Stats stats)
    {
        if (isPlayer)
        {
            playerUI.gameObject.SetActive(true);
            playerUI.SetCharacter(stats);
        }
        else
        {
            enemyUI.gameObject.SetActive(true);
            enemyUI.SetCharacter(stats);
        }
    }

    // Hides the character UI for the character
    public void HideCharacterUI(bool isPlayer)
    {
        if (isPlayer)
        {
            playerUI.gameObject.SetActive(false);
            SetActionsUI(false);
        }
        else
        {
            enemyUI.gameObject.SetActive(false);
        }
    }

    // Sets the state of Actions UI
    public void SetActionsUI(bool status)
    {
        actionsUI.SetActive(status);
    }

    // Sets the Turn UI
    public void SetTurnUI(TurnType type, int turnCounter, GameObject current, List<GameObject> turnQueue)
    {
        turnUI.SetActive(true); // Show Turn UI
        turnCountText.text = "TURN: " + turnCounter.ToString();
        if (type == TurnType.PLAYER) // Player's turn
        {
            turnText.text = "Player Turn";
            HideCharacterUI(false); // Hide enemy
            ShowCharacterUI(true, current.GetComponent<Stats>()); // Show player
        }
        else // Enemy's turn
        {
            turnText.text = "Enemy Turn";
            HideCharacterUI(true); // Hide player
            ShowCharacterUI(false, current.GetComponent<Stats>()); // Show Enemy
        }
        for (int i = 0; i < turnPortraits.Count; i++) // Show all the characters for the current and later turns
        {
            if (i == 0) // First is the current character
            {
                turnPortraits[0].sprite = current.GetComponent<Stats>().character.potrait;
            }
            else // Rest is occupied by the turn queue
            {
                turnPortraits[i].sprite = turnQueue[i - 1].GetComponent<Stats>().character.potrait;
            }
        }
    }

    // Hides one of the two character UI
    public void HideTurnUI()
    {
        turnUI.SetActive(false);
    }

    // Turns off the hint after a time
    private IEnumerator HideHintDelay(float timeLimit)
    {
        yield return new WaitForSeconds(timeLimit);
        HideHint(false);
    }

    // Shows the Title UI and later starts the game
    public IEnumerator ShowTitleUI(float timeLimit)
    {
        titleUI.SetActive(true);
        // Set UI to mission details
        Mission mission = GetComponent<GameManager>().mission;
        levelText.text = mission.level.levelName;
        missionText.text = mission.missionName;
        // Play out the Start animation
        Animator anim = titleUI.transform.GetChild(0).gameObject.GetComponent<Animator>();
        while (!Standard.IsAnimationPlaying(anim, "UI Normal"))
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        // Show Title UI for given time limit
        yield return new WaitForSeconds(timeLimit);
        // Play out the End animation
        anim.Play("UI Fade Out");
        yield return new WaitForSeconds(Time.deltaTime);
        while (Standard.IsAnimationPlaying(anim, "UI Fade Out"))
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        // Hide title and start spawning
        titleUI.SetActive(false);
        GetComponent<TurnManager>().StartCoroutine("StartPlayerSpawning");
    }
}
