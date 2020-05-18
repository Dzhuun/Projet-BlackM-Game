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

    public static List<NetworkPlayer> orderedPlayers;

    public static NetworkPlayer currentPlayer;

    private static List<NetworkPlayer> _players = new List<NetworkPlayer>();

    private Scenario _currentScenario;

    private int _currentIndexTurn = -1;

    private bool _playersReady;

    private GameState _state = GameState.Start;

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

        MoveToNextState();
    }

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
    /// Move to the next state of the game according to the current state.
    /// </summary>
    private void MoveToNextState()
    {
        switch(_state)
        {
            case GameState.Start:
                BeginTurn();
                break;

            case GameState.BeginTurn:
                DrawScenarioPhase();
                break;

            case GameState.DrawScenario:
                SelectAnswerPhase();
                break;
                
            case GameState.SelectAnswer:
                OpinionPhase();
                break;

            case GameState.Opinion:
                ShoppingPhase();
                break;

            case GameState.Shopping:
                BeginTurn();
                break;

        }
    }

    /// <summary>
    /// Starts a turn.
    /// </summary>
    private void BeginTurn()
    {
        _currentIndexTurn = (int)Mathf.Repeat(_currentIndexTurn + 1, orderedPlayers.Count);

        currentPlayer = orderedPlayers[_currentIndexTurn];

        gameUI.ShowDrawCardDisplay(NetworkPlayer.LocalPlayerInstance == currentPlayer);
    }

    /// <summary>
    /// Performs the phase where the player draws a card.
    /// </summary>
    private void DrawScenarioPhase()
    {
        // If the local player has to draw, show the draw UI

        // Else if the local player waits for the currently playing player to draw, show the 'wait for player to draw' UI
    }

    /// <summary>
    /// Performs the phase where the player selects an answer.
    /// </summary>
    private void SelectAnswerPhase()
    {

    }

    /// <summary>
    /// Performs the phase where the players give their opinion.
    /// </summary>
    private void OpinionPhase()
    {

    }

    /// <summary>
    /// Performs the phase where the player shops.
    /// </summary>
    private void ShoppingPhase()
    {
        // Check win condition only locally
    }

    /// <summary>
    /// Checks if the given ID corresponds to a scenario card.
    /// </summary>
    /// <param name="inputField"></param>
    public void OnDrawValidate(TMP_InputField inputField)
    {
        int cardID = -1;
        int.TryParse(inputField.text, out cardID);

        _currentScenario = Database.scenarios.Find(x => x.id == cardID);

        if(_currentScenario != null)
        {
            // Inform all clients to load and show the answers
            photonView.RPC("ShowAnswers", RpcTarget.All, _currentScenario.id);
        }
        else
        {
            // Display error message

        }
    }

    [PunRPC]
    private void ShowAnswers(int scenarioID)
    {
        gameUI.ShowAnswers(scenarioID, currentPlayer.character, currentPlayer == NetworkPlayer.LocalPlayerInstance);
    }

}
