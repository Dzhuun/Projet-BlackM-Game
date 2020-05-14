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
}
