using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Core")]
    public Animator animatorUI;

    [Header("Start")]
    public GameObject charactersDisplay;
    public List<CharacterDisplay> charactersStartInfos;

    [Header("DrawCard")]
    public GameObject drawCardDisplay;
    public GameObject waitForDrawCardDisplay;
    public List<CharacterDisplay> charactersDrawInfos;
    public Text waitingDrawTitle;

    [Header("Answer")]
    public GameObject answerDisplay;
    public GameObject waitForAnswerDisplay;
    public List<AnswerDisplay> answersToSelect;
    public List<AnswerDisplay> answersWhenWaiting;
    

    // Start is called before the first frame update
    void Start()
    {
        charactersDisplay.SetActive(false);
    }
    
    /// <summary>
    /// Displays the character assigned to each player.
    /// </summary>
    public void DisplayCharacters()
    {
        charactersDisplay.SetActive(true);

        for(int i = 0; i < charactersStartInfos.Count; i++)
        {
            if(i < GameManager.orderedPlayers.Count)
            {
                charactersStartInfos[i].SetupInfos(GameManager.orderedPlayers[i].character.nickname, GameManager.orderedPlayers[i].PlayerName, GameManager.orderedPlayers[i].character.avatar);
            }
            else
            {
                charactersStartInfos[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideCharacters()
    {
        animatorUI.SetTrigger("HideCharacters");
    }

    /// <summary>
    /// Shows the draw card display. If it is the local player's turn, show the corresponding UI.
    /// </summary>
    /// <param name="currentPlayer">True if it is the local player's turn.</param>
    public void ShowDrawCardDisplay(bool currentPlayer)
    {
        drawCardDisplay.SetActive(currentPlayer);
        waitForDrawCardDisplay.SetActive(!currentPlayer);

        for(int i = 0; i < charactersDrawInfos.Count; i++)
        {
            charactersDrawInfos[i].SetupInfos(GameManager.currentPlayer.character.nickname,
                                              GameManager.currentPlayer.PlayerName,
                                              GameManager.currentPlayer.character.avatar);
        }

        waitingDrawTitle.text = string.Format("Le joueur {0} pioche une carte", GameManager.currentPlayer.PlayerName);

        animatorUI.SetTrigger("DrawCard");
    }

    /// <summary>
    /// Activates the UI that show the answers to select.
    /// </summary>
    /// <param name="scenarioID">The ID of the drawn scenario.</param>
    /// <param name="character">The character that draws the scenario.</param>
    /// <param name="isLocal">Indicates if the local player has to select the answer.</param>
    public void ShowAnswers(int scenarioID, Character character, bool isLocal)
    {
        Scenario scenario = Database.scenarios.Find(x => x.id == scenarioID);

        if(scenario.commonAnswers.Count < 3)
        {
            throw new System.Exception("Incorrect number of common answers. There must be excatly 3 common answers.");
        }
        
        answerDisplay.SetActive(isLocal);
        waitForAnswerDisplay.SetActive(!isLocal);

        // Only update the UI according to the local player state (selecting or waiting for an answer)
        List<AnswerDisplay> answers = isLocal ? answersToSelect : answersWhenWaiting;

        // Fills the 3 first common answers UI elements
        for(int i = 0; i < 3; i++)
        {
            answers[i].SetupInfos(scenario.commonAnswers[i]);
        }

        // Show the 4th answer only on the currently playing client
        if(isLocal)
        {
            // Find the answer that correspond to the current character
            // Then setups the UI
            answers[3].SetupInfos(scenario.specificAnswers.Find(x => x.character == character.nickname).text);
        }
        else
        {
            answers[3].gameObject.SetActive(false);
        }
    }
}
