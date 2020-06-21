using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Inspector
    [Header("Component")]
    public GameObject playerPrefab;

    [Header("Settings")]
    public int positiveTraitRespected;
    public int negativeTraitRespected;
    public int traitNotRespected;

    #endregion
    
    public static GameManager Instance { get; private set; }
    public static List<NetworkPlayer> orderedPlayers;
    public static NetworkPlayer currentPlayer;
    private static List<NetworkPlayer> _players = new List<NetworkPlayer>();
    private List<Answer> _answers = new List<Answer>();
    private Scenario _currentScenario;
    private AnswerDisplay _clickedAnswer;
    private Answer _selectedAnswer;
    private int _currentIndexTurn = -1;
    private int _currentTurnCount = 0;
    private int _maxTurnCount = 0;
    private bool _playersReady;
    private int _playersFirstTurnReadyCount = 0;
    private GameState _state = GameState.None;
    private int[] _playersLikes;
    private int _societyLikesValue = 0;
    private int _updateMentalHealth = 0;
    private int _opinionCount = 0;
    private bool _hasLostFame;

    #region MonoBehaviour
    private void Awake()
    {
        Instance = this;

        // Fill the ordererd players list with null value
        // These null values will be replaced by the players and their index will correspond to their turn order
        orderedPlayers = new List<NetworkPlayer>();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            orderedPlayers.Add(null);
        }
    }

    void Start()
    {
        if (NetworkPlayer.LocalPlayerInstance == null)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        }
    }

    #endregion

    #region PUN Callbacks
    // When a player updates its properties, check if all players are ready
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!_playersReady && PhotonNetwork.IsMasterClient && CheckPlayersLoadedLevel())
        {
            _playersReady = true;

            InitializeGame();
        }
    }

    #endregion

    enum GameState
    {
        None,
        Start,
        BeginTurn,
        DrawScenario,
        SelectAnswer,
        Opinion,
        Shopping
    };

    /// <summary>
    /// Checks if all players in the room are ready (i.e. joined the game scene).
    /// </summary>
    /// <returns>True if all players are ready.</returns>
    private bool CheckPlayersLoadedLevel()
    {
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            object isReady;
            if (player.CustomProperties.TryGetValue(SettingsManager.KEY_PLAYER_LOADED_LEVEL, out isReady))
            {
                if (!((bool)isReady))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Initializes the state of the game.
    /// </summary>
    private void InitializeGame()
    {
        // Randomly assign characters to players
        List<Character> characters = Database.characters;

        if (characters.Count < PhotonNetwork.CurrentRoom.PlayerCount)
        {
            throw new System.Exception("Not enough characters were created. Add more Characters to the Resources/Characters folder.");
        }

        // Create a list of int from 1 to PlayerCount to manager players turn order
        List<int> playerOrder = new List<int>();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerOrder.Add(i);
        }

        int charToAssignIndex;
        int orderToAssignIndex;

        foreach (NetworkPlayer player in _players)
        {
            charToAssignIndex = Random.Range(0, characters.Count);
            orderToAssignIndex = Random.Range(0, playerOrder.Count);

            player.SetCharacter(characters[charToAssignIndex], playerOrder[orderToAssignIndex]);

            characters.RemoveAt(charToAssignIndex);
            playerOrder.RemoveAt(orderToAssignIndex);
        }


        photonView.RPC("StartGame", RpcTarget.All);
    }

    /// <summary>
    /// RPC that indicates all the players in the room to start the game.
    /// </summary>
    [PunRPC]
    private void StartGame()
    {
        // The game will end after 10 complete rounds
        _maxTurnCount = PhotonNetwork.CurrentRoom.PlayerCount * 10;
        
        SettingsManager.transition.FadeOut();
        
        // State : Start -> BeginTurn
        MoveToNextState();
    }

    /// <summary>
    /// Shows the informations on the start of the game.
    /// </summary>
    private void StartGamePhase()
    {
        GameUI.Instance.ShowStartPhase1(NetworkPlayer.LocalPlayerInstance);
    }

    /// <summary>
    /// Indicates the master client that a player is ready for the first turn.
    /// </summary>
    public void SendFirstTurnReady()
    {
        GameUI.Instance.DeactivateFirstTurnButton();

        photonView.RPC("AddPlayerReadyForFirstTurn", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void AddPlayerReadyForFirstTurn()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            _playersFirstTurnReadyCount++;

            if(_playersFirstTurnReadyCount == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                photonView.RPC("StartFirstTurn", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void StartFirstTurn()
    {
        MoveToNextState();
    }

    /// <summary>
    /// Adds a player to the cached list of players.
    /// </summary>
    /// <param name="player">The player to add.</param>
    public static void AddPlayer(NetworkPlayer player)
    {
        _players.Add(player);
    }

    /// <summary>
    /// Store the order of a player for the game in a list.
    /// </summary>
    /// <param name="player">The player to set its order.</param>
    /// <param name="order">The order of the player.</param>
    public static void SetPlayerOrder(NetworkPlayer player, int order)
    {
        player.orderIndex = order;
        orderedPlayers[order] = player;
    }

    /// <summary>
    /// Move to the next state of the game according to the current state.
    /// </summary>
    private void MoveToNextState()
    {
        switch (_state)
        {
            case GameState.None:
                _state = GameState.Start;
                StartGamePhase();
                break;

            case GameState.Start:
                _state = GameState.BeginTurn;
                BeginTurn();
                break;

            case GameState.BeginTurn:
                _state = GameState.DrawScenario;
                DrawScenarioPhase();
                break;

            case GameState.DrawScenario:
                _state = GameState.SelectAnswer;
                SelectAnswerPhase();
                break;

            case GameState.SelectAnswer:
                _state = GameState.Opinion;
                OpinionPhase();
                break;

            case GameState.Opinion:
                _state = GameState.Shopping;
                ShoppingPhase();
                break;

            case GameState.Shopping:
                _state = GameState.BeginTurn;
                BeginTurn();
                break;

        }
    }

    /// <summary>
    /// Starts a turn.
    /// </summary>
    private void BeginTurn()
    {
        _currentTurnCount++;
        _currentIndexTurn++;

        if (_currentIndexTurn >= orderedPlayers.Count)
        {
            _currentIndexTurn = 0;
        }

        currentPlayer = orderedPlayers[_currentIndexTurn];

        GameUI.Instance.BeginTurn(currentPlayer);

        // State : BeginTurn -> DrawScenario
        MoveToNextState();
    }

    /// <summary>
    /// Performs the phase where the player draws a card.
    /// </summary>
    private void DrawScenarioPhase()
    {
        //gameUI.BeginTurn(currentPlayer);

        GameUI.Instance.ShowDrawCardDisplay(currentPlayer);
    }

    /// <summary>
    /// Checks if the given ID corresponds to a scenario card.
    /// </summary>
    /// <param name="inputField"></param>
    public void OnDrawValidate(TMP_InputField inputField)
    {
        int scenarioID = -1;
        int.TryParse(inputField.text, out scenarioID);

        _currentScenario = Database.GetScenario(currentPlayer.fame, scenarioID);

        if (_currentScenario != null)
        {
            photonView.RPC("SetScenario", RpcTarget.All, _currentScenario.id);
        }
        else
        {
            // Display error message
            Debug.LogError("Wrong card ID");
        }
    }

    /// <summary>
    /// Sets the scenario drawn and continue to the answer phase.
    /// </summary>
    /// <param name="scenarioID">The ID of the scenario drawn.</param>
    [PunRPC]
    private void SetScenario(int scenarioID)
    {
        _currentScenario = Database.GetScenario(currentPlayer.fame, scenarioID);
        _answers.Clear();
        _answers.AddRange(_currentScenario.commonAnswers);
        _answers.Add(_currentScenario.specificAnswers.Find(x => x.character.nickname == currentPlayer.character.nickname));

        // State : DrawScenario -> SelectAnswer 
        MoveToNextState();
    }

    /// <summary>
    /// Performs the phase where the player selects an answer.
    /// </summary>
    private void SelectAnswerPhase()
    {
        _clickedAnswer = null;

        GameUI.Instance.ShowAnswers(_currentScenario.id, currentPlayer);
    }

    /// <summary>
    /// Selects an answer.
    /// </summary>
    /// <param name="answerDisplay">The UI element that was selected.</param>
    public void SelectAnswer(AnswerDisplay answerDisplay)
    {
        if(_clickedAnswer != null)
        {
            _clickedAnswer.Deselect();
        }

        _clickedAnswer = answerDisplay;
    }

    /// <summary>
    /// Indicates all the clients the selected answer.
    /// </summary>
    /// <param name="answerID">The ID of the selected answer.</param>
    public void ValidateAnswer()
    {
        _selectedAnswer = _answers[_clickedAnswer.answerID - 1];
        _societyLikesValue = _selectedAnswer.likesValue;
        _updateMentalHealth = 0;

        bool traitRespected = false;

        // Check all the respected traits
        // Add or remove mental health depending whether the traits were respected or not
        foreach (AnswerTrait answerTrait in _selectedAnswer.traits)
        {
            traitRespected = false;

            foreach (CharacterTrait charTrait in currentPlayer.character.traits)
            {
                if (charTrait.trait == answerTrait.trait)
                {
                    traitRespected = charTrait.isActive;
                    break;
                }
            }

            _updateMentalHealth += GetMentalHealthResult(answerTrait, traitRespected);
        }

        ComputeMentalHealth(_updateMentalHealth);

        photonView.RPC("ReceiveSelectedAnswer", RpcTarget.Others, _clickedAnswer.answerID, _societyLikesValue, _updateMentalHealth);

        if (currentPlayer.GetMentalHealthLevel() >= 2 && !currentPlayer.hasInactiveTrait)
        {
            string randomTrait = currentPlayer.character.traits[Random.Range(0, currentPlayer.character.traits.Count)].trait.GetTraitName(currentPlayer.character);

            photonView.RPC("SendDeactivateTrait", RpcTarget.All, randomTrait);
        }
        else if (currentPlayer.GetMentalHealthLevel() < 2 && currentPlayer.hasInactiveTrait)
        {
            photonView.RPC("ResetInactiveTraits", RpcTarget.All);
        }

        // State : SelectAnswer -> Opinion
        MoveToNextState();
    }

    /// <summary>
    /// Informs all the other players that a trait has been deactivated.
    /// </summary>
    /// <param name="traitName">The name of the deactivated trait.</param>
    [PunRPC]
    private void SendDeactivateTrait(string traitName)
    {
        currentPlayer.hasInactiveTrait = true;
        currentPlayer.character.traits.Find(x => x.trait.GetTraitName(currentPlayer.character) == traitName).isActive = false;
    }

    /// <summary>
    /// Informs all the other players that the traits are all reactivated.
    /// </summary>
    [PunRPC]
    private void ResetInactiveTraits()
    {
        currentPlayer.ResetTraits();

        currentPlayer.hasInactiveTrait = false;
    }

    /// <summary>
    /// Updates the player's mental health according to the answer trait condition.
    /// </summary>
    /// <param name="trait">An answer trait.</param>
    /// <param name="isRespected">True if the trait was respected, false otherwise.</param>
    private int GetMentalHealthResult(AnswerTrait trait, bool isRespected)
    {
        if (isRespected)
        {
            return trait.trait.isNegative ? negativeTraitRespected : positiveTraitRespected;
        }
        else
        {
            return traitNotRespected;
        }
    }

    /// <summary>
    /// Receive the answer selected by the currently playing player.
    /// </summary>
    /// <param name="answerID">The ID of the answer selected.</param>
    [PunRPC]
    private void ReceiveSelectedAnswer(int answerID, int societyLikes, int mentalHealthUpdate)
    {
        _selectedAnswer = _answers[answerID - 1];
        _societyLikesValue = societyLikes;

        ComputeMentalHealth(mentalHealthUpdate);

        // State : SelectAnswer -> Opinion
        MoveToNextState();
    }

    /// <summary>
    /// Changes the current player's mental health and returns the net update value.
    /// </summary>
    /// <param name="mentalHealthUpdate">The brut update value.</param>
    /// <returns>The net update value.</returns>
    private void ComputeMentalHealth(int mentalHealthUpdate)
    {
        //int previousMentalHealth = currentPlayer.mentalHealth;
        currentPlayer.mentalHealth = Mathf.Clamp(currentPlayer.mentalHealth + mentalHealthUpdate, 0, 100);

        //return currentPlayer.mentalHealth - previousMentalHealth;
    }

    /// <summary>
    /// Performs the phase where the players give their opinion.
    /// </summary>
    private void OpinionPhase()
    {
        _opinionCount = 0;
        _hasLostFame = false;

        _playersLikes = new int[PhotonNetwork.CurrentRoom.PlayerCount - 1];

        GameUI.Instance.ShowChooseOpinion(_selectedAnswer.text, _currentScenario.description);
    }

    /// <summary>
    /// Gives an opinion on a player's choice.
    /// </summary>
    /// <param name="value">The opinion value given.</param>
    public void GiveOpinion()
    {
        GameUI.Instance.DeactivateOpinionButton();

        photonView.RPC("ReceiveOpinion", RpcTarget.All, (int)LikesIncrementHandler.noteValue);
    }

    /// <summary>
    /// Receive the result of an opinion from a player.
    /// </summary>
    /// <param name="value">The opinion value received.</param>
    [PunRPC]
    private void ReceiveOpinion(int value)
    {
        if (currentPlayer == NetworkPlayer.LocalPlayerInstance)
        {
            _playersLikes[_opinionCount] = value;

            _opinionCount++;

            // If all other players have given their opinion
            if (_opinionCount == PhotonNetwork.CurrentRoom.PlayerCount - 1)
            {
                // Compute first the likes and fame earned locally
                ComputeLikes(_playersLikes, _societyLikesValue);

                // In order to check if a trait was earned or not
                ComputeEarnedTraits();

                photonView.RPC("ComputeLikes", RpcTarget.Others, _playersLikes, _societyLikesValue);
            }
        }
    }

    /// <summary>
    /// Updates the like value of the playing character and move to the next game state.
    /// </summary>
    /// <param name="orderIndex">The order index of the character that has its like value updated.</param>
    /// <param name="newValue">The new like value of the character.</param>
    [PunRPC]
    private void ComputeLikes(int[] playersLikes, int societyLikes)
    {
        int likesUpdate = societyLikes;

        _playersLikes = playersLikes;

        int positivePlayerLikes = 0;
        int negativePlayerLikes = 0;

        foreach(int i in playersLikes)
        {
            if(i < 0)
            {
                negativePlayerLikes += currentPlayer.GetMentalHealthLevel() > 3 ? -(i * i) : i;
            }
            else
            {
                positivePlayerLikes += i;
            }
        }
        
        likesUpdate += positivePlayerLikes;
        likesUpdate += negativePlayerLikes;

        int mentalHealthDislikes = 0;

        if (currentPlayer.GetMentalHealthLevel() > 1)
        {
            mentalHealthDislikes -= 5;

            if (currentPlayer.GetMentalHealthLevel() == 4)
            {
                mentalHealthDislikes -= 30;
            }
        }

        likesUpdate += mentalHealthDislikes;

        currentPlayer.likes += likesUpdate;

        if (currentPlayer.likes < 0)
        {
            currentPlayer.likes = 0;
        }
        
        float popularityUpdate = likesUpdate * 0.02f;

        // Correcting some floating point precision error (hoping it really does)
        popularityUpdate = Mathf.Round(popularityUpdate * 100) / 100;

        float previousFame = currentPlayer.fame;

        if (previousFame + popularityUpdate > 5)
        {
            popularityUpdate = 5 - previousFame;
            currentPlayer.fame = 5;
        }
        else
        {
            currentPlayer.fame += popularityUpdate;
        }

        if(Mathf.FloorToInt(previousFame) > Mathf.FloorToInt(currentPlayer.fame))
        {
            ComputeLostItems();
        }

        GameUI.Instance.ShowOpinionResults(positivePlayerLikes, negativePlayerLikes, _societyLikesValue, mentalHealthDislikes, likesUpdate, popularityUpdate, _selectedAnswer.text, _currentScenario.description);
    }

    /// <summary>
    /// Checks if a new trait must be added to the currentPlayer and adds it.
    /// </summary>
    private void ComputeEarnedTraits()
    {
        // Check for trait earned
        if (!currentPlayer.firstTraitEarned)
        {
            if (currentPlayer.fame >= 3)
            {
                // Add first random trait
                AddRandomTrait(currentPlayer);
                currentPlayer.firstTraitEarned = true;
            }
        }

        if(!currentPlayer.secondTraitEarned)
        {
            if(currentPlayer.fame >= 4)
            {
                // Add second random trait
                AddRandomTrait(currentPlayer);
                currentPlayer.secondTraitEarned = true;
            }
        }
    }

    /// <summary>
    /// Adds a random (not already owned) trait the a player.
    /// </summary>
    /// <param name="player">The player to add a random trait.</param>
    private void AddRandomTrait(NetworkPlayer player)
    {
        List<Trait> traits = new List<Trait>(Database.positiveTraits);
        
        foreach(CharacterTrait charTrait in player.character.traits)
        {
            traits.Remove(charTrait.trait);
        }

        CharacterTrait newTrait = new CharacterTrait(traits[Random.Range(0, traits.Count)]);

        player.character.traits.Add(newTrait);

        photonView.RPC("SendNewTrait", RpcTarget.Others, newTrait.trait.GetTraitName(player.character));
    }

    /// <summary>
    /// Informs all other players that a new trait has been added.
    /// </summary>
    /// <param name="newTraitName">The name of the new trait.</param>
    [PunRPC]
    private void SendNewTrait(string newTraitName)
    {
        // Find the trait to add to the current player
        CharacterTrait traitToAdd = new CharacterTrait(Database.positiveTraits.Find(x => x.GetTraitName(currentPlayer.character) == newTraitName));

        currentPlayer.character.traits.Add(traitToAdd);
    }

    /// <summary>
    /// Checks if any item has to be downgraded and sent it to the network.
    /// </summary>
    private void ComputeLostItems()
    {
        _hasLostFame = true;

        float popularityLevel = Mathf.Floor(currentPlayer.fame);

        if(popularityLevel < 1)
        {
            popularityLevel = 1;
        }

        if (currentPlayer.car.level > popularityLevel)
        {
            photonView.RPC("SendLostItem", RpcTarget.All, ItemType.Voiture, currentPlayer.car.level, (int)popularityLevel);
        }

        if (currentPlayer.house.level > popularityLevel)
        {
            photonView.RPC("SendLostItem", RpcTarget.All, ItemType.Maison, currentPlayer.house.level, (int)popularityLevel);
        }

        if (currentPlayer.work.level > popularityLevel)
        {
            photonView.RPC("SendLostItem", RpcTarget.All, ItemType.Travail, currentPlayer.work.level, (int)popularityLevel);
        }

        if (currentPlayer.friends.level > popularityLevel)
        {
            photonView.RPC("SendLostItem", RpcTarget.All, ItemType.Amis, currentPlayer.friends.level, (int)popularityLevel);
        }

        if (currentPlayer.family.level > popularityLevel)
        {
            photonView.RPC("SendLostItem", RpcTarget.All, ItemType.Famille, currentPlayer.family.level, (int)popularityLevel);
        }
    }

    /// <summary>
    /// Informs all the players that an item has been downgraded.
    /// </summary>
    /// <param name="itemType">The downgraded item.</param>
    /// <param name="previousLevel">The previous level of the item.</param>
    /// <param name="newLevel">The new level of the item.</param>
    [PunRPC]
    private void SendLostItem(ItemType itemType, int previousLevel, int newLevel)
    {
        GameUI.Instance.AddLostItem(itemType, previousLevel, newLevel);

        switch(itemType)
        {
            case ItemType.Voiture:
                currentPlayer.car.level = newLevel;
                break;

            case ItemType.Travail:
                currentPlayer.work.level = newLevel;
                break;

            case ItemType.Amis:
                currentPlayer.friends.level = newLevel;
                break;

            case ItemType.Maison:
                currentPlayer.house.level = newLevel;
                break;

            case ItemType.Famille:
                currentPlayer.family.level = newLevel;
                break;
        }
    }

    /// <summary>
    /// Informs all player to continue to the next state.
    /// </summary>
    public void SkipOpinion()
    {
        photonView.RPC("SendSkipOpinion", RpcTarget.All);
    }

    /// <summary>
    /// Continues to the shopping or show the fame lost.
    /// </summary>
    [PunRPC]
    private void SendSkipOpinion()
    {
        if(_hasLostFame)
        {
            GameUI.Instance.ShowFameLost();
        }
        else
        {
            // Opinion -> Shopping
            MoveToNextState();
        }
    }

    /// <summary>
    /// Informs all player to continue to the shoppping phase.
    /// </summary>
    public void SkipToShop()
    {
        // Opinion -> Shopping
        photonView.RPC("SendSkipToShop", RpcTarget.All);
    }

    /// <summary>
    /// Continues to the shop.
    /// </summary>
    [PunRPC]
    private void SendSkipToShop()
    {
        MoveToNextState();
    }

    /// <summary>
    /// Performs the phase where the player shops.
    /// </summary>
    private void ShoppingPhase()
    {
        GameUI.Instance.ShowShop(currentPlayer);
    }

    /// <summary>
    /// Upgrades an item and update the shop UI.
    /// </summary>
    /// <param name="itemType">The item to upgrade.</param>
    /// <param name="upgradeCost">The upgrade cost as likes.</param>
    public static void BuyUpgrade(ItemType itemType, int upgradeCost)
    {
        currentPlayer.BuyUpgrade(itemType, upgradeCost);
    }

    /// <summary>
    /// Check if the game is over or begin an other turn.
    /// </summary>
    public void ValidateShopping()
    {
        // Game end condition
        if(_currentTurnCount >= _maxTurnCount)
        {
            NetworkPlayer winner = null;

            foreach(NetworkPlayer player in orderedPlayers)
            {
                if(winner == null)
                {
                    winner = player;
                }
                else
                {
                    // TO DO : handle draw
                    if(player.fame > winner.fame)
                    {
                        winner = player;
                    }
                }
            }

            photonView.RPC("EndGame", RpcTarget.All, winner.PlayerID);
        }
        else
        {
            photonView.RPC("EndTurn", RpcTarget.All);
        }
    }

    /// <summary>
    /// Ends the game.
    /// </summary>
    /// <param name="playerID">The ID of the winning player.</param>
    [PunRPC]
    private void EndGame(int playerID)
    {
        GameUI.Instance.ShowEndUI(orderedPlayers.Find(x => x.PlayerID == playerID));
    }

    /// <summary>
    /// Ends the turn.
    /// </summary>
    [PunRPC]
    private void EndTurn()
    {
        // State : Shopping -> BeginTurn
        MoveToNextState();
    }

}
