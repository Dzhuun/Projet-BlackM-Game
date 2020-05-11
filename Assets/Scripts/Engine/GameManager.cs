using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    private static List<NetworkPlayer> _players = new List<NetworkPlayer>();

    private bool _playersReady;

    #region PUN Callbacks
    // When a player updates its properties, check if all players are ready
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(!_playersReady && PhotonNetwork.IsMasterClient && CheckPlayersReady())
        {
            _playersReady = true;

            InitializeGame();
        }
    }

    #endregion

    #region MonoBehaviour
    void Start()
    {
        if(NetworkPlayer.LocalPlayerInstance == null)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {SettingsManager.KEY_PLAYER_LOADED_LEVEL, true}
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        }
    }

    void Update()
    {
        if(!_playersReady)
        {
            return;
        }

        if(PhotonNetwork.IsMasterClient)
        {
            UpdateGameState();  
        }
    }

    #endregion

    /// <summary>
    /// Checks if all players in the room are ready (i.e. joined the game scene).
    /// </summary>
    /// <returns>True if all players are ready.</returns>
    private bool CheckPlayersReady()
    {
        foreach(Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            object isReady;
            if(player.CustomProperties.TryGetValue(SettingsManager.KEY_PLAYER_READY, out isReady))
            {
                if(!((bool)isReady))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void InitializeGame()
    {
        // Randomly assign characters to players
        List<Character> characters = Database.characters;

        if(characters.Count < PhotonNetwork.CurrentRoom.PlayerCount)
        {
            throw new System.Exception("Not enough characters were created. Add more Characters to the Resources/Characters folder.");
        }

        int charToAssignIndex;

        foreach(NetworkPlayer player in _players)
        {
            charToAssignIndex = Random.Range(0, characters.Count);
            player.SetCharacter(characters[charToAssignIndex]);

            characters.RemoveAt(charToAssignIndex);
        }

        photonView.RPC("StartGame", RpcTarget.All);
    }

    [PunRPC]
    private void StartGame()
    {
        SettingsManager.transition.FadeOut();


    }

    public static void AddPlayer(NetworkPlayer player)
    {
        _players.Add(player);
    }

    enum GameState
    {
        Start,
        PlayerTurn
    };
    GameState state = GameState.Start;


    public void UpdateGameState()
    {
        switch(state)
        {
            case GameState.Start:
            // wait for player one to click on start button

            break;

            case GameState.PlayerTurn:
            break;
        }
    }
}
