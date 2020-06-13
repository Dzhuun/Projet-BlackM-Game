using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Core")]
    public Animator animatorUI;

    [Header("Player Infos")]
    public PlayerSelectorUI playerSelectorUI;
    public FameGauge fameGauge;
    public MentalHealthUI mentalHealthUI;
    public Color disactivatedTraitColor = Color.gray;
    public TextMeshProUGUI traitsList;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerLikes;
    public TextMeshProUGUI playerFame;
    public TextMeshProUGUI playerDescription;

    [Header("Start")]
    public GameObject charactersDisplay;

    [Header("DrawCard")]
    public GameObject drawCardDisplay;
    public GameObject waitForDrawCardDisplay;
    public Text waitingDrawText;

    [Header("Answer")]
    public GameObject answerDisplay;
    public GameObject waitForAnswerDisplay;
    public TextMeshProUGUI scenarioDescription;
    public TextMeshProUGUI scenarioDescriptionWhenWaiting;
    public TextMeshProUGUI waitingAnswerText;
    public List<AnswerDisplay> answersToSelect;
    public List<AnswerDisplay> answersWhenWaiting;

    [Header("ChooseOpinion")]
    public GameObject chooseOpinionDisplay;
    public GameObject waitForOpinionDisplay;
    public TextMeshProUGUI noteValue;
    public LikesIncrementHandler likesIncrementHandler;


    [Header("ShowOpinion")]
    public GameObject showOpinionDisplay;
    public TextMeshProUGUI societyLikesUpdateValue;
    public TextMeshProUGUI likesUpdateValue;
    public TextMeshProUGUI fameUpdateValue;
    public TextMeshProUGUI mentalHealthUpdateValue;
    public TextMeshProUGUI likesValue;
    public TextMeshProUGUI fameValue;
    public TextMeshProUGUI mentalHealthValue;
    public TextMeshProUGUI itemsLostText;
    public Button skipOpinionButton;

    [Header("Shopping")]
    public GameObject shoppingDisplay;
    public GameObject waitForShoppingDisplay;

    private bool _isLocal;
    private NetworkPlayer _currentPlayer;
    private NetworkPlayer _observedPlayer;

    public static GameUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        drawCardDisplay.SetActive(false);
        waitForDrawCardDisplay.SetActive(false);
        answerDisplay.SetActive(false);
        waitForAnswerDisplay.SetActive(false);
        chooseOpinionDisplay.SetActive(false);
        showOpinionDisplay.SetActive(false);
        waitForOpinionDisplay.SetActive(false);
        shoppingDisplay.SetActive(false);
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
        _observedPlayer = NetworkPlayer.LocalPlayerInstance;
        //itemsLostText.text = "";
        
        InitializeAvatars();

        ShowPlayerInfos(_observedPlayer);
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
        // Display texts
        playerName.text = player.character.name;
        playerFame.text = player.fame.ToString("F2");
        playerLikes.text = player.likes.ToString();
        playerDescription.text = player.character.description;

        // Display graphics UI
        fameGauge.SetGauge(player.fame);
        mentalHealthUI.SetMentalHealth(player.mentalHealth);

        // Display traits
        string traitsText = string.Empty;
        for(int i = 0; i < player.character.traits.Count; i++)
        {
            if(player.character.traits[i].isActive)
            {
                traitsText += string.Format(", {0}", player.character.traits[i].trait.traitName);
            }
            else
            {
                // Strike and change the color of the inactive trait
                traitsText += string.Format(", <s><color=#{0}>{1}</color></s>", ColorUtility.ToHtmlStringRGBA(disactivatedTraitColor), player.character.traits[i].trait.traitName);
            }
        }
       
        // Remove the first two char which are a coma and a white space
        traitsList.text = traitsText.Remove(0, 2);
    }

    /// <summary>
    /// Shows the draw card display. If it is the local player's turn, show the corresponding UI.
    /// </summary>
    /// <param name="currentPlayer">True if it is the local player's turn.</param>
    public void ShowDrawCardDisplay()
    {
        shoppingDisplay.SetActive(false);
        waitForShoppingDisplay.SetActive(false);

        drawCardDisplay.SetActive(_isLocal);
        waitForDrawCardDisplay.SetActive(!_isLocal);

        //waitingDrawText.text = string.Format("Le joueur {0} pioche une carte", GameManager.currentPlayer.PlayerName);
        //animatorUI.SetTrigger("DrawCard");
    }

    /// <summary>
    /// Activates the UI that shows the answers to select.
    /// </summary>
    /// <param name="scenarioID">The ID of the drawn scenario.</param>
    /// <param name="character">The character that draws the scenario.</param>
    /// <param name="isLocal">Indicates if the local player has to select the answer.</param>
    public void ShowAnswers(int scenarioID, Character character)
    {
        drawCardDisplay.SetActive(false);
        waitForDrawCardDisplay.SetActive(false);

        Scenario scenario = Database.GetScenario(GameManager.currentPlayer.fame, scenarioID);

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
        answers[3].SetupInfos(scenario.specificAnswers.Find(x => x.character.nickname == character.nickname));
        scenarioDescription.text = scenario.description;

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
        
        //animatorUI.SetTrigger("ShowAnswers");
    }

    /// <summary>
    /// Activates the UI that shows the opinion to give.
    /// </summary>
    /// <param name="answerID">The ID of the answer.</param>
    public void ShowChooseOpinion(string answer, string question)
    {
        answerDisplay.SetActive(false);
        waitForAnswerDisplay.SetActive(false);

        chooseOpinionDisplay.SetActive(!_isLocal);
        waitForOpinionDisplay.SetActive(_isLocal);

        likesIncrementHandler.UpdateSettings(NetworkPlayer.LocalPlayerInstance.fame);
        
        //animatorUI.SetTrigger("ChooseOpinion");
    }

    /// <summary>
    /// Updates the displayed value of the note given.
    /// </summary>
    /// <param name="newValue">The new value to display.</param>
    public void UpdateOpinion(float newValue)
    {
        noteValue.text = newValue.ToString();
    }

    public void AddLostItem(ItemType itemType, int previousLevel, int newLevel)
    {
        itemsLostText.text = string.Format("{0} Vous avez perdu votre {1} niveau {2}. Ce bien a été rétrogradé au niveau {3}.", itemsLostText.text, itemType.ToString(), previousLevel, newLevel);
    }

    /// <summary>
    /// Activates the UI that shows the results of the opinion phase.
    /// </summary>
    public void ShowOpinionResults(int societyLikes, int likesUpdate, float popularityUpdate, int mentalHealthUpdate)
    {
        chooseOpinionDisplay.SetActive(false);
        waitForOpinionDisplay.SetActive(false);

        showOpinionDisplay.SetActive(true);

        skipOpinionButton.gameObject.SetActive(_isLocal);

        if (societyLikes > 0)
        {
            societyLikesUpdateValue.text = string.Format("+{0}", societyLikes);
        }
        else
        {
            societyLikesUpdateValue.text = societyLikes.ToString();
        }

        if (likesUpdate > 0)
        {
            likesUpdateValue.text = string.Format("+{0}", likesUpdate);
        }
        else
        {
            likesUpdateValue.text = likesUpdate.ToString();
        }

        if(popularityUpdate > 0)
        {
            fameUpdateValue.text = string.Format("+{0}", popularityUpdate);
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

        //likesValue.text = GameManager.currentPlayer.likes.ToString();
        //fameValue.text = GameManager.currentPlayer.fame.ToString("F2");
        mentalHealthValue.text = string.Format("{0}%", GameManager.currentPlayer.mentalHealth);

        //animatorUI.SetTrigger("ShowOpinion");
    }

    /// <summary>
    /// Activates the UI that shows the shop.
    /// </summary>
    public void ShowShop()
    {
        showOpinionDisplay.SetActive(false);

        shoppingDisplay.SetActive(_isLocal);
        waitForShoppingDisplay.SetActive(!_isLocal);

        ShopManager.Instance.UpdateDisplays();

        //animatorUI.SetTrigger("Shop");
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
