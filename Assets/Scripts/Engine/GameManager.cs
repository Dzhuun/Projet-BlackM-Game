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
    public GameUI gameUI;

    public GameObject playerPrefab;

    public float showOpinionDuration;

    public static List<NetworkPlayer> orderedPlayers;

    public static NetworkPlayer currentPlayer;

    private static List<NetworkPlayer> _players = new List<NetworkPlayer>();

    private List<Answer> _answers = new List<Answer>();

    private Scenario _currentScenario;

    private int _currentIndexTurn = -1;

    private int _currentTurnCount = 0;

    private int _maxTurnCount = 0;

    private bool _playersReady;

    private GameState _state = GameState.Start;

    private int _updateLikesValue = 0;

    private int _opinionCount = 0;

    private int _selectedAnswerID = -1;

    #region MonoBehaviour
    private void Awake()
    {
        // Fill the ordererd players list with null value
        // These null values will be replaced by the players and their index will correspond to their turn order
        orderedPlayers = new List<NetworkPlayer>();
        for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            orderedPlayers.Add(null);
        }
    }

    void Start()
    {
        if(NetworkPlayer.LocalPlayerInstance == null)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        }
    }
    
    #endregion

    #region PUN Callbacks
    // When a player updates its properties, check if all players are ready
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(!_playersReady && PhotonNetwork.IsMasterClient && CheckPlayersLoadedLevel())
        {
            _playersReady = true;

            InitializeGame();
        }
    }

    #endregion

    enum GameState
    {
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
        foreach(Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            object isReady;
            if(player.CustomProperties.TryGetValue(SettingsManager.KEY_PLAYER_LOADED_LEVEL, out isReady))
            {
                if(!((bool)isReady))
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

        if(characters.Count < PhotonNetwork.CurrentRoom.PlayerCount)
        {
            throw new System.Exception("Not enough characters were created. Add more Characters to the Resources/Characters folder.");
        }

        // Create a list of int from 1 to PlayerCount to manager players turn order
        List<int> playerOrder = new List<int>();
        for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerOrder.Add(i);
        }

        int charToAssignIndex;
        int orderToAssignIndex;

        foreach(NetworkPlayer player in _players)
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

        gameUI.DisplayCharacters();

        SettingsManager.transition.FadeOut();

        StartCoroutine(StartGameDelay());
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
    /// Starts the game after a short delay.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartGameDelay()
    {
        yield return new WaitForSeconds(4);

        gameUI.HideCharacters();

        // State : Start -> BeginTurn
        MoveToNextState();
    }

    /// <summary>
    /// Move to the next state of the game according to the current state.
    /// </summary>
    private void MoveToNextState()
    {
        switch(_state)
        {
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

        if(_currentIndexTurn >= orderedPlayers.Count)
        {
            _currentIndexTurn = 0;
        }

        currentPlayer = orderedPlayers[_currentIndexTurn];

        gameUI.BeginTurn(currentPlayer);

        // State : BeginTurn -> DrawScenario
        MoveToNextState();
    }

    /// <summary>
    /// Performs the phase where the player draws a card.
    /// </summary>
    private void DrawScenarioPhase()
    {
        gameUI.BeginTurn(currentPlayer);

        gameUI.ShowDrawCardDisplay();
    }

    /// <summary>
    /// Checks if the given ID corresponds to a scenario card.
    /// </summary>
    /// <param name="inputField"></param>
    public void OnDrawValidate(TMP_InputField inputField)
    {
        int scenarioID = -1;
        int.TryParse(inputField.text, out scenarioID);

        _currentScenario = Database.scenarios.Find(x => x.id == scenarioID);

        if(_currentScenario != null)
        {
            photonView.RPC("SetScenario", RpcTarget.All, _currentScenario.id);
        }
        else
        {
            // Display error message

        }
    }

    /// <summary>
    /// Sets the scenario drawn and continue to the answer phase.
    /// </summary>
    /// <param name="scenarioID">The ID of the scenario drawn.</param>
    [PunRPC]
    private void SetScenario(int scenarioID)
    {
        _currentScenario = Database.scenarios.Find(x => x.id == scenarioID);
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
        _updateLikesValue = 0;
        gameUI.ShowAnswers(_currentScenario.id, currentPlayer.character);
    }

    /// <summary>
    /// Indicates all the clients the selected answer.
    /// </summary>
    /// <param name="answerID">The ID of the selected answer.</param>
    public void SelectAnswer(int answerID)
    {
        photonView.RPC("ReceiveSelectedAnswer", RpcTarget.All, answerID);
    }

    /// <summary>
    /// Receive the answer selected by the currently playing player.
    /// </summary>
    /// <param name="answerID">The ID of the answer selected.</param>
    [PunRPC]
    private void ReceiveSelectedAnswer(int answerID)
    {
        _selectedAnswerID = answerID;

        if(currentPlayer == NetworkPlayer.LocalPlayerInstance)
        {
            Answer selectedAnswer = _answers[answerID - 1];
            _updateLikesValue += selectedAnswer.likesValue;
        }

        // State : SelectAnswer -> Opinion
        MoveToNextState();
    }

    /// <summary>
    /// Performs the phase where the players give their opinion.
    /// </summary>
    private void OpinionPhase()
    {
        _opinionCount = 0;

        gameUI.ShowChooseOpinion(_selectedAnswerID);
    }
    
    /// <summary>
    /// Gives an opinion on a player's choice.
    /// </summary>
    /// <param name="value">The opinion value given.</param>
    public void GiveOpinion(Slider slider)
    {
        photonView.RPC("ReceiveOpinion", RpcTarget.All, (int)slider.value);
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
            _updateLikesValue += value;
            _opinionCount++;

            // If all other players have given their opinion
            if(_opinionCount == PhotonNetwork.CurrentRoom.PlayerCount - 1)
            {
                photonView.RPC("ComputeOpinion", RpcTarget.All, NetworkPlayer.LocalPlayerInstance.orderIndex, _updateLikesValue);
            }
        }
    }

    /// <summary>
    /// Updates the like value of the playing character and move to the next game state.
    /// </summary>
    /// <param name="orderIndex">The order index of the character that has its like value updated.</param>
    /// <param name="newValue">The new like value of the character.</param>
    [PunRPC]
    private void ComputeOpinion(int orderIndex, int likesUpdate)
    {
        orderedPlayers[orderIndex].likes += likesUpdate;

        if(orderedPlayers[orderIndex].likes < 0)
        {
            orderedPlayers[orderIndex].likes = 0;
        }

        orderedPlayers[orderIndex].popularity += likesUpdate * 0.02f;

        StartCoroutine(ShowOpinion(likesUpdate));
    }

    /// <summary>
    /// Shows the opnion results.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowOpinion(int likesUpdate)
    {
        gameUI.ShowOpinionResults(likesUpdate);

        yield return new WaitForSeconds(showOpinionDuration);


        // State : Opinion -> Shopping
        MoveToNextState();
    }

    /// <summary>
    /// Performs the phase where the player shops.
    /// </summary>
    private void ShoppingPhase()
    {
        gameUI.ShowShop();
    }

    /// <summary>
    /// Check if the game is over or begin an other turn.
    /// </summary>
    public void ValidateShopping()
    {
        Debug.LogError($"Current count : {_currentTurnCount} / Max : {_maxTurnCount}");

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
                    if(player.popularity > winner.popularity)
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

    [PunRPC]
    private void EndGame(int playerID)
    {
        gameUI.ShowEndUI(orderedPlayers.Find(x => x.PlayerID == playerID));
    }

    [PunRPC]
    private void EndTurn()
    {
        // State : Shopping -> BeginTurn
        MoveToNextState();
    }

}
