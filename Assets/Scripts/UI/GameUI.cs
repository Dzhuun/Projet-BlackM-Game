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
    public Color deactivatedTraitColor = Color.gray;
    public TextMeshProUGUI traitsList;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerLikes;
    public TextMeshProUGUI playerFame;
    public TextMeshProUGUI playerDescription;

    [Header("Start")]
    public GameObject charactersDisplay;

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
    public LikesIncrementHandler likesIncrementHandler;

    [Header("WaitForOpinion")]
    public GameObject waitForOpinionDisplay;
    public TextMeshProUGUI waitForOpinionTitle;
    public TextMeshProUGUI waitForOpinionText;


    [Header("ShowOpinion")]
    public GameObject showOpinionDisplay;
    public GameObject opinionPanel;
    public GameObject lostFamePanel;
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
    public TextMeshProUGUI mentalHealthValue;
    public Button skipOpinionButton;

    [Header("Shopping")]
    public GameObject shoppingDisplay;
    public TextMeshProUGUI shoppingText;

    [Header("WaitForShopping")]
    public GameObject waitForShoppingDisplay;
    public TextMeshProUGUI waitForShoppingText;

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
    public void ShowChooseOpinion(string answer, string question)
    {
        answerDisplay.SetActive(false);
        waitForAnswerDisplay.SetActive(false);

        RefreshPlayerInfos();

        chooseOpinionDisplay.SetActive(!_isLocal);
        waitForOpinionDisplay.SetActive(_isLocal);

        validateOpinionButton.interactable = true;
        validateOpinionText.SetActive(true);
        validatedOpinionIcon.SetActive(false);

        scenarioDescription_chooseOpinion.text = question;
        answerText_chooseOpinion.text = answer;
        voteTitle.text = string.Format("Note {0}",GameManager.currentPlayer.character.GetFirstName());

        likesIncrementHandler.UpdateSettings(NetworkPlayer.LocalPlayerInstance.fame);
        maxVoteText.text = string.Format("Tu peux distribuer {0} votes au maximum", LikesIncrementHandler.maxValue);
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
        //itemsLostText.text = string.Format("{0} Vous avez perdu votre {1} niveau {2}. Ce bien a été rétrogradé au niveau {3}.", itemsLostText.text, itemType.ToString(), previousLevel, newLevel);
    }

    /// <summary>
    /// Activates the UI that shows the results of the opinion phase.
    /// </summary>
    public void ShowOpinionResults(int positivePlayersLikes, int negativePlayersLikes, int societyLikes, int mentalHealthDislikes, int totalLikesUpdate, float popularityUpdate, string answer, string situation)
    {
        chooseOpinionDisplay.SetActive(false);
        waitForOpinionDisplay.SetActive(false);

        showOpinionDisplay.SetActive(true);
        opinionPanel.SetActive(true);
        lostFamePanel.SetActive(false);

        RefreshPlayerInfos();

        skipOpinionButton.gameObject.SetActive(_isLocal);

        scenarioDescription_showOpinion.text = situation;
        answerText_showOpinion.text = answer;

        positivePlayersLikesUpdateValue.text = string.Format("+{0}", positivePlayersLikes);
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
        
        mentalHealthUpdateValue.text = mentalHealthDislikes.ToString();    

        //likesValue.text = GameManager.currentPlayer.likes.ToString();
        //fameValue.text = GameManager.currentPlayer.fame.ToString("F2");
        mentalHealthValue.text = string.Format("{0}%", GameManager.currentPlayer.mentalHealth);

        //animatorUI.SetTrigger("ShowOpinion");
    }

    /// <summary>
    /// Activates the UI that shows the fame and items lost.
    /// </summary>
    public void ShowFameLost()
    {
        opinionPanel.SetActive(false);
        lostFamePanel.SetActive(true);
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
            shoppingText.text = string.Format("Souhaites-tu acheter un bien {0} étoile{1}", fameLevel, fameLevel > 1 ? "s" : string.Empty);
        }
        else
        {
            waitForShoppingText.text = string.Format("Patience, {0} effectue ses achats", player.character.GetFirstName());
        }

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
