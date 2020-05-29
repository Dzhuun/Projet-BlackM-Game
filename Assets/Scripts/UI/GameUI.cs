﻿using System.Collections;
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
    public Text waitingDrawText;
    public List<CharacterDisplay> charactersDrawInfos;

    [Header("Answer")]
    public GameObject answerDisplay;
    public GameObject waitForAnswerDisplay;
    public Text scenarioDescription;
    public Text scenarioDescriptionWhenWaiting;
    public Text waitingAnswerText;
    public List<AnswerDisplay> answersToSelect;
    public List<AnswerDisplay> answersWhenWaiting;

    [Header("ChooseOpinion")]
    public GameObject chooseOpinionDisplay;
    public GameObject waitForOpinionDisplay;
    public LikesSlider likesSlider;
    public List<CharacterDisplay> charactersOpinionInfos;

    [Header("ShowOpinion")]
    public Text likesUpdateValue;
    public Text popularityUpdateValue;
    public Text likesValue;
    public Text popularityValue;
    public Text itemsLostText;

    [Header("Shopping")]
    public GameObject shoppingDisplay;
    public GameObject waitForShoppingDisplay;
    public Text likesCountText;

    private bool _isLocal;
    private NetworkPlayer _currentPlayer;
    private CharacterDisplay[] _allCharacterDisplays;
    private ItemDisplay[] _allItemDisplays;

    private void Awake()
    {
        _allCharacterDisplays = Resources.FindObjectsOfTypeAll<CharacterDisplay>();
        _allItemDisplays = Resources.FindObjectsOfTypeAll<ItemDisplay>();
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
                charactersStartInfos[i].gameObject.SetActive(true);
                charactersStartInfos[i].SetupInfos(GameManager.orderedPlayers[i].character.nickname, GameManager.orderedPlayers[i].PlayerName, GameManager.orderedPlayers[i].character.avatar);
            }
            else
            {
                charactersStartInfos[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Fades out the initial display of characters.
    /// </summary>
    public void HideCharacters()
    {
        animatorUI.SetTrigger("HideCharacters");
    }

    /// <summary>
    /// Initializes the needed informations of the turn.
    /// </summary>
    /// <param name="currentPlayer">The network player that is currently playing.</param>
    public void BeginTurn(NetworkPlayer currentPlayer)
    {
        _currentPlayer = currentPlayer;
        _isLocal = _currentPlayer == NetworkPlayer.LocalPlayerInstance;

        itemsLostText.text = "";
    }

    /// <summary>
    /// Initializes the UI that displays the currently playing character.
    /// This function is called in an animation event, during the DrawCard animation.
    /// </summary>
    public void InitializeCharactersDisplay()
    {
        foreach (CharacterDisplay charDisplay in _allCharacterDisplays)
        {
            charDisplay.SetupInfos(_currentPlayer.character.nickname,
                                   _currentPlayer.PlayerName,
                                   _currentPlayer.character.avatar);
        }
    }

    /// <summary>
    /// Shows the draw card display. If it is the local player's turn, show the corresponding UI.
    /// </summary>
    /// <param name="currentPlayer">True if it is the local player's turn.</param>
    public void ShowDrawCardDisplay()
    {
        drawCardDisplay.SetActive(_isLocal);
        waitForDrawCardDisplay.SetActive(!_isLocal);


        //for(int i = 0; i < charactersDrawInfos.Count; i++)
        //{
        //    charactersDrawInfos[i].SetupInfos(GameManager.currentPlayer.character.nickname,
        //                                      GameManager.currentPlayer.PlayerName,
        //                                      GameManager.currentPlayer.character.avatar);
        //}

        waitingDrawText.text = string.Format("Le joueur {0} pioche une carte", GameManager.currentPlayer.PlayerName);

        animatorUI.SetTrigger("DrawCard");
    }

    /// <summary>
    /// Activates the UI that shows the answers to select.
    /// </summary>
    /// <param name="scenarioID">The ID of the drawn scenario.</param>
    /// <param name="character">The character that draws the scenario.</param>
    /// <param name="isLocal">Indicates if the local player has to select the answer.</param>
    public void ShowAnswers(int scenarioID, Character character)
    {
        Scenario scenario = Database.GetScenario(GameManager.currentPlayer.popularity, scenarioID);

        if(scenario.commonAnswers.Count < 3)
        {
            throw new System.Exception("Incorrect number of common answers. There must be excatly 3 common answers.");
        }
        
        answerDisplay.SetActive(_isLocal);
        waitForAnswerDisplay.SetActive(!_isLocal);

        // Only update the UI according to the local player state (selecting or waiting for an answer)
        List<AnswerDisplay> answers = _isLocal ? answersToSelect : answersWhenWaiting;

        // Fills the 3 first common answers UI elements
        for(int i = 0; i < 3; i++)
        {
            answers[i].SetupInfos(scenario.commonAnswers[i]);
        }

        // Show the 4th answer only on the currently playing client
        if(_isLocal)
        {
            // Find the answer that correspond to the current character
            // Then setups the UI
            answers[3].SetupInfos(scenario.specificAnswers.Find(x => x.character.nickname == character.nickname));
            scenarioDescription.text = scenario.description;
        }
        else
        {
            // Deactivates the 4th answer
            // TO DO : instead of deactivating it, we can replace its content by a big '?'
            answers[3].gameObject.SetActive(false);
            scenarioDescriptionWhenWaiting.text = scenario.description;
            waitingAnswerText.text = string.Format("Le joueur {0} choisit une réponse", GameManager.currentPlayer.PlayerName);
        }

        // Randomize answers order
        List<int> transformOrder = new List<int>();
        for(int i = 0; i < 4; i++)
        {
            transformOrder.Add(i);
        }

        int randomIndex;
        for(int i = 0; i < 4; i++)
        {
            randomIndex = Random.Range(0, transformOrder.Count);
            answers[transformOrder[randomIndex]].transform.SetSiblingIndex(i);
            transformOrder.RemoveAt(randomIndex);
        }
        
        animatorUI.SetTrigger("ShowAnswers");
    }

    /// <summary>
    /// Activates the UI that shows the opinion to give.
    /// </summary>
    /// <param name="answerID">The ID of the answer.</param>
    public void ShowChooseOpinion(int answerID)
    {
        chooseOpinionDisplay.SetActive(!_isLocal);
        waitForOpinionDisplay.SetActive(_isLocal);

        likesSlider.UpdateSettings(NetworkPlayer.LocalPlayerInstance.popularity);
        
        animatorUI.SetTrigger("ChooseOpinion");
    }

    public void AddLostItem(ItemType itemType, int previousLevel, int newLevel)
    {
        itemsLostText.text = string.Format("{0} Vous avez perdu votre {1} niveau {2}. Ce bien a été rétrogradé au niveau {3}.", itemsLostText.text, itemType.ToString(), previousLevel, newLevel);
    }

    /// <summary>
    /// Activates the UI that shows the results of the opinion phase.
    /// </summary>
    public void ShowOpinionResults(int likesUpdate, float popularityUpdate)
    {
        if(likesUpdate > 0)
        {
            likesUpdateValue.text = string.Format("+{0}", likesUpdate);
        }
        else
        {
            likesUpdateValue.text = likesUpdate.ToString();
        }

        if(popularityUpdate > 0)
        {
            popularityUpdateValue.text = string.Format("+{0}", popularityUpdate);
        }
        else
        {
            popularityUpdateValue.text = popularityUpdate.ToString("F2");
        }

        likesValue.text = GameManager.currentPlayer.likes.ToString();
        popularityValue.text = GameManager.currentPlayer.popularity.ToString("F2");

        animatorUI.SetTrigger("ShowOpinion");
    }

    /// <summary>
    /// Activates the UI that shows the shop.
    /// </summary>
    public void ShowShop()
    {
        shoppingDisplay.SetActive(_isLocal);
        waitForShoppingDisplay.SetActive(!_isLocal);

        if(_isLocal)
        {
            foreach(ItemDisplay itemDisplay in _allItemDisplays)
            {
                itemDisplay.SetupInfos();
            }
        }

        likesCountText.text = GameManager.currentPlayer.likes.ToString();

        animatorUI.SetTrigger("Shop");
    }

    /// <summary>
    /// Updates the shop UI.
    /// </summary>
    public void UpdateShop()
    {
        foreach (ItemDisplay itemDisplay in _allItemDisplays)
        {
            itemDisplay.SetupInfos();
        }
        
        likesCountText.text = GameManager.currentPlayer.likes.ToString();
    }

    /// <summary>
    /// Activates the UI that shows the winner.
    /// </summary>
    /// <param name="winner">The winning player.</param>
    public void ShowEndUI(NetworkPlayer winner)
    {
        // UI stuff

        animatorUI.SetTrigger("GameEnd");
    }
}
