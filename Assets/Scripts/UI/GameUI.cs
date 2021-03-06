﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Core")]
    public GameObject mainPanel;
    public GameObject turnCountPanel;
    public TextMeshProUGUI currentTurnCount;
    public TextMeshProUGUI maxTurnCount;

    [Header("Player Infos")]
    public GameObject playersUI;
    public PlayerSelectorUI playerSelectorUI;
    public FameGauge fameGauge;
    public MentalHealthUI mentalHealthUI;
    public Color deactivatedTraitColor = Color.gray;
    public TextMeshProUGUI traitsList;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerLikes;
    public TextMeshProUGUI playerFame;
    public TextMeshProUGUI playerDescription;

    [Header("Start")]
    public GameObject startPhaseDisplay;
    public GameObject startPhase1;
    public TextMeshProUGUI startPhaseText;
    public GameObject startPhase2;
    public Button startFirstTurnButton;
    public GameObject startFirstTurnIcon;
    public GameObject startFirstTurnText;

    [Header("DrawCard")]
    public GameObject drawCardDisplay;
    public TextMeshProUGUI drawText;
    public TMP_InputField drawCardInputField;

    [Header("WaitForDrawCard")]
    public GameObject waitForDrawCardDisplay;
    public TextMeshProUGUI waitDrawTitle;
    public Image waitDrawAvatar;
    public TextMeshProUGUI waitDrawText;

    [Header("Answer")]
    public GameObject answerDisplay;
    public TextMeshProUGUI scenarioDescription;
    public Button validateAnswerButton;
    public List<AnswerDisplay> answersToSelect;

    [Header("WaitForAnswer")]
    public GameObject waitForAnswerDisplay;
    public TextMeshProUGUI waitForAnswerTitle;
    public TextMeshProUGUI scenarioDescriptionWhenWaiting;
    public List<AnswerDisplay> answersWhenWaiting;

    [Header("ChooseOpinion")]
    public GameObject chooseOpinionDisplay;
    public Button validateOpinionButton;
    public GameObject validatedOpinionIcon;
    public GameObject validateOpinionText;
    public TextMeshProUGUI scenarioDescription_chooseOpinion;
    public TextMeshProUGUI answerText_chooseOpinion;
    public TextMeshProUGUI voteTitle;
    public TextMeshProUGUI maxVoteText;
    public TextMeshProUGUI noteValue;
    public ParticleSystem likeParticles;
    public ParticleSystem dislikeParticles;
    public LikesIncrementHandler likesIncrementHandler;

    [Header("WaitForOpinion")]
    public GameObject waitForOpinionDisplay;
    public TextMeshProUGUI scenarioDescription_waitOpinion;
    public TextMeshProUGUI answerText_waitOpinion;
    public SocietyResultUI societyResult;


    [Header("ShowOpinion")]
    public GameObject showOpinionDisplay;
    public GameObject opinionPanel;
    public GameObject bilanPanel;
    public OpinionResultUI opinionResultUI;
    public TextMeshProUGUI scenarioDescription_showOpinion;
    public TextMeshProUGUI answerText_showOpinion;
    public TextMeshProUGUI positivePlayersLikesUpdateValue;
    public TextMeshProUGUI negativePlayersLikesUpdateValue;
    public TextMeshProUGUI societyLikesUpdateValue;
    public Image societyLikesUpdateIcon;
    public TextMeshProUGUI likesUpdateValue;
    public Image totalLikesUpdateIcon;
    public TextMeshProUGUI fameUpdateValue;
    public TextMeshProUGUI mentalHealthUpdateValue;
    public TextMeshProUGUI mentalHealthDislikesValue;
    public TextMeshProUGUI mentalHealthValue;
    public Button skipOpinionButton;

    [Header("Shopping")]
    public GameObject shoppingDisplay;
    public TextMeshProUGUI shoppingText;

    [Header("WaitForShopping")]
    public GameObject waitForShoppingDisplay;
    public TextMeshProUGUI waitForShoppingText;

    [Header("End")]
    public GameObject endPhaseDisplay;
    public GameObject playerScoresDisplay;
    public GameObject winnerDisplay;
    public List<PlayerScoresUI> playerScores;
    public WinnerUI winnerUI;
    public List<WinnerUI> loserUIs;

    private bool _isLocal;
    private NetworkPlayer _currentPlayer;
    private NetworkPlayer _observedPlayer;
    private List<LostItemData> _lostItems;

    public static GameUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        mainPanel.SetActive(true);
        playersUI.SetActive(true);

        turnCountPanel.SetActive(false);
        startPhaseDisplay.SetActive(false);
        drawCardDisplay.SetActive(false);
        waitForDrawCardDisplay.SetActive(false);
        answerDisplay.SetActive(false);
        waitForAnswerDisplay.SetActive(false);
        chooseOpinionDisplay.SetActive(false);
        showOpinionDisplay.SetActive(false);
        waitForOpinionDisplay.SetActive(false);
        shoppingDisplay.SetActive(false);
        endPhaseDisplay.SetActive(false);
    }


    /// <summary>
    /// Shows the first part of the beginning of the game.
    /// </summary>
    public void ShowStartPhase1(NetworkPlayer player)
    {
        InitializeAvatars();

        ShowPlayerInfos(player);

        turnCountPanel.SetActive(true);
        maxTurnCount.text = GameManager.Instance.turnCount.ToString();


        startPhaseDisplay.SetActive(true);

        startPhase1.SetActive(true);
        startPhase2.SetActive(false);

        startPhaseText.text = string.Format("Tu incarnes {0}, voici ton profil.", player.character.GetFirstName());
    }

    /// <summary>
    /// Shows the second part of the beginning of the game.
    /// </summary>
    public void ShowStartPhase2()
    {
        startPhase1.SetActive(false);
        startPhase2.SetActive(true);

        startFirstTurnIcon.SetActive(false);
    }

    /// <summary>
    /// Deactivates the button that sets ready for the first turn.
    /// </summary>
    public void DeactivateFirstTurnButton()
    {
        startFirstTurnButton.interactable = false;
        startFirstTurnIcon.SetActive(true);
        startFirstTurnText.SetActive(false);
    }

    /// <summary>
    /// Initializes the needed informations of the turn.
    /// </summary>
    /// <param name="currentPlayer">The network player that is currently playing.</param>
    public void BeginTurn(NetworkPlayer currentPlayer, int currentTurn)
    {
        _isLocal = currentPlayer == NetworkPlayer.LocalPlayerInstance;
        _observedPlayer = NetworkPlayer.LocalPlayerInstance;

        _lostItems = new List<LostItemData>();
        currentTurnCount.text = (currentTurn + 1).ToString();
    }

    /// <summary>
    /// Initializes the displayed avatars.
    /// </summary>
    private void InitializeAvatars()
    {
        List<NetworkPlayer> players = new List<NetworkPlayer>(GameManager.orderedPlayers);

        playerSelectorUI.SetPlayer(0, NetworkPlayer.LocalPlayerInstance);
        
        players.Remove(NetworkPlayer.LocalPlayerInstance);

        for(int i = 1; i < GameManager.orderedPlayers.Count; i++)
        {
            playerSelectorUI.SetPlayer(i, players[0]);
            players.RemoveAt(0);
        }
    }
    
    /// <summary>
    /// Displays the informations of a player.
    /// </summary>
    /// <param name="player">The player to display.</param>
    public void ShowPlayerInfos(NetworkPlayer player)
    {
        _observedPlayer = player;

        // Display texts
        playerName.text = player.character.nickname;
        playerFame.text = player.fame.ToString("F2");
        playerLikes.text = player.likes.ToString();
        playerDescription.text = player.character.description;

        // Display graphics UI
        fameGauge.SetGauge(player.fame);

        if(player == NetworkPlayer.LocalPlayerInstance)
        {
            mentalHealthUI.SetMentalHealth(player);
        }
        else
        {
            mentalHealthUI.SetActive(false);
        }

        // Display traits
        string traitsText = string.Empty;
        for(int i = 0; i < player.character.traits.Count; i++)
        {
            if(player.character.traits[i].isActive)
            {
                traitsText += string.Format(", {0}", player.character.traits[i].trait.GetTraitName(player.character));
            }
            else
            {
                // Strike and change the color of the inactive trait
                traitsText += string.Format(", <s><color=#{0}>{1}</color></s>", ColorUtility.ToHtmlStringRGBA(deactivatedTraitColor), player.character.traits[i].trait.GetTraitName(player.character));
            }
        }
       
        // Remove the first two char which are a coma and a white space
        traitsList.text = traitsText.Remove(0, 2);
    }

    /// <summary>
    /// Refreshes the informations of the player.
    /// </summary>
    public void RefreshPlayerInfos()
    {
        ShowPlayerInfos(_observedPlayer);
    }

    /// <summary>
    /// Shows the draw card display. If it is the local player's turn, show the corresponding UI.
    /// </summary>
    /// <param name="currentPlayer">True if it is the local player's turn.</param>
    public void ShowDrawCardDisplay(NetworkPlayer player)
    {
        int fameLevel = Mathf.FloorToInt(player.fame);

        startPhaseDisplay.SetActive(false);
        shoppingDisplay.SetActive(false);
        waitForShoppingDisplay.SetActive(false);

        drawCardDisplay.SetActive(_isLocal);
        waitForDrawCardDisplay.SetActive(!_isLocal);

        if(_isLocal)
        {
            drawText.text = string.Format("Pioche une carte évènement {0} étoile{1}", fameLevel, fameLevel > 1 ? "s" : string.Empty);
            drawCardInputField.text = string.Empty;
        }
        else
        {
            waitDrawTitle.text = string.Format("A {0} de jouer !", player.character.GetFirstName());
            waitDrawAvatar.sprite = player.character.avatar;
            waitDrawText.text = string.Format("En attente que {0} pioche une carte {1} étoile{2}..", player.character.GetFirstName(), fameLevel, fameLevel > 1 ? "s" : string.Empty);
        }
    }

    /// <summary>
    /// Activates the UI that shows the answers to select.
    /// </summary>
    /// <param name="scenarioID">The ID of the drawn scenario.</param>
    /// <param name="character">The character that draws the scenario.</param>
    /// <param name="isLocal">Indicates if the local player has to select the answer.</param>
    public void ShowAnswers(int scenarioID, NetworkPlayer player)
    {
        drawCardDisplay.SetActive(false);
        waitForDrawCardDisplay.SetActive(false);

        Scenario scenario = Database.GetScenario(player.fame, scenarioID);

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

        // Find the answer that correspond to the current character then setups the UI
        answers[3].SetupInfos(scenario.specificAnswers.Find(x => x.character.nickname == player.character.nickname));

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
        
        if(_isLocal)
        {
            scenarioDescription.text = scenario.description;
            validateAnswerButton.interactable = false;
        }
        else
        {
            scenarioDescriptionWhenWaiting.text = scenario.description;
            waitForAnswerTitle.text = string.Format("A {0} de jouer !", player.character.GetFirstName());
        }
    }

    /// <summary>
    /// Activates the UI that shows the opinion to give.
    /// </summary>
    /// <param name="answerID">The ID of the answer.</param>
    public void ShowChooseOpinion(string answer, string question, AnswerResultType resultType)
    {
        answerDisplay.SetActive(false);
        waitForAnswerDisplay.SetActive(false);

        RefreshPlayerInfos();

        chooseOpinionDisplay.SetActive(!_isLocal);
        waitForOpinionDisplay.SetActive(_isLocal);

        if(_isLocal)
        {
            scenarioDescription_waitOpinion.text = question;
            answerText_waitOpinion.text = answer;
            societyResult.ShowResult(resultType, GameManager.currentPlayer.fame);
        }
        else
        {
            validateOpinionButton.interactable = true;
            validateOpinionText.SetActive(true);
            validatedOpinionIcon.SetActive(false);
            scenarioDescription_chooseOpinion.text = question;
            answerText_chooseOpinion.text = answer;
            voteTitle.text = string.Format("Note {0}",GameManager.currentPlayer.character.GetFirstName());

            likesIncrementHandler.UpdateSettings(NetworkPlayer.LocalPlayerInstance.fame);
            maxVoteText.text = string.Format("Tu peux distribuer {0} votes au maximum", LikesIncrementHandler.maxValue);
        }
    }

    /// <summary>
    /// Called when validating the opinion. Deactivates the validate button.
    /// </summary>
    public void DeactivateOpinionButton()
    {
        validateOpinionButton.interactable = false;
        validateOpinionText.SetActive(false);
        validatedOpinionIcon.SetActive(true);
    }

    /// <summary>
    /// Updates the displayed value of the note given.
    /// </summary>
    /// <param name="newValue">The new value to display.</param>
    public void UpdateOpinion(float newValue)
    {
        if(newValue <= 0)
        {
            noteValue.text = newValue.ToString();
        }
        else
        {
            noteValue.text = string.Format("+{0}", newValue.ToString());
        }
    }

    /// <summary>
    /// Registers a lost item.
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="previousLevel"></param>
    /// <param name="newLevel"></param>
    public void AddLostItem(ItemType itemType, int previousLevel, int newLevel)
    {
        _lostItems.Add(new LostItemData(itemType, previousLevel, newLevel));
    }

    /// <summary>
    /// Activates the UI that shows the results of the opinion phase.
    /// </summary>
    public void ShowOpinionResults(int positivePlayersLikes, int negativePlayersLikes, int societyLikes, int mentalHealthUpdate, int mentalHealthDislikes, int totalLikesUpdate, float popularityUpdate, string answer, string situation)
    {
        chooseOpinionDisplay.SetActive(false);
        waitForOpinionDisplay.SetActive(false);

        showOpinionDisplay.SetActive(true);
        opinionPanel.SetActive(true);
        bilanPanel.SetActive(false);

        RefreshPlayerInfos();

        skipOpinionButton.gameObject.SetActive(_isLocal);

        scenarioDescription_showOpinion.text = situation;
        answerText_showOpinion.text = answer;

        if(positivePlayersLikes > 0)
        {
            positivePlayersLikesUpdateValue.text = string.Format("+{0}", positivePlayersLikes);
        }
        else
        {
            positivePlayersLikesUpdateValue.text = positivePlayersLikes.ToString();
        }

        negativePlayersLikesUpdateValue.text = negativePlayersLikes.ToString();

        if (societyLikes > 0)
        {
            societyLikesUpdateValue.text = string.Format("+{0}", societyLikes);
        }
        else
        {
            societyLikesUpdateValue.text = societyLikes.ToString();
        }

        societyLikesUpdateIcon.rectTransform.localScale = new Vector3(societyLikes >= 0 ? 1 : -1, societyLikes >= 0 ? 1 : -1);

        if (totalLikesUpdate > 0)
        {
            likesUpdateValue.text = string.Format("+{0}", totalLikesUpdate);
        }
        else
        {
            likesUpdateValue.text = totalLikesUpdate.ToString();
        }

        totalLikesUpdateIcon.rectTransform.localScale = new Vector3(totalLikesUpdate >= 0 ? 1 : -1, totalLikesUpdate >= 0 ? 1 : -1);

        if (popularityUpdate > 0)
        {
            fameUpdateValue.text = string.Format("+{0}", popularityUpdate.ToString("F2"));
        }
        else
        {
            fameUpdateValue.text = popularityUpdate.ToString("F2");
        }

        if(mentalHealthUpdate > 0)
        {
            mentalHealthUpdateValue.text = string.Format("+{0}", mentalHealthUpdate);
        }
        else
        {
            mentalHealthUpdateValue.text = mentalHealthUpdate.ToString();
        }
        
        mentalHealthDislikesValue.text = mentalHealthDislikes.ToString();    
        
        mentalHealthValue.text = string.Format("{0}%", GameManager.currentPlayer.mentalHealth);
    }

    /// <summary>
    /// Activates the UI that shows the fame and items lost.
    /// </summary>
    public void ShowBilan(NetworkPlayer player, int previousRank, int newRank, string firstNewTrait, string secondNewTrait)
    {
        opinionPanel.SetActive(false);
        bilanPanel.SetActive(true);

        opinionResultUI.SetupInfos(player, _lostItems, previousRank, newRank, firstNewTrait, secondNewTrait);
    }

    /// <summary>
    /// Activates the UI that shows the shop.
    /// </summary>
    public void ShowShop(NetworkPlayer player)
    {
        showOpinionDisplay.SetActive(false);

        shoppingDisplay.SetActive(_isLocal);
        waitForShoppingDisplay.SetActive(!_isLocal);

        if(_isLocal)
        {
            int fameLevel = Mathf.FloorToInt(player.fame);
            shoppingText.text = string.Format("Tu peux acheter des biens d'une valeur maximale de {0} étoile{1}.", fameLevel, fameLevel > 1 ? "s" : string.Empty);
        }
        else
        {
            waitForShoppingText.text = string.Format("Patience, {0} effectue ses achats", player.character.GetFirstName());
        }

        ShopManager.Instance.UpdateDisplays();
    }

    /// <summary>
    /// Activates the UI that shows the winner.
    /// </summary>
    /// <param name="winner">The winning player.</param>
    public void ShowEndUI()
    {
        mainPanel.SetActive(false);
        playersUI.SetActive(false);
        shoppingDisplay.SetActive(false);

        endPhaseDisplay.SetActive(true);

        playerScoresDisplay.SetActive(true);
        winnerDisplay.SetActive(false);

        for(int i = 0; i < playerScores.Count; i++)
        {
            if(i < GameManager.orderedPlayers.Count)
            {
                playerScores[i].SetupInfos(GameManager.orderedPlayers[i]);
            }
            else
            {
                playerScores[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Activates the UI that shows the winner.
    /// </summary>
    /// <param name="winner">The winning player.</param>
    public void ShowWinnerUI(NetworkPlayer winner)
    {
        playerScoresDisplay.SetActive(false);
        winnerDisplay.SetActive(true);

        // Show winner UI
        winnerUI.SetupInfos(winner);

        // Show losers UI
        List<NetworkPlayer> players = new List<NetworkPlayer>(GameManager.orderedPlayers);
        players.Remove(winner);
        
        for(int i = 0; i < loserUIs.Count; i++)
        {
            int score = 0;
            NetworkPlayer nextPlayer = null;

            for(int j = 0; j < players.Count; j++)
            {
                if(players[j].totalScore > score)
                {
                    nextPlayer = players[j];
                    score = nextPlayer.totalScore;
                }
            }

            if(nextPlayer != null)
            {
                loserUIs[i].SetupInfos(nextPlayer);
                players.Remove(nextPlayer);
            }
            else
            {
                loserUIs[i].gameObject.SetActive(false);
            }
        }

    }
}

public class LostItemData
{
    public ItemType itemType;

    public int previousLevel;

    public int newLevel;

    public LostItemData(ItemType _itemType, int _previousLevel, int _newLevel)
    {
        itemType = _itemType;

        previousLevel = _previousLevel;

        newLevel = _newLevel;
    }
}
